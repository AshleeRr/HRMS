using HRMS.Application.DTOs.RoomManagementDto.TarifaDtos;
using HRMS.Application.Interfaces.RoomManagementService;
using HRMS.Domain.Base;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.Extensions.Logging;
using MyValidator.Validator;

namespace HRMS.Application.Services.RoomServices;

public class TarifaServices : ITarifaService
{
    private readonly ITarifaRepository _tarifaRepository;
    private readonly ILogger<TarifaServices> _logger;
    private readonly ICategoryRepository _categoriaRepository;
    private readonly Validator<CreateTarifaDto> _validator;

    public TarifaServices(ITarifaRepository tarifaRepository, ILogger<TarifaServices> logger,
        ICategoryRepository categoriaRepository, Validator<CreateTarifaDto> validator)
    {
        _tarifaRepository = tarifaRepository;
        _logger = logger;
        _categoriaRepository = categoriaRepository;
        _validator = validator;
    }

    public async Task<OperationResult> GetAll()
    {
        return await OperationResult.ExecuteOperationAsync(async () =>
        {
            _logger.LogInformation("Buscando todas las tarifas.");
            var tarifas = await _tarifaRepository.GetAllAsync();
            return tarifas.Any() ? OperationResult.Success(tarifas.Select(MapToDto)) : OperationResult.Failure("No se encontraron tarifas.");
        });
    }

    public async Task<OperationResult> GetById(int id)
    {
        var validarId = validateId(id, "El ID de la tarifa debe ser mayor que 0.");
        if (!validarId.IsSuccess) return validarId;
        return await OperationResult.ExecuteOperationAsync(async () =>
            {
                _logger.LogInformation("Buscando la tarifa por ID: {Id}", id);
                if (id <= 0) return OperationResult.Failure("El ID de la tarifa debe ser mayor que 0.");
                var tarifa = await _tarifaRepository.GetEntityByIdAsync(id);
                return tarifa != null ? OperationResult.Success(MapToDto(tarifa)) :OperationResult. Failure($"No se encontró la tarifa con ID {id}.");
            });
    }
    
    public async Task<OperationResult> Update(UpdateTarifaDto dto)
    {
        return await OperationResult.ExecuteOperationAsync(async () =>
        {
           
            var validarId = validateId(dto.IdTarifa, "El ID de la tarifa debe ser mayor que 0.");
            if (!validarId.IsSuccess) return validarId;
            var validar = _validator.Validate(dto);
            if (!validar.IsSuccess) return validar;

            if (dto.IdCategoria > 0)
            {
                var validacionCategoria = await ValidarCategoria(dto.IdCategoria);
                if (!validacionCategoria.IsSuccess) return validacionCategoria;
            }
            
            _logger.LogInformation("Actualizando la tarifa con ID: {Id}", dto.IdTarifa);

            var existingTarifa = await _tarifaRepository.GetEntityByIdAsync(dto.IdTarifa);
            if (existingTarifa == null)
                return OperationResult.Failure($"No se encontró la tarifa con ID {dto.IdTarifa}.");

            UpdateDtoFields(existingTarifa, dto);

            if (dto.IdCategoria > 0)
            {
                existingTarifa.IdCategoria = dto.IdCategoria;
            }

            var result = await _tarifaRepository.UpdateEntityAsync(existingTarifa);
            return result.IsSuccess && result.Data != null
                ? OperationResult.Success(MapToDto((Tarifas)result.Data), "Tarifa actualizada correctamente.")
                : OperationResult.Failure(result.Message ?? "Error al actualizar la tarifa.");
        });
    }

    public async Task<OperationResult> Save(CreateTarifaDto dto)
    {
        return await OperationResult.ExecuteOperationAsync(async () =>
        {
            var validation = _validator.Validate(dto);
            if (!validation.IsSuccess) return validation;
            var validacionCategoria = await ValidarCategoria(dto.IdCategoria);
            if (!validacionCategoria.IsSuccess) return validacionCategoria;
            

            _logger.LogInformation("Creando una nueva tarifa.");

            var tarifa = new Tarifas
            {
                IdCategoria = dto.IdCategoria,
                Descripcion = dto.Descripcion,
                Estado = true,
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin,
                PrecioPorNoche = dto.PrecioPorNoche,
                Descuento = dto.Descuento
            };

            var result = await _tarifaRepository.SaveEntityAsync(tarifa);
            return result.IsSuccess && result.Data != null
                ? OperationResult.Success(MapToDto((Tarifas)result.Data), "Tarifa creada correctamente.")
                : OperationResult.Failure(result.Message ?? "Error al guardar la tarifa.");
        });
    }

    public async Task<OperationResult> Remove(DeleteTarifaDto dto)
    {
        return await OperationResult.ExecuteOperationAsync(async () =>
            {
                _logger.LogInformation("Eliminando la tarifa con ID: {Id}", dto.IdTarifa);
                if (dto.IdTarifa <= 0)
                    return OperationResult.Failure("El ID de la tarifa debe ser mayor que 0.");

                var tarifa = await _tarifaRepository.GetEntityByIdAsync(dto.IdTarifa);
                if (tarifa == null)
                    return OperationResult.Failure($"No se encontró la tarifa con ID {dto.IdTarifa}.");

                tarifa.Estado = false;
                var result = await _tarifaRepository.UpdateEntityAsync(tarifa);
                return result.IsSuccess && result.Data != null
                    ? OperationResult.Success(MapToDto((Tarifas)result.Data), "Tarifa eliminada correctamente.")
                    : result;
            });
    }

    public async Task<OperationResult> GetTarifasVigentes(string fechaInput)
    {
        return await OperationResult.ExecuteOperationAsync(async () =>
        {
            _logger.LogInformation("Buscando tarifas vigentes para la fecha ingresada: {FechaInput}", fechaInput);

            var fechaValidacion = ValidateFechaFormat(fechaInput);
            if (!fechaValidacion.IsSuccess) return fechaValidacion;

            DateTime fecha = (DateTime)fechaValidacion.Data;

            return await _tarifaRepository.GetTarifasVigentesAsync(fechaInput);
        });
    }

    public async Task<OperationResult> GetHabitacionesByPrecio(decimal precio)
    {
        return await OperationResult.ExecuteOperationAsync(async () =>
        {
            _logger.LogInformation("Buscando habitaciones con precio de tarifa: {Precio}", precio);

            if (precio <= 0)
                return OperationResult.Failure("El precio de la tarifa debe ser mayor a 0.");

            var operationResult = await _tarifaRepository.GetHabitacionByPrecioAsync(precio);

            if (!operationResult.IsSuccess)
            {
                return OperationResult.Failure(operationResult.Message);
            }

            if (operationResult.Data == null)
            {
                return OperationResult.Failure("No se encontraron habitaciones con el precio de tarifa indicado.");
            }

            if (operationResult.Data is IEnumerable<Habitacion> habitacionesEnum)
            {
                var habitaciones = habitacionesEnum.ToList();
                if (habitaciones.Any())
                {
                    return OperationResult.Success(habitaciones);
                }
            }

            return OperationResult.Failure("No se encontraron habitaciones con el precio de tarifa indicado.");
        });
    }


    private static void UpdateDtoFields(Tarifas existingTarifa, UpdateTarifaDto dto)
    {
        existingTarifa.Descripcion = dto.Descripcion ?? existingTarifa.Descripcion;
        existingTarifa.PrecioPorNoche = dto.PrecioPorNoche;
        existingTarifa.Descuento = dto.Descuento;
        existingTarifa.FechaInicio = dto.FechaInicio;
        existingTarifa.FechaFin = dto.FechaFin;
    }

    private static TarifaDto MapToDto(Tarifas tarifas) => new()
    {
        Descripcion = tarifas.Descripcion,
        PrecioPorNoche = tarifas.PrecioPorNoche,
        Descuento = tarifas.Descuento,
        FechaInicio = tarifas.FechaInicio,
        FechaFin = tarifas.FechaFin,
        IdCategoria = tarifas.IdCategoria,
    };

    

    private async Task<OperationResult> ValidarCategoria(int idCategoria)
    {
        
        var categoria = await _categoriaRepository.GetEntityByIdAsync(idCategoria);

        if (categoria == null)
            return OperationResult.Failure($"No existe una categoría con ID {idCategoria} en la base de datos.");

        if (categoria.Estado == false)
            return OperationResult.Failure(
                $"La categoría con ID {idCategoria} está inactiva. Active la categoría antes de asociarla a una tarifa.");

        return OperationResult.Success();
    }
    
    
    private static OperationResult validateId(int id, string message)
    {
        if (id <= 0)
            return OperationResult.Failure(message);
        return OperationResult.Success();
    }

    private static OperationResult ValidateFechaFormat(string fechaInput)
    {
        if (string.IsNullOrWhiteSpace(fechaInput))
            return new OperationResult { IsSuccess = false, Message = "La fecha no puede estar vacía." };

        DateTime fecha;
        string[] formatosValidos = { "dd/MM/yyyy", "yyyy-MM-dd", "MM/dd/yyyy", "dd-MM-yyyy" };
        string formatosEjemplo = string.Join(", ", formatosValidos);

        if (!DateTime.TryParseExact(fechaInput, formatosValidos,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.None, out fecha))
        {
            return new OperationResult
            {
                IsSuccess = false,
                Message =
                    $"El formato de la fecha '{fechaInput}' es incorrecto. Debe usar uno de los siguientes formatos: {formatosEjemplo}."
            };
        }

        return new OperationResult { IsSuccess = true, Data = fecha };
    }
}

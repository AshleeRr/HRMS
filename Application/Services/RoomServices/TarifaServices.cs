using HRMS.Application.DTOs.RoomManagementDto.TarifaDtos;
using HRMS.Application.Interfaces.RoomManagementService;
using HRMS.Domain.Base;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Repositories.RoomRepository;
using Microsoft.Extensions.Logging;

namespace HRMS.Application.Services.RoomServices;

public class TarifaServices : ITarifaService
{
    private readonly TarifaRepository _tarifaRepository;
    private readonly ILogger<TarifaServices> _logger;

    public TarifaServices(TarifaRepository tarifaRepository, ILogger<TarifaServices> logger)
    {
        _tarifaRepository = tarifaRepository;
        _logger = logger;
    }

    public async Task<OperationResult> GetAll()
    {
        return await ExecuteOperationAsync(async () =>
        {
            _logger.LogInformation("Buscando todas las tarifas.");
            var tarifas = await _tarifaRepository.GetAllAsync();
            return tarifas.Any() ? Success(tarifas.Select(MapToDto)) : Failure("No se encontraron tarifas.");
        }, "Error al buscar todas las tarifas.");
    }

    public async Task<OperationResult> GetById(int id)
    {
        return await ExecuteOperationAsync(async () =>
        {
            _logger.LogInformation("Buscando la tarifa por ID: {Id}", id);
            if (id <= 0) return Failure("El ID de la tarifa debe ser mayor que 0.");
            var tarifa = await _tarifaRepository.GetEntityByIdAsync(id);
            return tarifa != null ? Success(MapToDto(tarifa)) : Failure($"No se encontró la tarifa con ID {id}.");
        }, $"Error al buscar la tarifa con ID {id}.");
    }

    public async Task<OperationResult> Update(UpdateTarifaDto dto)
    {
        return await ExecuteOperationAsync(async () =>
        {
            var validacion = ValidarDescripcion(dto.Descripcion);
            if (!validacion.IsSuccess) return validacion;

            ValidarFechas(dto.FechaInicio, dto.FechaFin);
            ValidarPrecios(dto.PrecioPorNoche, dto.Descuento);

            _logger.LogInformation("Actualizando la tarifa con ID: {Id}", dto.IdTarifa);

            var existingTarifa = await _tarifaRepository.GetEntityByIdAsync(dto.IdTarifa);
            if (existingTarifa == null) return Failure($"No se encontró la tarifa con ID {dto.IdTarifa}.");

            UpdateDtoFields(existingTarifa, dto);

            var result = await _tarifaRepository.UpdateEntityAsync(existingTarifa);
            return result.IsSuccess && result.Data != null
                ? Success(MapToDto((Tarifas)result.Data), "Tarifa actualizada correctamente.")
                : result;
        }, "Error al actualizar la tarifa.");
    }

    public async Task<OperationResult> Save(CreateTarifaDto dto)
    {
        return await ExecuteOperationAsync(async () =>
        {
            var validacion = ValidarDescripcion(dto.Descripcion);
            if (!validacion.IsSuccess) return validacion;

            ValidarFechas(dto.FechaInicio, dto.FechaFin);
            ValidarPrecios(dto.PrecioPorNoche, dto.Descuento);

            _logger.LogInformation("Creando una nueva tarifa.");

            var tarifa = new Tarifas
            {
                Descripcion = dto.Descripcion,
                Estado = true,
                FechaInicio = dto.FechaInicio,
                FechaFin = dto.FechaFin,
                PrecioPorNoche = dto.PrecioPorNoche,
                Descuento = dto.Descuento
            };

            var result = await _tarifaRepository.SaveEntityAsync(tarifa);
            return result.IsSuccess && result.Data != null
                ? Success(MapToDto((Tarifas)result.Data), "Tarifa creada correctamente.")
                : result;
        }, "Error al crear la tarifa.");
    }

    public async Task<OperationResult> Remove(DeleteTarifaDto dto)
    {
        return await ExecuteOperationAsync(async () =>
        {
            _logger.LogInformation("Eliminando la tarifa con ID: {Id}", dto.IdTarifa);
            if (dto.IdTarifa <= 0)
                return Failure("El ID de la tarifa debe ser mayor que 0.");

            var tarifa = await _tarifaRepository.GetEntityByIdAsync(dto.IdTarifa);
            if (tarifa == null)
                return Failure($"No se encontró la tarifa con ID {dto.IdTarifa}.");

            tarifa.Estado = false;
            var result = await _tarifaRepository.UpdateEntityAsync(tarifa);
            return result.IsSuccess && result.Data != null
                ? Success(MapToDto((Tarifas)result.Data), "Tarifa eliminada correctamente.")
                : result;
        }, $"Error al eliminar la tarifa con ID {dto.IdTarifa}.");
    }

    public async Task<OperationResult> GetTarifasVigentes(string fechaInput)
    {
        return await ExecuteOperationAsync(async () =>
        {
            _logger.LogInformation("Buscando tarifas vigentes para la fecha ingresada: {FechaInput}", fechaInput);

            var fechaValidacion = ValidateFechaFormat(fechaInput);
            if (!fechaValidacion.IsSuccess) return fechaValidacion;

            DateTime fecha = (DateTime)fechaValidacion.Data;

            return await _tarifaRepository.GetTarifasVigentesAsync(fechaInput);
        }, "Error al buscar tarifas vigentes.");
    }
    
    public async Task<OperationResult> GetHabitacionesByPrecio(decimal precio)
    {
        try
        {
            _logger.LogInformation("Buscando habitaciones con precio de tarifa: {Precio}", precio);
            if (precio <= 0)
                return Failure("El precio de la tarifa debe ser mayor a 0.");
            var operationResult = await _tarifaRepository.GetHabitacionByPrecioAsync(precio);
            var habitaciones = operationResult.Data as List<Habitacion>;
            return habitaciones != null
                ? Success(habitaciones)
                : Failure(" No se encontraron habitaciones con el precio de tarifa indicado.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar habitaciones por precio de tarifa.");
            return Failure("Error al buscar habitaciones por precio de tarifa.");
        }
    }
    

    private static OperationResult ValidarDescripcion(string descripcion)
    {
        if (string.IsNullOrWhiteSpace(descripcion))
            return Failure("La descripción de la tarifa es requerida.");
        if (descripcion.Length > 255)
            return Failure("La descripción de la tarifa no puede exceder los 255 caracteres.");
        return Success();
    }

    private static void ValidarFechas(DateTime fechaInicio, DateTime fechaFin)
    {
        if (fechaInicio == DateTime.MinValue)
            throw new ArgumentException("La fecha de inicio de la tarifa es requerida.");
        if (fechaFin == DateTime.MinValue)
            throw new ArgumentException("La fecha de fin de la tarifa es requerida.");
        if (fechaInicio > fechaFin)
            throw new ArgumentException("La fecha de inicio no puede ser mayor a la fecha de fin.");
    }

    private static void ValidarPrecios(decimal precio, decimal descuento)
    {
        if (precio <= 0)
            throw new ArgumentException("El precio de la tarifa debe ser mayor a 0.");
        if (descuento < 0)
            throw new ArgumentException("El descuento no puede ser menor a 0.");
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
        FechaFin = tarifas.FechaFin
    };

    private static OperationResult Success(object data = null, string message = null) =>
        new() { IsSuccess = true, Data = data, Message = message };

    private static OperationResult Failure(string message) =>
        new() { IsSuccess = false, Message = message };

    private async Task<OperationResult> ExecuteOperationAsync(Func<Task<OperationResult>> operation, string errorMessage)
    {
        try
        {
            return await operation();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, errorMessage);
            return Failure(errorMessage);
        }
    }
    
    private static OperationResult ValidateFechaFormat(string fechaInput)
    {
        if (string.IsNullOrWhiteSpace(fechaInput))
            return new OperationResult { IsSuccess = false, Message = "La fecha no puede estar vacía." };

        DateTime fecha;
        string[] formatosValidos = { "dd/MM/yyyy", "yyyy-MM-dd", "MM/dd/yyyy", "dd-MM-yyyy" };

        if (!DateTime.TryParseExact(fechaInput, formatosValidos, 
                System.Globalization.CultureInfo.InvariantCulture, 
                System.Globalization.DateTimeStyles.None, out fecha))
        {
            return new OperationResult
            {
                IsSuccess = false,
                Message = "El formato de la fecha es incorrecto. Usa formatos válidos: dd/MM/yyyy, yyyy-MM-dd, MM/dd/yyyy, dd-MM-yyyy."
            };
        }

        return new OperationResult { IsSuccess = true, Data = fecha };
    }
}

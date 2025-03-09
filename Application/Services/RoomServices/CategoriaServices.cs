using HRMS.Application.DTOs.RoomManagementDto.CategoriaDTOS;
using HRMS.Application.Interfaces.RoomManagementService;
using HRMS.Domain.Base;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Interfaces.IRoomRepository;
using HRMS.Persistence.Interfaces.IServicioRepository;
using Microsoft.Extensions.Logging;

namespace HRMS.Application.Services.RoomServices
{
    public class CategoriaServices : ICategoryService
    {
        private readonly ILogger<CategoriaServices> _logger;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IHabitacionRepository _habitacionRepository;
        private readonly IServicioRepository _servicioRepository;
        private readonly ITarifaRepository _tarifaRepository;

        public CategoriaServices(ICategoryRepository categoryRepository, ILogger<CategoriaServices> logger, 
                                 IHabitacionRepository habitacionRepository, IServicioRepository servicioRepository, ITarifaRepository tarifaRepository)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
            _habitacionRepository = habitacionRepository;
            _servicioRepository = servicioRepository;
            _tarifaRepository = tarifaRepository;
        }

        public async Task<OperationResult> GetAll()
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                _logger.LogInformation("Buscando todas las categorías.");
                var categories = await _categoryRepository.GetAllAsync();
                return categories.Any() 
                    ? OperationResult.Success(categories, "Categorías obtenidas correctamente.") 
                    : OperationResult.Failure("No se encontraron categorías.");
            });
        }

        public async Task<OperationResult> GetById(int id)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                var  validateId = ValidateId(id, "El ID de la categoría debe ser mayor que 0.");
                if (!validateId.IsSuccess) return validateId;
                _logger.LogInformation("Buscando categoría con ID: {Id}", id);

                var category = await FindCategoryByIdAsync(id);
                return OperationResult.Success(category, "Categoría obtenida correctamente.");
            });
        }

        public async Task<OperationResult> Save(CreateCategoriaDto dto)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                var validateId = ValidateId(dto.IdServicio, "El ID del servicio debe ser mayor que 0.");
                var validateCapacidad = ValidateId(dto.Capacidad, "La capacidad de la categoría debe ser mayor que 0.");
                var validateDescription = ValidateString(dto.Descripcion, "La descripción de la categoría no puede estar vacía.");
            
                if (!validateId.IsSuccess || !validateCapacidad.IsSuccess || !validateDescription.IsSuccess)
                    return OperationResult.Failure("Error al validar los datos de la categoría.");
            
                var servicioValidation = await ValidateServicioExistsAndActiveAsync(dto.IdServicio);
                if (!servicioValidation.IsSuccess)
                    return servicioValidation;
            
                if (await _categoryRepository.ExistsAsync(c => c.Descripcion == dto.Descripcion && c.Estado == true))
                    return OperationResult.Failure($"Ya existe una categoría con la descripción '{dto.Descripcion}'.");
            
                _logger.LogInformation("Creando una nueva categoría.");

                var category = new Categoria
                {
                    Descripcion = dto.Descripcion,
                    Capacidad = dto.Capacidad,
                    IdServicio = dto.IdServicio,
                };

                await _categoryRepository.SaveEntityAsync(category);
                return OperationResult.Success(category, "Categoría creada exitosamente.");
            });
        }

        public async Task<OperationResult> Update(UpdateCategoriaDto dto)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
                {
                    var validateId = ValidateId(dto.IdCategoria, "El ID de la categoría debe ser mayor que 0.");
                    if (!validateId.IsSuccess) return validateId;

                    var validateDescription = ValidateString(dto.Descripcion,
                        "La descripción de la categoría no puede estar vacía.");
                    if (!validateDescription.IsSuccess) return validateDescription;

                    var validateServiceId = ValidateId(dto.IdServicio, "El ID del servicio debe ser mayor que 0.");
                    if (!validateServiceId.IsSuccess) return validateServiceId;

                    var category = await FindCategoryByIdAsync(dto.IdCategoria);
                    if (category == null)
                        return OperationResult.Failure($"No se encontró la categoría con ID {dto.IdCategoria}.");

                    var servicioValidation = await ValidateServicioExistsAndActiveAsync(dto.IdServicio);
                    if (!servicioValidation.IsSuccess)
                        return servicioValidation;

                    if (await _categoryRepository.ExistsAsync(c =>
                            c.Descripcion == dto.Descripcion &&
                            c.IdCategoria != dto.IdCategoria &&
                            c.Estado == true))
                        return OperationResult.Failure(
                            $"Ya existe otra categoría con la descripción '{dto.Descripcion}'.");

                    category.Descripcion = dto.Descripcion;
                    category.Capacidad = dto.Capacidad;
                    category.IdServicio = dto.IdServicio;

                    await _categoryRepository.UpdateEntityAsync(category);
                    return OperationResult.Success(category, "Categoría actualizada correctamente.");
                });
        }

        public async Task<OperationResult> Remove(DeleteCategoriaDto dto)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
                {
                    _logger.LogInformation("Eliminando categoría con ID: {Id}", dto.IdCategoria);

                    var category = await FindCategoryByIdAsync(dto.IdCategoria);
                    var validateId = ValidateId(dto.IdCategoria, "El ID de la categoría debe ser mayor que 0.");
                    if (!validateId.IsSuccess) return validateId;
            
                    if (await TieneHabitacionesAsociadas(dto.IdCategoria))
                    {
                        return OperationResult.Failure("No se puede eliminar la categoría porque tiene habitaciones asociadas.");
                    }
            
                    if (await TieneTarifasAsociadas(dto.IdCategoria))
                    {
                        return OperationResult.Failure("No se puede eliminar la categoría porque tiene tarifas activas asociadas.");
                    }

                    category.Estado = false;
                    await _categoryRepository.UpdateEntityAsync(category);
                    return OperationResult.Success(category, "Categoría eliminada correctamente.");
                });
        }

        public async Task<OperationResult> GetCategoriaByServicio(string nombreServicio)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                var validateService = ValidateString(nombreServicio, "El nombre del servicio no puede estar vacío.");
                if (!validateService.IsSuccess) return validateService;
                _logger.LogInformation("Buscando categoría para el servicio: {ServiceName}", nombreServicio);
                var category = await _categoryRepository.GetCategoriaByServiciosAsync(nombreServicio);

                return category != null
                    ? OperationResult.Success(category, "Categoría encontrada.")
                    : OperationResult.Failure("No se encontró una categoría para el servicio indicado.");
            });
        }

        public async Task<OperationResult> GetCategoriaByDescripcion(string descripcion)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
                {
                    var validateDescription = ValidateString(descripcion, "La descripción de la categoría no puede estar vacía.");
                    if (!validateDescription.IsSuccess) return validateDescription;
                    _logger.LogInformation("Buscando categorías con la descripción: {Description}", descripcion);
                    var result = await _categoryRepository.GetCategoriaByDescripcionAsync(descripcion);
                    
                    return result;
                });
        }

        public async Task<OperationResult> GetHabitacionesByCapacidad(int capacidad)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {

                var validateCapacity = ValidateId(capacidad, "La capacidad de la habitación debe ser mayor que 0.");
                if (!validateCapacity.IsSuccess) return validateCapacity;
                _logger.LogInformation("Buscando habitaciones con capacidad: {Capacity}", capacidad);

                var categories = await _categoryRepository.GetHabitacionByCapacidad(capacidad);
                return categories != null
                    ? OperationResult.Success(categories, "Habitaciones obtenidas correctamente.")
                    : OperationResult.Failure("No se encontraron habitaciones con la capacidad indicada.");
            });
        }
        
        private async Task<bool> TieneHabitacionesAsociadas(int idCategoria)
        {
            try
            {
                return await _habitacionRepository.ExistsAsync(h => 
                    h.IdCategoria == idCategoria && h.Estado == true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al verificar habitaciones asociadas a la categoría {idCategoria}");
                return false; 
            }
        }
        
        
        private async Task<bool> TieneTarifasAsociadas(int idCategoria)
        {
            try
            {
                return await _tarifaRepository.ExistsAsync(t => 
                    t.IdCategoria == idCategoria && t.Estado == true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al verificar tarifas asociadas a la categoría {idCategoria}");
                return false; 
            }
        }

        private async Task<Categoria> FindCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetEntityByIdAsync(id);
            if (category == null) OperationResult.Failure($"No se encontró la categoría por el Id {id}.");
            return category;
        }
        
        private async Task<OperationResult> ValidateServicioExistsAndActiveAsync(short idServicio)
        {
            try
            {
               
                var servicio = await _servicioRepository.GetEntityByIdAsync(idServicio);
        
                if (servicio == null)
                    return OperationResult.Failure($"El servicio con ID {idServicio} no existe.");
            
                if (servicio.Estado != true)
                    return OperationResult.Failure($"El servicio con ID {idServicio} está inactivo.");
            
                return OperationResult.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al verificar el servicio con ID {idServicio}");
                return OperationResult.Failure($"Error al verificar el servicio: {ex.Message}");
            }
        }

        private OperationResult  ValidateId(int value, string message)
        {
            return value <= 0 
                ? OperationResult.Failure(message) 
                : new OperationResult { IsSuccess = true };
        }
        private OperationResult ValidateString(string description, string message)
        {
            return string.IsNullOrWhiteSpace(description) 
                ? OperationResult.Failure(message) 
                : new OperationResult { IsSuccess = true };
        }
        
    }
}
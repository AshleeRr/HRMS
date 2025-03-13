using HRMS.Application.DTOs.RoomManagementDto.CategoriaDTOS;
using HRMS.Application.Interfaces.RoomManagementService;
using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
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
        private readonly IValidator<CreateCategoriaDto> _validator;

        public CategoriaServices(ICategoryRepository categoryRepository, ILogger<CategoriaServices> logger, 
                                 IHabitacionRepository habitacionRepository, IServicioRepository servicioRepository, 
                                 ITarifaRepository tarifaRepository, IValidator<CreateCategoriaDto> validator)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
            _habitacionRepository = habitacionRepository;
            _servicioRepository = servicioRepository;
            _tarifaRepository = tarifaRepository;
            _validator = validator;
        }

        public async Task<OperationResult> GetAll()
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                _logger.LogInformation("Buscando todas las categorías.");
                var categories = await _categoryRepository.GetAllAsync();
                
                if (!categories.Any())
                    return OperationResult.Failure("No se encontraron categorías.");
                
                var categoriesDto = categories.Select(MapToDto).ToList();
                return OperationResult.Success(categoriesDto, "Categorías obtenidas correctamente.");
            });
        }

        public async Task<OperationResult> GetById(int id)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                var validateId = ValidateId(id, "El ID de la categoría debe ser mayor que 0.");
                if (!validateId.IsSuccess) return validateId;
                
                _logger.LogInformation("Buscando categoría con ID: {Id}", id);
                var category = await _categoryRepository.GetEntityByIdAsync(id);
                
                if (category == null)
                    return OperationResult.Failure($"No se encontró la categoría con ID {id}.");
                
                var categoryDto = MapToDto(category);
                return OperationResult.Success(categoryDto, "Categoría obtenida correctamente.");
            });
        }

        public async Task<OperationResult> Save(CreateCategoriaDto dto)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                var validation = _validator.Validate(dto);
                if (!validation.IsSuccess)
                    return OperationResult.Failure(validation.Message);
                
                var servicioValidation = await ValidateServicioExistsAndActiveAsync(dto.IdServicio);
                if (!servicioValidation.IsSuccess)
                    return servicioValidation;
            
                if (await _categoryRepository.ExistsAsync(c => c.Descripcion == dto.Descripcion && c.Estado == true))
                    return OperationResult.Failure($"Ya existe una categoría con la descripción '{dto.Descripcion}'.");
            
                _logger.LogInformation("Creando una nueva categoría.");

                var category = MapToEntity(dto);

                await _categoryRepository.SaveEntityAsync(category);
                var categoryDto = MapToDto(category);
                return OperationResult.Success(categoryDto, "Categoría creada exitosamente.");
            });
        }

        public async Task<OperationResult> Update(UpdateCategoriaDto dto)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                var validateId = ValidateId(dto.IdCategoria, "El ID de la categoría debe ser mayor que 0.");
                if (!validateId.IsSuccess) return validateId;
        
                var validation = _validator.Validate(dto);
                if (!validation.IsSuccess)
                    return OperationResult.Failure(validation.Message);

                var categoria = await _categoryRepository.GetEntityByIdAsync(dto.IdCategoria);
        
                if (categoria == null)
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

                UpdateEntityFromDto(categoria, dto);

                await _categoryRepository.UpdateEntityAsync(categoria);
                var categoryDto = MapToDto(categoria);
                return OperationResult.Success(categoryDto, "Categoría actualizada correctamente.");
            });
        }

        public async Task<OperationResult> Remove(DeleteCategoriaDto dto)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                _logger.LogInformation("Eliminando categoría con ID: {Id}", dto.IdCategoria);
                var validateId = ValidateId(dto.IdCategoria, "El ID de la categoría debe ser mayor que 0.");
                if (!validateId.IsSuccess) return validateId;
                
                var category = await _categoryRepository.GetEntityByIdAsync(dto.IdCategoria);
                if (category == null)
                    return OperationResult.Failure($"No se encontró la categoría con ID {dto.IdCategoria}.");
            
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
                var categoryDto = MapToDto(category);
                return OperationResult.Success(categoryDto, "Categoría eliminada correctamente.");
            });
        }

        public async Task<OperationResult> GetCategoriaByServicio(string nombreServicio)
        {
            return await OperationResult.ExecuteOperationAsync(async () =>
            {
                var validateService = ValidateString(nombreServicio, "El nombre del servicio no puede estar vacío.");
                if (!validateService.IsSuccess) return validateService;
                
                _logger.LogInformation("Buscando categoría para el servicio: {ServiceName}", nombreServicio);
                var result = await _categoryRepository.GetCategoriaByServiciosAsync(nombreServicio);
                
                var categorias = result.Data as List<Categoria>;
                if (categorias == null || !categorias.Any())
                    return OperationResult.Failure("No se encontró una categoría para el servicio indicado.");
                
                var categoriasDto = categorias.Select(MapToDto).ToList();
                return OperationResult.Success(categoriasDto, "Categoría encontrada.");
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
                
                if (result.IsSuccess && result.Data is List<Categoria> categorias && categorias.Any())
                {
                    var categoriasDto = categorias.Select(MapToDto).ToList();
                    return OperationResult.Success(categoriasDto, result.Message);
                }
                
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
                var result = await _categoryRepository.GetHabitacionByCapacidad(capacidad);
                
                if (!result.IsSuccess)
                    return OperationResult.Failure("No se encontraron habitaciones con la capacidad indicada.");
                
                return OperationResult.Success(result.Data, "Habitaciones obtenidas correctamente.");
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

        private static CategoriaDto MapToDto(Categoria categoria)
            => new CategoriaDto()
            {
                IdCategoria = categoria.IdCategoria,
                Descripcion = categoria.Descripcion,
                Capacidad = categoria.Capacidad,
                IdServicio = categoria.IdServicio
            };

        private static Categoria MapToEntity(CreateCategoriaDto dto)
            => new Categoria()
            {
                Descripcion = dto.Descripcion,
                Capacidad = dto.Capacidad,
                IdServicio = dto.IdServicio,
            };

        private static void UpdateEntityFromDto(Categoria entity, UpdateCategoriaDto dto)
        {
            entity.Descripcion = dto.Descripcion;
            entity.Capacidad = dto.Capacidad;
            entity.IdServicio = dto.IdServicio;
        }
        
        private OperationResult ValidateId(int value, string message)
        {
            return value <= 0 
                ? OperationResult.Failure(message) 
                : new OperationResult { IsSuccess = true };
        }
        
        private OperationResult ValidateString(string value, string message)
        {
            return string.IsNullOrWhiteSpace(value) 
                ? OperationResult.Failure(message) 
                : new OperationResult { IsSuccess = true };
        }
    }
}
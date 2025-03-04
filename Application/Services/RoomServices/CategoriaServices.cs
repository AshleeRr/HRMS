using HRMS.Application.DTOs.RoomManagementDto.CategoriaDTOS;
using HRMS.Application.Interfaces.RoomManagementService;
using HRMS.Domain.Base;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.Extensions.Logging;

namespace HRMS.Application.Services.RoomServices
{
    public class CategoriaServices : ICategoryService
    {
        private readonly ILogger<CategoriaServices> _logger;
        private readonly ICategoryRepository _categoryRepository;

        public CategoriaServices(ICategoryRepository categoryRepository, ILogger<CategoriaServices> logger)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<OperationResult> GetAll()
        {
            var result = new OperationResult();
            try
            {
                _logger.LogInformation("Buscando todas las categorias");

                var categorias = await _categoryRepository.GetAllAsync();
                if (categorias.Count == 0)
                {
                    result.IsSuccess = false;
                    result.Message = "Ninguna categoría encontrada";
                    return result;
                }

                result.Data = categorias;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al buscar todas las categorias");
                result.IsSuccess = false;
                result.Message = "Ocurrió un error al buscar las categorias";
            }
            return result;
        }

        public async Task<OperationResult> GetById(int id)
        {
            var result = new OperationResult();
            
            try
            {
                _logger.LogInformation("Buscando categoria por ID: {Id}", id);
                if (id <= 0)
                {
                    _logger.LogWarning("ID de categoria tiene que ser mayor a 0");
                    result.IsSuccess = false;
                    result.Message = "ID de categoria inválido";
                }
                
                var categoria = await _categoryRepository.GetEntityByIdAsync(id);
                
                if (categoria == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Ninguna categoría encontrada";
                    return result;
                }

                result.Data = categoria;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al buscar categoria por ID");
                result.IsSuccess = false;
                result.Message = "Ocurrió un error al buscar la categoria";
            }
            return result;
        }

        public async Task<OperationResult> Update(UpdateCategoriaDto dto)
        {
            var result = new OperationResult();
            try
            {
                _logger.LogInformation("Updating category with ID: {Id}", dto.IdCategoria);

                var categoria = await _categoryRepository.GetEntityByIdAsync(dto.IdCategoria);
                if (categoria == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Category not found";
                    return result;
                }

                categoria.Descripcion = dto.Descripcion;
                await _categoryRepository.UpdateEntityAsync(categoria);

                result.Message = "La categoría se actualizó correctamente";
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error al actualizar la categoria");
                result.IsSuccess = false;
                result.Message = "Ocurrió un error al actualizar la categoria";
            }
            return result;
        }

        public async Task<OperationResult> Save(CreateCategoriaDto dto)
        {
            var result = new OperationResult();
            try
            {
                _logger.LogInformation("Creando una nueva categoria");

                var categoria = new Categoria
                {
                    Descripcion = dto.Descripcion
                };

                await _categoryRepository.SaveEntityAsync(categoria);

                result.Message = "Category created successfully";
                result.Data = categoria;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error creating category");
                result.IsSuccess = false;
                result.Message = "An error occurred while creating the category";
            }
            return result;
        }

        public async Task<OperationResult> Remove(DeleteCategoriaDto dto)
        {
            var result = new OperationResult();
            try
            {
                _logger.LogInformation("Deleting category with ID: {Id}", dto.IdCategoria);

                var categoria = await _categoryRepository.GetEntityByIdAsync(dto.IdCategoria);
                if (categoria == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Category not found";
                    return result;
                }

                categoria.Estado = false;
                await _categoryRepository.UpdateEntityAsync(categoria);

                result.Message = "Category deleted successfully";
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting category");
                result.IsSuccess = false;
                result.Message = "An error occurred while deleting the category";
            }
            return result;
        }

        public async Task<OperationResult> GetCategoriaByServicio(string nombreServicio)
        {
            var result = new OperationResult();
            try
            {
                _logger.LogInformation("Fetching category for service: {ServiceName}", nombreServicio);

                var categoria = await _categoryRepository.GetCategoriaByServiciosAsync(nombreServicio);
                if (categoria == null)
                {
                    result.IsSuccess = false;
                    result.Message = "Category not found for the given service";
                    return result;
                }

                result.Data = categoria;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error fetching category by service");
                result.IsSuccess = false;
                result.Message = "An error occurred while retrieving the category";
            }
            return result;
        }

        public async Task<OperationResult> GetServiciosByDescripcion(string descripcion)
        {
            var result = new OperationResult();
            try
            {
                _logger.LogInformation("Fetching services for description: {Description}", descripcion);

                var servicios = await _categoryRepository.GetServiciosByDescripcionAsync(descripcion);
                if (servicios == null)
                {
                    result.IsSuccess = false;
                    result.Message = "No services found for the given description";
                    return result;
                }

                result.Data = servicios;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error fetching services by description");
                result.IsSuccess = false;
                result.Message = "An error occurred while retrieving services";
            }
            return result;
        }

        public async Task<OperationResult> GetHabitacionesByCapacidad(int capacidad)
        {
            var result = new OperationResult();
            try
            {
                _logger.LogInformation("Fetching rooms with capacity: {Capacity}", capacidad);

                var habitaciones = await _categoryRepository.GetHabitacionByCapacidad(capacidad);
                if (habitaciones == null)
                {
                    result.IsSuccess = false;
                    result.Message = "No rooms found for the given capacity";
                    return result;
                }

                result.Data = habitaciones;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error fetching rooms by capacity");
                result.IsSuccess = false;
                result.Message = "An error occurred while retrieving rooms";
            }
            return result;
        }
    }
}

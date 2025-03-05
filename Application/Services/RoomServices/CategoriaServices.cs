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
            _logger.LogInformation("Buscando todas las categorías.");
            return await ExecuteOperationAsync(async () =>
            {
                var categories = await _categoryRepository.GetAllAsync();
                return categories.Any()
                    ? new OperationResult { Data = categories }
                    : new OperationResult { IsSuccess = false, Message = "No se encontraron categorías." };
            }, "Error al buscar todas las categorías.");
        }

        public async Task<OperationResult> GetById(int id)
        {
            _logger.LogInformation("Buscando categoría por ID: {Id}", id);
            ValidateId(id, "El ID de la categoría debe ser mayor que 0.");
            return await ExecuteOperationAsync(async () =>
            {
                var category = await _categoryRepository.GetEntityByIdAsync(id);
                return category != null
                    ? new OperationResult { Data = category }
                    : new OperationResult { IsSuccess = false, Message = "Categoría no encontrada." };
            }, "Error al buscar la categoría por ID.");
        }

        public async Task<OperationResult> Save(CreateCategoriaDto dto)
        {
            _logger.LogInformation("Creando una nueva categoría.");
            ValidateId(dto.IdServicio, "El ID del servicio debe ser mayor que 0.");

            var category = new Categoria 
            { 
                Descripcion = dto.Descripcion, 
                Capacidad = dto.Capacidad, 
                IdServicio = dto.IdServicio 
            };

            return await ExecuteOperationAsync(async () =>
            {
                await _categoryRepository.SaveEntityAsync(category);
                return new OperationResult { Data = category, Message = "Categoría creada exitosamente." };
            }, "Error al crear la categoría.");
        }

        public async Task<OperationResult> Update(UpdateCategoriaDto dto)
        {
            _logger.LogInformation("Actualizando la categoría con ID: {Id}", dto.IdCategoria);
            return await ExecuteOperationAsync(async () =>
            {
                var category = await FindCategoryByIdAsync(dto.IdCategoria);
                category.Descripcion = dto.Descripcion;

                await _categoryRepository.UpdateEntityAsync(category);
                return new OperationResult { Message = "Categoría actualizada correctamente." };
            }, "Error al actualizar la categoría.");
        }

        public async Task<OperationResult> Remove(DeleteCategoriaDto dto)
        {
            _logger.LogInformation("Eliminando la categoría con ID: {Id}", dto.IdCategoria);
            return await ExecuteOperationAsync(async () =>
            {
                var category = await FindCategoryByIdAsync(dto.IdCategoria);
                category.Estado = false;

                await _categoryRepository.UpdateEntityAsync(category);
                return new OperationResult { Message = "Categoría eliminada correctamente." };
            }, "Error al eliminar la categoría.");
        }

        public async Task<OperationResult> GetCategoriaByServicio(string nombreServicio)
        {
            _logger.LogInformation("Buscando categoría para el servicio: {ServiceName}", nombreServicio);
            return await ExecuteOperationAsync(async () =>
            {
                var category = await _categoryRepository.GetCategoriaByServiciosAsync(nombreServicio);
                return category != null
                    ? new OperationResult { Data = category }
                    : new OperationResult { IsSuccess = false, Message = "No se encontró una categoría para el servicio indicado." };
            }, "Error al buscar la categoría por servicio.");
        }

        public async Task<OperationResult> GetServiciosByDescripcion(string descripcion)
        {
            try
            {
                _logger.LogInformation("Buscando servicios con la descripción: {Description}", descripcion);
                var operationResult = await _categoryRepository.GetServiciosByDescripcionAsync(descripcion);
                return operationResult.IsSuccess
                    ? new OperationResult { Data = operationResult.Data }
                    : new OperationResult
                        { IsSuccess = false, Message = "No se encontraron servicios con esa descripción." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar servicios por descripción.");
                return new OperationResult { IsSuccess = false, Message = "Error al buscar servicios por descripción." };
            }
        }
        

        public async Task<OperationResult> GetHabitacionesByCapacidad(int capacidad)
        {
            _logger.LogInformation("Buscando habitaciones con capacidad: {Capacity}", capacidad);
            return await ExecuteOperationAsync(async () =>
            {
                ValidateId(capacidad, "La capacidad de la habitación debe ser mayor que 0.");
                var categories = await _categoryRepository.GetHabitacionByCapacidad(capacidad);
                return categories != null
                    ? new OperationResult { Data = categories }
                    : new OperationResult { IsSuccess = false, Message = "No se encontraron habitaciones con la capacidad indicada." };
            }, "Error al buscar habitaciones por capacidad.");
        }


        private async Task<OperationResult> ExecuteOperationAsync(Func<Task<OperationResult>> operation, string errorMessage)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, errorMessage);
                return new OperationResult { IsSuccess = false, Message = errorMessage };
            }
        }


        private void ValidateId(int value, string message)
        {
            if (value <= 0) throw new ArgumentException(message);
        }


        private async Task<Categoria> FindCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetEntityByIdAsync(id);
            if (category == null) throw new KeyNotFoundException("Categoría no encontrada.");
            return category;
        }
    }
}

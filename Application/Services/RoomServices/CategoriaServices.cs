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
        private readonly IHabitacionRepository _habitacionRepository;

        public CategoriaServices(ICategoryRepository categoryRepository, ILogger<CategoriaServices> logger, 
                                 IHabitacionRepository habitacionRepository)
        {
            _categoryRepository = categoryRepository;
            _logger = logger;
            _habitacionRepository = habitacionRepository;
        }

        public async Task<OperationResult> GetAll()
        {
            return await ExecuteOperationAsync(async () =>
            {
                _logger.LogInformation("Buscando todas las categorías.");
                var categories = await _categoryRepository.GetAllAsync();
                return categories.Any() 
                    ? Success(categories, "Categorías obtenidas correctamente.") 
                    : Failure("No se encontraron categorías.");
            }, "Error al obtener todas las categorías.");
        }

        public async Task<OperationResult> GetById(int id)
        {
            return await ExecuteOperationAsync(async () =>
            {
                ValidateId(id, "El ID de la categoría debe ser mayor que 0.");
                _logger.LogInformation("Buscando categoría con ID: {Id}", id);

                var category = await FindCategoryByIdAsync(id);
                return Success(category, "Categoría obtenida correctamente.");
            }, $"Error al obtener la categoría con ID {id}.");
        }

        public async Task<OperationResult> Save(CreateCategoriaDto dto)
        {
            return await ExecuteOperationAsync(async () =>
            {
                ValidateId(dto.IdServicio, "El ID del servicio debe ser mayor que 0.");
                _logger.LogInformation("Creando una nueva categoría.");

                var category = new Categoria
                {
                    Descripcion = dto.Descripcion,
                    Capacidad = dto.Capacidad,
                    IdServicio = dto.IdServicio
                };

                await _categoryRepository.SaveEntityAsync(category);
                return Success(category, "Categoría creada exitosamente.");
            }, "Error al crear la categoría.");
        }

        public async Task<OperationResult> Update(UpdateCategoriaDto dto)
        {
            return await ExecuteOperationAsync(async () =>
            {
                _logger.LogInformation("Actualizando categoría con ID: {Id}", dto.IdCategoria);

                var category = await FindCategoryByIdAsync(dto.IdCategoria);
                category.Descripcion = dto.Descripcion;

                await _categoryRepository.UpdateEntityAsync(category);
                return Success(category, "Categoría actualizada correctamente.");
            }, $"Error al actualizar la categoría con ID {dto.IdCategoria}.");
        }

        public async Task<OperationResult> Remove(DeleteCategoriaDto dto)
        {
            return await ExecuteOperationAsync(async () =>
            {
                _logger.LogInformation("Eliminando categoría con ID: {Id}", dto.IdCategoria);
                
                var category = await FindCategoryByIdAsync(dto.IdCategoria);
                if (await TieneHabitacionesAsociadas(category.Descripcion))
                {
                    return Failure("No se puede eliminar la categoría porque tiene habitaciones asociadas.");
                }

                category.Estado = false;
                await _categoryRepository.UpdateEntityAsync(category);
                return Success(category, "Categoría eliminada correctamente.");
            }, $"Error al eliminar la categoría con ID {dto.IdCategoria}.");
        }

        public async Task<OperationResult> GetCategoriaByServicio(string nombreServicio)
        {
            return await ExecuteOperationAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(nombreServicio))
                    return Failure("El nombre del servicio no puede estar vacío.");

                _logger.LogInformation("Buscando categoría para el servicio: {ServiceName}", nombreServicio);
                var category = await _categoryRepository.GetCategoriaByServiciosAsync(nombreServicio);

                return category != null
                    ? Success(category, "Categoría encontrada.")
                    : Failure("No se encontró una categoría para el servicio indicado.");
            }, $"Error al obtener la categoría por servicio '{nombreServicio}'.");
        }

        public async Task<OperationResult> GetServiciosByDescripcion(string descripcion)
        {
            return await ExecuteOperationAsync(async () =>
            {
                _logger.LogInformation("Buscando servicios con la descripción: {Description}", descripcion);
                var result = await _categoryRepository.GetServiciosByDescripcionAsync(descripcion);

                return result.IsSuccess && result.Data != null
                    ? Success(result.Data, "Servicios obtenidos correctamente.")
                    : Failure("No se encontraron servicios con esa descripción.");
            }, "Error al buscar servicios por descripción.");
        }

        public async Task<OperationResult> GetHabitacionesByCapacidad(int capacidad)
        {
            return await ExecuteOperationAsync(async () =>
            {
                ValidateId(capacidad, "La capacidad de la habitación debe ser mayor que 0.");
                _logger.LogInformation("Buscando habitaciones con capacidad: {Capacity}", capacidad);

                var categories = await _categoryRepository.GetHabitacionByCapacidad(capacidad);
                return categories != null
                    ? Success(categories, "Habitaciones obtenidas correctamente.")
                    : Failure("No se encontraron habitaciones con la capacidad indicada.");
            }, $"Error al obtener habitaciones con capacidad {capacidad}.");
        }
        
        private async Task<bool> TieneHabitacionesAsociadas(string descripcionCategoria)
        {
            var habitacionesResult = await _habitacionRepository.GetByCategoriaAsync(descripcionCategoria);
            if (!habitacionesResult.IsSuccess || habitacionesResult.Data == null) return false;

            var habitaciones = habitacionesResult.Data as IEnumerable<Habitacion>;
            return habitaciones != null && habitaciones.Any();
        }

        private async Task<Categoria> FindCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetEntityByIdAsync(id);
            if (category == null) throw new KeyNotFoundException($"No se encontró la categoría con ID {id}.");
            return category;
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
                return Failure(errorMessage);
            }
        }

        private void ValidateId(int value, string message)
        {
            if (value <= 0) throw new ArgumentException(message);
        }

        private static OperationResult Success(object data = null, string message = null) =>
            new() { IsSuccess = true, Data = data, Message = message };

        private static OperationResult Failure(string message) =>
            new() { IsSuccess = false, Message = message };
    }
}

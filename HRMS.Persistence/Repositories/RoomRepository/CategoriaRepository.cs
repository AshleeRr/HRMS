using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HRMS.Persistence.Repositories.RoomRepository
{
    public class CategoriaRepository : BaseRepository<Categoria, int>, ICategoryRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IValidator<Categoria> _validator;
        private readonly ILogger<CategoriaRepository> _logger;

        public CategoriaRepository(HRMSContext context, IConfiguration configuration, IValidator<Categoria> validator, ILogger<CategoriaRepository> logger) 
            : base(context)
        {
            _configuration = configuration;
            _validator = validator;
            _logger = logger;
        }

        public override async Task<List<Categoria>> GetAllAsync()
        {
            _logger.LogInformation("Obteniendo todas las categorías activas");
            return await _context.Categorias
                .Where(c => c.Estado == true)
                .ToListAsync();
        }
        
        public override async Task<Categoria> GetEntityByIdAsync(int id)
        {
            _logger.LogInformation($"Obteniendo categoría por id {id}");
            
            return (id == 0 ? null : await _context.Categorias
                .FirstOrDefaultAsync(c => c.IdCategoria == id && c.Estado == true))!;
        }
        
        public override async Task<OperationResult> SaveEntityAsync(Categoria categoria) =>
            await OperationResult.ExecuteOperationAsync(async () =>
            {
                _logger.LogInformation("Guardando nueva categoría");
                
                var validationResult = _validator.Validate(categoria);
                if (!validationResult.IsSuccess)
                {
                    _logger.LogWarning("Error de validación al crear categorias: {Error}", validationResult.Message);
                    return OperationResult.Failure(validationResult.Message);
                }
                
                await _context.Categorias.AddAsync(categoria);
                await _context.SaveChangesAsync();
                return OperationResult.Success(categoria, "Categoría guardada correctamente.");
            });
        
        public override async Task<OperationResult> UpdateEntityAsync(Categoria categoria) =>
            await OperationResult.ExecuteOperationAsync(async () =>
            {
                _logger.LogInformation($"Actualizando categoría con id {categoria.IdCategoria}");
                
            
                var validationResult = _validator.Validate(categoria);
                if (!validationResult.IsSuccess)
                {
                    _logger.LogWarning("Error de validación al actualizar categorias: {Error}", validationResult.Message);
                    return OperationResult.Failure(validationResult.Message);
                }
                
                var existingCategoria = await _context.Categorias.FindAsync(categoria.IdCategoria);
                if (existingCategoria == null) return OperationResult.Failure("La categoría no existe.");
                UpdateCategoria(existingCategoria, categoria);
                await _context.SaveChangesAsync();
                return OperationResult.Success(existingCategoria);
            });
        
        public async Task<OperationResult> GetCategoriaByServiciosAsync(string nombre) =>
            await OperationResult.ExecuteOperationAsync(async () =>
            {
                var validation = await ValidateString(nombre, "El nombre del servicio no puede ser nulo o vacío.");
                if (!validation.IsSuccess)
                    return validation;
                _logger.LogInformation($"Obteniendo categorías por servicio '{nombre}'");
                
                var categoriasYServicios = await _context.Categorias
                    .Join(_context.Servicios,
                        c => c.IdServicio,
                        s => s.IdServicio,
                        (c, s) => new { Categoria = c, Servicio = s })
                    .Where(cs => cs.Categoria.Estado == true && cs.Servicio.Estado == true)
                    .ToListAsync();
 
                var categorias = categoriasYServicios
                    .Where(cs => cs.Servicio.Nombre.Contains(nombre, StringComparison.OrdinalIgnoreCase))
                    .Select(cs => cs.Categoria)
                    .ToList();
 
                return OperationResult.Success(categorias, 
                    categorias.Any() ? null : $"No se encontraron categorías para el servicio '{nombre}'.");
            });        
        public async Task<OperationResult> GetCategoriaByDescripcionAsync(string descripcion) =>
            await OperationResult.ExecuteOperationAsync(async () =>
            {
                var validation = await ValidateString(descripcion, "La descripción no puede ser nula o vacía.");
                if (!validation.IsSuccess)
                    return validation;
                
                _logger.LogInformation($"Obteniendo categorías por descripción '{descripcion}'");
                
                var categorias = await _context.Categorias
                    .Where(c => c.Descripcion != null && 
                                EF.Functions.Like(c.Descripcion, $"%{descripcion}%") && 
                                c.Estado == true)
                    .ToListAsync();

                return categorias.Any() 
                    ? OperationResult.Success(categorias) 
                    : OperationResult.Failure($"No se encontraron categorias con esa descripción: '{descripcion}'");
            });
        
        public async Task<OperationResult> GetHabitacionByCapacidad(int capacidad) =>
            await OperationResult.ExecuteOperationAsync(async () =>
            {
                
                var validation = await ValidateInt(capacidad, "La capacidad debe ser mayor a 0.");
                if (!validation.IsSuccess)
                    return validation;
                
                _logger.LogInformation("Obteniendo habitaciones con capacidad para {Capacidad} personas", capacidad);
                
                var categoriasIds = await _context.Categorias
                    .Where(c => c.Capacidad == capacidad && c.Estado == true)
                    .Select(c => c.IdCategoria)
                    .ToListAsync();

                if (!categoriasIds.Any())
                    return OperationResult.Failure($"No se encontraron categorías con capacidad para {capacidad} personas.");

                var habitaciones = await _context.Habitaciones
                    .Where(h => h.IdCategoria.HasValue && categoriasIds.Contains(h.IdCategoria.Value))
                    .ToListAsync();

                if (!habitaciones.Any())
                    return OperationResult.Failure($"Se encontraron categorías con capacidad para {capacidad} personas, pero no tienen habitaciones asociadas.");

                return OperationResult.Success(habitaciones, "Habitaciones obtenidas correctamente.");
            });
        
        private async Task<OperationResult> ValidateInt(int id , string message)
        {
            if (id <= 0)
                return OperationResult.Failure(message);
            return OperationResult.Success();
        }
        
        private async Task<OperationResult> ValidateString(string message , string error)
        {
            if (string.IsNullOrEmpty(message))
                return OperationResult.Failure(error);
            return OperationResult.Success();
        }
        
        private static void UpdateCategoria(Categoria existing, Categoria updated)
        {
            existing.Descripcion = updated.Descripcion;
            existing.IdServicio = updated.IdServicio;
            existing.Capacidad = updated.Capacidad;
        }
    }
}

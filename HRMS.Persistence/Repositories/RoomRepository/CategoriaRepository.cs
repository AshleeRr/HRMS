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
        private readonly ILogger<CategoriaRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly IValidator<Categoria> _validator;

        public CategoriaRepository(HRMSContext context, ILogger<CategoriaRepository> logger,
            IConfiguration configuration, IValidator<Categoria> validator) : base(context)
        {
            _logger = logger;
            _configuration = configuration;
            _validator = validator;
        }

        public override async Task<List<Categoria>> GetAllAsync()
        {
            return await _context.Categorias.Where(c => c.Estado == true).ToListAsync();
        }

        public override async Task<OperationResult> SaveEntityAsync(Categoria categoria)
        {
            var validation = _validator.Validate(categoria);
            if (!validation.IsSuccess) return validation;

            try
            {
                if (await ExistsAsync(c => c.Descripcion == categoria.Descripcion))
                {
                    return new OperationResult
                    {
                        IsSuccess = false,
                        Message = $"Ya existe una categoría con la descripción '{categoria.Descripcion}'."
                    };
                }

                await _context.Categorias.AddAsync(categoria);
                await _context.SaveChangesAsync();

                return new OperationResult
                {
                    IsSuccess = true,
                    Message = "Categoría guardada correctamente.",
                    Data = categoria
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error guardando la categoría.");
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = $"Ocurrió un error guardando la categoría: {ex.Message}"
                };
            }
        }

        public override async Task<OperationResult> UpdateEntityAsync(Categoria categoria)
        {
            var validation = _validator.Validate(categoria);
            if (!validation.IsSuccess) return validation;

            try
            {
                var existingCategoria = await _context.Categorias.FindAsync(categoria.IdCategoria);
                if (existingCategoria == null)
                {
                    return new OperationResult
                    {
                        IsSuccess = false,
                        Message = "La categoría no existe."
                    };
                }

                if (await _context.Categorias.AnyAsync(c => c.Descripcion == categoria.Descripcion 
                                                            && c.IdCategoria != categoria.IdCategoria))
                {
                    return new OperationResult
                    {
                        IsSuccess = false,
                        Message = $"Ya existe otra categoría con la descripción '{categoria.Descripcion}'."
                    };
                }

                UpdateCategoria(existingCategoria, categoria);
                await _context.SaveChangesAsync();

                return new OperationResult
                {
                    IsSuccess = true,
                    Message = "Categoría actualizada correctamente.",
                    Data = existingCategoria
                };
            }
            catch (DbUpdateConcurrencyException)
            {
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = "La categoría fue modificada por otro usuario. Intente nuevamente."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando la categoría.");
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = $"Ocurrió un error actualizando la categoría: {ex.Message}"
                };
            }
        }

        public async Task<OperationResult> GetCategoriaByServiciosAsync(string nombre)
        {
            return await GetByFilterAsync(
                "El nombre del servicio no puede estar vacío.",
                nombre,
                from c in _context.Categorias
                join s in _context.Servicios on c.IdServicio equals s.IdSercicio
                where s.Descripcion.Contains(nombre)
                select c,
                $"No se encontraron categorías para el servicio '{nombre}'."
            );
        }

        public async Task<OperationResult> GetServiciosByDescripcionAsync(string descripcion)
        {
            return await GetByFilterAsync(
                "La descripción del servicio no puede estar vacía.",
                descripcion,
                from c in _context.Categorias
                join s in _context.Servicios on c.IdServicio equals s.IdSercicio
                where s.Descripcion.Contains(descripcion)
                select s,
                "No se encontraron servicios con esa descripción."
            );
        }

        public async Task<OperationResult> GetHabitacionByCapacidad(int capacidad)
        {
            if (capacidad <= 0)
                return new OperationResult { IsSuccess = false, Message = "La capacidad debe ser mayor que cero." };

            return await GetByFilterAsync(
                null,
                null,
                from c in _context.Categorias
                join h in _context.Habitaciones on c.IdCategoria equals h.IdCategoria
                where c.Capacidad == capacidad && h.Estado == true
                select h,
                $"No se encontraron habitaciones con capacidad para {capacidad} personas."
            );
        }

        
        private void UpdateCategoria(Categoria existing, Categoria updated)
        {
            existing.Descripcion = updated.Descripcion;
            existing.IdServicio = updated.IdServicio;
            existing.Capacidad = updated.Capacidad;
            existing.Estado = updated.Estado;
        }

        private async Task<OperationResult> GetByFilterAsync<T>(
            string? validationMessage, string? filterValue,
            IQueryable<T> query, string notFoundMessage)
        {
            if (!string.IsNullOrWhiteSpace(validationMessage) && string.IsNullOrWhiteSpace(filterValue))
            {
                return new OperationResult { IsSuccess = false, Message = validationMessage };
            }

            try
            {
                var resultList = await query.ToListAsync();
                return new OperationResult
                {
                    IsSuccess = true,
                    Message = resultList.Any() ? null : notFoundMessage,
                    Data = resultList
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo datos.");
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = $"Ocurrió un error obteniendo datos: {ex.Message}"
                };
            }
        }
    }
}

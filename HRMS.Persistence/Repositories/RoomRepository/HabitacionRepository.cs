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
    public class HabitacionRepository : BaseRepository<Habitacion, int>, IHabitacionRepository
    {
        private readonly ILogger<HabitacionRepository> _logger;
        private readonly IConfiguration _configuration;
        private readonly IValidator<Habitacion> _validator;

        public HabitacionRepository(HRMSContext context, ILogger<HabitacionRepository> logger,
            IConfiguration configuration, IValidator<Habitacion> validator) : base(context)
        {
            _logger = logger;
            _configuration = configuration;
            _validator = validator;
        }

        public override async Task<List<Habitacion>> GetAllAsync()
        {
            return await _context.Habitaciones.Where(h => h.Estado == true).ToListAsync();
        }

        public override async Task<Habitacion?> GetEntityByIdAsync(int id)
        {
            return id != 0 ? await _context.Set<Habitacion>().FindAsync(id) : null;
        }

        public async Task<OperationResult> GetByPisoAsync(int idPiso)
        {
            if(idPiso <= 0)
            {
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = "El ID del piso debe ser mayor que cero."
                };
            }
            var habitaciones = await _context.Habitaciones
                .Where(h => h.IdPiso == idPiso)
                .ToListAsync(); 
            return await GetByFilterAsync(
                null,
                null,
                habitaciones.AsQueryable(), 
                $"No se encontraron habitaciones en el piso {idPiso}."
            );
        }


        public async Task<OperationResult> GetByCategoriaAsync(string categoria)
        {
            var habitaciones = await _context.Habitaciones
                .Join(_context.Categorias, h => h.IdCategoria, c => c.IdCategoria, (h, c) => new
                {
                    h, c
                })
                .Where(hc => hc.c.Descripcion.Contains(categoria))
                .Select(hc => hc.h)
                .ToListAsync();
            return await GetByFilterAsync(
                "Tiene que introducir una categoria", categoria, habitaciones.AsQueryable(),
                " No se encontraron habitaciones con la categoría especificada.");
        }


        public async Task<OperationResult> GetInfoHabitacionesAsync()
        {
            var result = new OperationResult();
            try
            {
                var query = from h in _context.Habitaciones
                    join p in _context.Pisos on h.IdPiso equals p.IdPiso
                    join c in _context.Categorias on h.IdCategoria equals c.IdCategoria
                    join t in _context.Tarifas on h.IdCategoria equals t.IdCategoria
                    join s in _context.Servicios on c.IdServicio equals s.IdSercicio 
                    where h.Estado == true && (t.FechaInicio <= DateTime.Now && t.FechaFin >= DateTime.Now)
                    select new 
                    {
                        h.IdHabitacion,
                        h.Numero, 
                        h.Detalle, 
                        h.Estado,
                        t.PrecioPorNoche,
                        DescripcionPiso = p.Descripcion,
                        DescripcionCategoria = c.Descripcion,
                        NombreServicio = s.Nombre,
                        DescripcionServicio = s.Descripcion
                    };


                var habitaciones = await query.ToListAsync();
                result.Data = habitaciones;
                result.IsSuccess = true;
                return result;
            }
            catch (Exception e)
            {
                result.IsSuccess = false;
                result.Message = "Ocurrió un error obteniendo la información de las habitaciones.";
                return result;
            }
        }

        public async Task<OperationResult> GetByNumeroAsync(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
            {
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = "El número de habitación no puede estar vacío."
                };
            }

            try
            {
                var habitacion = await _context.Habitaciones.FirstOrDefaultAsync(h => h.Numero == numero);
                return new OperationResult
                {
                    IsSuccess = habitacion != null,
                    Message = habitacion != null ? null : $"No se encontró la habitación con número '{numero}'.",
                    Data = habitacion
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo la habitación por número.");
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = $"Error obteniendo la habitación por número: {ex.Message}"
                };
            }
        }

        public override async Task<OperationResult> SaveEntityAsync(Habitacion habitacion)
        {
            var result = ValidateHabitacion(habitacion);
            if (!result.IsSuccess) return result;

            try
            {
                if (await ExistsAsync(h => h.Numero == habitacion.Numero))
                {
                    return new OperationResult
                    {
                        IsSuccess = false,
                        Message = $"Ya existe una habitación con el número '{habitacion.Numero}'."
                    };
                }

                await _context.Habitaciones.AddAsync(habitacion);
                await _context.SaveChangesAsync();

                return new OperationResult
                {
                    IsSuccess = true,
                    Message = "Habitación guardada exitosamente.",
                    Data = habitacion
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al guardar la habitación.");
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = $"Error al guardar la habitación: {ex.Message}"
                };
            }
        }

        public override async Task<OperationResult> UpdateEntityAsync(Habitacion habitacion)
        {
            var result = ValidateHabitacion(habitacion);
            if (!result.IsSuccess) return result;

            try
            {
                var existingRoom = await _context.Habitaciones.FindAsync(habitacion.IdHabitacion);
                if (existingRoom == null)
                {
                    return new OperationResult
                    {
                        IsSuccess = false,
                        Message = "La habitación no existe."
                    };
                }

                if (habitacion.Numero != existingRoom.Numero)
                {
                    if (await _context.Habitaciones.AnyAsync(h => h.Numero == habitacion.Numero && h.IdHabitacion != habitacion.IdHabitacion))
                    {
                        return new OperationResult
                        {
                            IsSuccess = false,
                            Message = $"Ya existe otra habitación con el número '{habitacion.Numero}'."
                        };
                    }
                }

                UpdateHabitacion(existingRoom, habitacion);
                await _context.SaveChangesAsync();

                return new OperationResult
                {
                    IsSuccess = true,
                    Message = "Habitación actualizada correctamente.",
                    Data = existingRoom
                };
            }
            catch (DbUpdateConcurrencyException)
            {
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = "La habitación ha sido modificada por otro usuario. Por favor, vuelva a intentarlo."
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la habitación.");
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = $"Error al actualizar la habitación: {ex.Message}"
                };
            }
        }
        
        
        private OperationResult ValidateHabitacion(Habitacion habitacion)
        {
            var validation = _validator.Validate(habitacion);
            return validation.IsSuccess ? new OperationResult { IsSuccess = true } : validation;
        }

        private void UpdateHabitacion(Habitacion existing, Habitacion updated)
        {
            existing.Numero = updated.Numero;
            existing.Detalle = updated.Detalle;
            existing.Precio = updated.Precio;
            existing.IdPiso = updated.IdPiso;
            existing.IdCategoria = updated.IdCategoria;
            existing.IdEstadoHabitacion = updated.IdEstadoHabitacion;
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
                    IsSuccess = resultList.Any(),
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

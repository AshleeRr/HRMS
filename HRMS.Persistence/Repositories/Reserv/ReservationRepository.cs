using HRMS.Domain.Base;
using HRMS.Domain.Entities.Reservation;
using HRMS.Models.Models.ReservationModels;
using HRMS.Domain.Repository;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Linq.Expressions;


namespace HRMS.Persistence.Repositories.Reserv
{
    public class ReservationRepository : BaseRepository<Reservation, int>, IReservationRepository
    {
        private ILogger<ReservationRepository> _logger;
        private IConfiguration _configuration;
        
        public ReservationRepository(HRMSContext context, ILogger<ReservationRepository> logger, IConfiguration configuration) : base(context)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public override async Task<bool> ExistsAsync(Expression<Func<Reservation, bool>> filter)
        {
            if (filter != null)
            {
                return await base.ExistsAsync(filter);
            }
            return false;
        }

        public override async Task<OperationResult> GetAllAsync(Expression<Func<Reservation, bool>> filter)
        {
            if(filter != null)
            {
                return await base.GetAllAsync(filter);
            }
            var res = new OperationResult();
            res.IsSuccess = false;
            res.Message = "No se ha especificado un filtro para la consulta.";
            return res;
        }


        public override async Task<OperationResult> SaveEntityAsync(Reservation entity)
        {
            OperationResult result;
            var validRes = await _validReservationForSaving(entity);
            if (!validRes.IsSuccess)
            {
                return validRes;
            }
            else
            {
                result = await base.SaveEntityAsync(entity);
                if (!result.IsSuccess)
                {
                    string? message = _getErrorMessage();
                    _logger.LogError(message);
                    result.Message = message;
                }

            }
            return result;
        }

        public override async Task<List<Reservation>> GetAllAsync()
            =>  await _context.Reservations.Where(r => r.Estado.Value).ToListAsync();

        /*
        private async Task<bool> _validReservationForSaving(Reservation resev)
            => (resev != null &&
               resev.idCliente != 0 &&
               await _isRoomDisponible(resev.idHabitacion, resev.FechaEntrada.Value, resev.FechaSalida.Value) &&
               resev.idCliente != null &&
               resev.FechaEntrada > DateTime.Now &&
               resev.FechaSalida > resev.FechaEntrada);

        private async Task<bool> _validReservationForUpdating(Reservation resev)
            => (resev != null &&
               resev.idCliente != 0 &&
               resev.idCliente != null &&
               resev.FechaEntrada > DateTime.Now &&
               resev.FechaSalida > resev.FechaEntrada
                && (resev.Estado.Value)
                );
        */

        private async Task<OperationResult> _validReservationForSaving(Reservation resev)
        {
            OperationResult operationResult = new OperationResult();
            List<string> errors = new List<string>();

            if (resev == null)
            {
                errors.Add("La reserva no puede ser nula");
            }
            else
            {
                if (resev.idCliente == 0)
                {
                    errors.Add("El ID del cliente no puede ser cero");
                }
                if (resev.idHabitacion == 0)
                {
                    errors.Add("El ID de la habitación no puede ser cero");
                }
                if (!resev.FechaEntrada.HasValue)
                {
                    errors.Add("La fecha de entrada no puede ser nula");
                }
                if (!resev.FechaSalida.HasValue)
                {
                    errors.Add("La fecha de salida no puede ser nula");
                }
                if (resev.FechaEntrada <= DateTime.Now)
                {
                    errors.Add("La fecha de entrada debe ser posterior a la fecha actual");
                }
                if (resev.FechaSalida <= resev.FechaEntrada)
                {
                    errors.Add("La fecha de salida debe ser posterior a la fecha de entrada");
                }
                
                if (!await _isRoomDisponible(resev.idHabitacion, resev.FechaEntrada.Value, resev.FechaSalida.Value))
                {
                    errors.Add("La habitación no está disponible en las fechas seleccionadas");
                }
            }

            if (errors.Count > 0)
            {
                operationResult.IsSuccess = false;
                operationResult.Message = string.Join(Environment.NewLine, errors);
            }
            return operationResult;
        }

        private async Task<OperationResult> _validReservationForUpdating(Reservation resev)
        {
            OperationResult operationResult = new OperationResult();
            List<string> errors = new List<string>();

            if (resev == null)
            {
                errors.Add("La reserva no puede ser nula");
            }
            else
            {
                if (resev.idCliente == 0)
                {
                    errors.Add("El ID del cliente no puede ser cero");
                }
                if (!resev.FechaEntrada.HasValue)
                {
                    errors.Add("La fecha de entrada no puede ser nula");
                }
                if (!resev.FechaSalida.HasValue)
                {
                    errors.Add("La fecha de salida no puede ser nula");
                }
                if (resev.FechaEntrada <= DateTime.Now)
                {
                    errors.Add("La fecha de entrada debe ser posterior a la fecha actual");
                }
                if (resev.FechaSalida <= resev.FechaEntrada)
                {
                    errors.Add("La fecha de salida debe ser posterior a la fecha de entrada");
                }
                if (!resev.Estado.HasValue || !resev.Estado.Value)
                {
                    errors.Add("El estado de la reserva no es válido");
                }
            }

            if (errors.Count > 0)
            {
                operationResult.IsSuccess = false;
                operationResult.Message = string.Join(Environment.NewLine, errors);
            }
            return operationResult;
        }
        public override async Task<OperationResult> UpdateEntityAsync(Reservation entity)
        {
            OperationResult result;
            var validRes = await _validReservationForUpdating(entity);
            if (!validRes.IsSuccess)
            {
                return validRes;
            }
            else
            {
                result = await base.UpdateEntityAsync(entity);
                if (!result.IsSuccess)
                {
                    string? message = _getErrorMessage();
                    _logger.LogError(message);
                    result.Message = message;
                }
            
            }
            return result;
        }

        public override async Task<Reservation> GetEntityByIdAsync(int id)
        {
            if (id != 0)
            {
                return await base.GetEntityByIdAsync(id);
            }
            return null;
        }

        public async Task<OperationResult> GetReservationsByClientId(int clientId)
        {
            OperationResult result = new OperationResult();
            if (clientId == 0)
            {
                result.IsSuccess = false;
                result.Message = "No se ha especificado un cliente.";
            }
            else
            {
                try
                {
                    var query = from r in _context.Reservations
                                join c in _context.Clients on r.idCliente equals c.IdCliente
                                join h in _context.Habitaciones on r.idHabitacion equals h.Id
                                where r.idCliente == clientId
                                select new ReservHabitClientModel
                                {
                                    ReservationID = r.idRecepcion,
                                    In = r.FechaEntrada.Value,
                                    Out = r.FechaSalida.Value,
                                    Total = r.TotalPagado,
                                    RoomNumber = h.Numero,
                                    ClientID = c.IdCliente,
                                    ClientName = c.NombreCompleto
                                };

                    result.Data = await query.ToListAsync();
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = _getErrorMessage();
                    _logger.LogError(result.Message, ex.ToString());
                }
            }
            return result;
        }

        public async Task<OperationResult> GetDisponibleRoomsOfCategoryInTimeLapse(DateTime start, DateTime end, int categoriaId)
        {
            OperationResult result = new OperationResult();
            if (start > end)
            {
                result.IsSuccess = false;
                result.Message = "La fecha de inicio no puede ser mayor a la fecha de fin.";
            }
            else if (categoriaId == 0)
            {
                result.IsSuccess = false;
                result.Message = "No se ha especificado una categoría.";
            }
            else
            {
                try
                {
                    var query = _context.Habitaciones
                                    .Where(h => h.IdCategoria == categoriaId &&
                                        !_context.Reservations.Any(r =>
                                            r.idHabitacion == h.Id &&
                                            !(r.FechaSalida < start || r.FechaEntrada > end)   
                                        ))
                                    .Select(h => h.Id);
                    result.Data = await query.ToListAsync();
                }
                catch (Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = _getErrorMessage();
                    _logger.LogError(result.Message, ex.ToString());
                }
            }
            return result;
        }

        private async Task<bool> _isRoomDisponible(int? roomId, DateTime start, DateTime end)
        {
            if (roomId == null) return false;

            bool existeReserva = await _context.Reservations
                .AnyAsync(r => r.idHabitacion == roomId &&
                    ((r.FechaEntrada <= start && r.FechaSalida >= start) ||
                     (r.FechaEntrada <= end && r.FechaSalida >= end) ||
                     (r.FechaEntrada >= start && r.FechaSalida <= end)));

            return !existeReserva;
        }
        private string? _getErrorMessage([CallerMemberName]string source ="")
            => _configuration["ErrorReservationRepository:" + source]; 
    }
}

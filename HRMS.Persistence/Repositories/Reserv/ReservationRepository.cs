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
        private ILogger _logger;
        private IConfiguration _configuration;
        
        public ReservationRepository(HRMSContext context,ILogger logger, IConfiguration configuration) : base(context)
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
            if (!_validReservationForSaving(entity))
            {
                result = new OperationResult();
                result.IsSuccess = false;
                result.Message = "La reserva no es válida.";
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

        private bool _validReservationForSaving(Reservation resev)
            => (resev.idCliente != 0 &&
               resev.idCliente != null &&
               resev.FechaEntrada > DateTime.Now &&
               resev.FechaSalida > resev.FechaEntrada);

        private bool _validReservationForUpdating(Reservation resev)
            => (_validReservationForSaving(resev) &&
                (resev.Estado?? false));

        public override async Task<OperationResult> UpdateEntityAsync(Reservation entity)
        {
            OperationResult result;
            if (!_validReservationForUpdating(entity))
            {
                result = new OperationResult();
                result.IsSuccess = false;
                result.Message = "La reserva no es valida, para actualización";
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

        public virtual async Task<Reservation> GetEntityByIdAsync(int id)
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
            try
            {
                var query = from r in _Context.Reservations
                            join c in _Context.Clients on r.idCliente equals c.IdCliente
                            join h in _Context.Habitaciones on r.idHabitacion equals h.Id
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
            catch(Exception ex)
            {
                result.IsSuccess = false;
                result.Message = _getErrorMessage();
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        public async Task<OperationResult> GetReservationsInTimeLapse(DateTime start, DateTime end)
        {
            OperationResult result = new OperationResult();
            try
            {
                var query = _Context.Reservations.Where(r => r.FechaEntrada >= start && r.FechaSalida <= end);
                result.Data = await query.ToListAsync();
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = _getErrorMessage();
                _logger.LogError(result.Message, ex.ToString());
            }
            return result;
        }

        private string? _getErrorMessage([CallerMemberName]string source ="")
            => _configuration["ErrorMessages:ReservationRepository:" + source]; 
    }
}

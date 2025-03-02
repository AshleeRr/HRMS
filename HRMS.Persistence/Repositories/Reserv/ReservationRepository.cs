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
using HRMS.Domain.Base.Validator;
using MyValidator.Validator;
using HRMS.Domain.Entities.Servicio;

namespace HRMS.Persistence.Repositories.Reserv
{
    public class ReservationRepository : BaseRepository<Reservation, int>, IReservationRepository
    {
        private ILogger<ReservationRepository> _logger;
        private IValidator<Reservation> _validator;
        private IConfiguration _configuration;
         
        public ReservationRepository(HRMSContext context, ILogger<ReservationRepository> logger, IConfiguration configuration, IValidator<Reservation> validator) : base(context)
        {
            _logger = logger;
            _validator = validator;
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
                return await base.GetAllAsync(r => r.Estado.Value && filter.Compile()(r));
            }
            var res = new OperationResult();
            res.IsSuccess = false;
            res.Message = "No se ha especificado un filtro para la consulta.";
            return res;
        }


        public override async Task<OperationResult> SaveEntityAsync(Reservation entity)
        {
            OperationResult result;
            Task<bool> t_isRoomDisponible = _isRoomDisponible(entity.IdHabitacion, entity.FechaEntrada.Value, entity.FechaSalida.Value);
            var validRes = _validator.Validate(entity);
            bool isRoomDisponible = await t_isRoomDisponible;
            if (!validRes.IsSuccess)
            {
                return validRes;
            }
            else if (!isRoomDisponible)
            {
                var opRes = new OperationResult();
                opRes.IsSuccess = false;
                opRes.Message = "Habitación no disponible";
                return opRes;
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



        private async Task<OperationResult> _validReservationForUpdating(Reservation resev)
        {
            OperationResult operationResult = new OperationResult();
            List<string> errors = new List<string>();
            var b = _validator.Validate(resev);
            if (b.IsSuccess)
            {

                if (!resev.Estado.HasValue || !resev.Estado.Value)
                {
                    var opRes = new OperationResult();
                    opRes.IsSuccess = false;
                    opRes.Message = "No se puede editar una reserva ya eliminada";
                    return opRes;
                }
            }

            return b;
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
                                join c in _context.Clients on r.IdCliente equals c.IdCliente
                                join h in _context.Habitaciones on r.IdHabitacion equals h.IdHabitacion
                                where r.IdCliente == clientId
                                select new ReservHabitClientModel
                                {
                                    ReservationID = r.IdRecepcion,
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
                                            r.IdHabitacion == h.IdHabitacion &&
                                            !(r.FechaSalida < start || r.FechaEntrada > end)   
                                        ))
                                    .Select(h => h.IdHabitacion);
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
                .AnyAsync(r => r.IdHabitacion == roomId &&
                    ((r.FechaEntrada <= start && r.FechaSalida >= start) ||
                     (r.FechaEntrada <= end && r.FechaSalida >= end) ||
                     (r.FechaEntrada >= start && r.FechaSalida <= end)));

            return !existeReserva;
        }

        public async Task<OperationResult> GetPricesForServicesinRoomCategory(int categoryId, IEnumerable<int> servicesIds) 
        {
            OperationResult result = new OperationResult();
            if (categoryId == 0)
            {
                result.IsSuccess = false;
                result.Message = "No se ha especificado una categoría.";
            }
            else if (servicesIds == null || servicesIds.Count() == 0)
            {
                result.IsSuccess = false;
                result.Message = "No se han especificado servicios.";
            }
            else
            {
                try
                {
                    ServicioPorCategoria[] prices = await _context.ServicioPorCategorias
                                        .Where(sc => sc.CategoriaID == categoryId && servicesIds.Contains(sc.CategoriaID))
                                        //.Select(sc => sc.Precio)
                                        .ToArrayAsync();

                    if (prices.Length == servicesIds.Count())
                    {
                        result.Data = prices;
                    }
                    else
                    {
                        result.IsSuccess = false;
                        result.Message = "No todos los servicios se encuentran disponibles para esta categoria de habitación";
                    }
                }
                catch(Exception ex)
                {
                    result.IsSuccess = false;
                    result.Message = _getErrorMessage();
                    _logger.LogError(result.Message, ex.ToString());
                }
            }
            return result;
        }

        public Task<bool> CanRoomCategoryTakePeople(int categoryId, int people)
        {
           // _context.Habitaciones.AnyAsync(h => h.IdCategoria == categoryId && h.Capa >= people);
            throw new NotImplementedException();
        }

        private string? _getErrorMessage([CallerMemberName]string source ="")
            => _configuration["ErrorReservationRepository:" + source]; 
    }
}

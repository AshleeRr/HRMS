﻿using HRMS.Domain.Base;
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
            if (!await _validReservationForSaving(entity))
            {
                result = new OperationResult();
                result.IsSuccess = false;
                result.Message = "La reserva no es válida.";
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
        

        private async Task<bool> _validReservationForSaving(Reservation resev)
            => (resev != null &&
               resev.idCliente != 0 &&
               await _isRoomDisponible(resev.idHabitacion, resev.FechaEntrada.Value, resev.FechaSalida.Value) &&
               resev.idCliente != null &&
               resev.FechaEntrada > DateTime.Now &&
               resev.FechaSalida > resev.FechaEntrada);

        private async Task<bool> _validReservationForUpdating(Reservation resev)
            => (await _validReservationForSaving(resev)
                && (resev.Estado.Value)
                );

        public override async Task<OperationResult> UpdateEntityAsync(Reservation entity)
        {
            OperationResult result;
            if (!await _validReservationForUpdating(entity))
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

        private async Task<bool> _isRoomDisponible(int?  roomId, DateTime start, DateTime end)
        {
            return await _context.Habitaciones.Where(h => h.Id == roomId)
                .Where(h => !_context.Reservations
                    .Any(r => r.idHabitacion == h.Id &&
                        (r.FechaEntrada <= start && r.FechaSalida >= start) ||
                        (r.FechaEntrada <= end && r.FechaSalida >= end) ||
                        (r.FechaEntrada >= start && r.FechaSalida <= end)))
                .AnyAsync();
        }
        private string? _getErrorMessage([CallerMemberName]string source ="")
            => _configuration["ErrorReservationRepository:" + source]; 
    }
}

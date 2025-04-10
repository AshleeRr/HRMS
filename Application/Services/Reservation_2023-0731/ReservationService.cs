﻿using HRMS.Application.DTOs.Reservation_2023_0731;
using HRMS.Application.Interfaces.Reservation_2023_0731;
using HRMS.Domain.Base;
using HRMS.Domain.Entities.Reservations;
using HRMS.Domain.Entities.Servicio;
using HRMS.Domain.InfraestructureInterfaces.Logging;
using HRMS.Domain.Repository;
using HRMS.Infraestructure.Notification;
using HRMS.Models.Models.ReservationModels;

namespace HRMS.Application.Services.Reservation_2023_0731
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly INotificationService _notificationService;
        private readonly ILoggingServices _loggingServices;

        public ReservationService(IReservationRepository reservationRepository, INotificationService notificationService, ILoggingServices loggingServices)
        {
            _reservationRepository = reservationRepository;
            _notificationService = notificationService;
            _loggingServices = loggingServices;
        }

        public async Task<OperationResult> CancelReservation(int id)
        {
            var result = new OperationResult();
            try
            {
                if(id <= 0)
                {
                    result.IsSuccess = false;
                    result.Message = "No se ha especificado la reserva a buscar";
                    return result;
                }
                Reservation resv = await _reservationRepository.GetEntityByIdAsync(id);
                if (resv == null)
                {
                    result.IsSuccess = false;
                    result.Message = "No se ha encontrado la reserva";
                    return result;
                }
                else if (!resv.EstadoReserva.Equals(EstadoReserva.Pendiente))
                {
                    result.IsSuccess = false;
                    result.Message = "Solo se pueden cancelar reservas Pendientes, no confirmadas ni canceladas";
                    return result;
                }
                else
                {
                    resv.EstadoReserva = EstadoReserva.Cancelada;
                    result = await _reservationRepository.UpdateEntityAsync(resv);
                    result.Data = resv;
                }
            }
            catch (Exception ex)
            {
                result = await _loggingServices.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<OperationResult> ConfirmReservation(ReservationConfirmDTO dto)
        {
            var result = new OperationResult();
            try
            {
                if (dto.ReservationId <= 0)
                {
                    result.IsSuccess = false;
                    result.Message = "No se ha especificado la reserva a confirmar";
                    return result;
                }
                Reservation resv = await _reservationRepository.GetEntityByIdAsync(dto.ReservationId);
                if(resv == null)
                {
                    result.IsSuccess = false;
                    result.Message = "No se ha encontrado la reserva a confirmar";
                     
                }
                else if (!resv.EstadoReserva.Equals(EstadoReserva.Pendiente))
                {
                    result.IsSuccess = false;
                    result.Message = "Solo se pueden confirmar reservas Pendientes, no confirmadas ni canceladas";
                    return result;
                }
                else if(!(resv.PrecioRestante == dto.Abono))
                {
                    result.IsSuccess = false;
                    result.Message = "El abono no coincide con el precio restante";
                }
                else
                {
                    resv.TotalPagado += dto.Abono;
                    resv.EstadoReserva = EstadoReserva.Confirmada;
                    result = await _reservationRepository.UpdateEntityAsync(resv);
                    result.Data = resv;
                }
                await _notificationService.SendNotification(dto.UserID, "Su reservación ha sido confirmada");
            }
            catch (Exception ex)
            {
                result = await _loggingServices.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<OperationResult> GetAll()
        {
            var result = new OperationResult();
            try
            {
                var resv = await _reservationRepository.GetAllAsync();
                result.Data = resv.Select(r => MapToDto(r));
            }
            catch(Exception ex)
            {
                result = await _loggingServices.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<OperationResult> GetById(int id)
        {
            var result = new OperationResult();
            try
            {
                if(id <= 0)
                {
                    result.IsSuccess = false;
                    result.Message = "El id no se ha especificado";
                   
                }
                else
                {
                    var resv = await _reservationRepository.GetEntityByIdAsync(id);
                    result.Data = MapToDto(resv);
                }

            }
            catch (Exception ex)
            {
                result = await _loggingServices.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<OperationResult> GetReservationByClientID(int id)
        {
            var result = new OperationResult();
            try
            {
                if (id <= 0)
                {
                    result.IsSuccess = false;
                    result.Message = "El id no se ha especificado";
                }
                else
                {
                    var resv = await _reservationRepository.GetReservationsByClientId(id);
                    var dto = ((IEnumerable<ReservHabitClientModel>)resv.Data).Select(r => MapToDTOClientInfo(r));
                    result.Data = dto;
                }
            }
            catch (Exception ex)
            {
                result = await _loggingServices.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<OperationResult> Remove(ReservationRemoveDTO dto)
        {
            var result = new OperationResult();
            try
            {
                Reservation resv = await _reservationRepository.GetEntityByIdAsync(dto.ReservationId);
                if (resv == null)
                {
                    result.IsSuccess = false;
                    result.Message = "No se ha encontrado la reserva";
                    return result;
                }
                else if (resv.EstadoReserva == EstadoReserva.Cancelada)
                {
                    result.IsSuccess = false;
                    result.Message = "No se puede confirmar una reserva cancelada";
                    return result;
                }
                else
                {
                    resv.Estado = false;
                    result = await _reservationRepository.UpdateEntityAsync(resv);
                    result.Data = resv;
                }
            }
            catch (Exception ex)
            {
                result = await _loggingServices.LogError(ex.Message, this);
            }
            return result;
        }
        

        public async Task<OperationResult> Save(ReservationAddDTO dto)
        {
            var result = new OperationResult();
            try
            {
                
                var userExists = await _reservationRepository.ExistUser(dto.UserID);
                OperationResult CatAndTarif = await _reservationRepository.GetCategoryForReserv(dto.RoomCategoryID, dto.In, dto.Out);

                if (!CatAndTarif.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = "No se ha encontrado la categoria de habitación seleccionada";
                }
                else if (!userExists.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = "No se ha encontrado el usuario";
                }

                else if (((CategoryRoomForReserv)CatAndTarif.Data).Capacity < dto.PeopleNumber)
                {
                    result.IsSuccess = false;
                    result.Message = "La cantidad de personas supera la capacidad de la habitación";
                }
                else
                {
                    var servicesSelected = await _reservationRepository.GetPricesForServicesinRoomCategory(dto.RoomCategoryID, dto.Services);
                    if (!servicesSelected.IsSuccess)
                    {
                        result = servicesSelected;
                    }
                    else
                    {
                        var habitacionId = await _reservationRepository.GetDisponibleRoomsOfCategoryInTimeLapse(dto.In, dto.Out, dto.RoomCategoryID);
                        if (!habitacionId.IsSuccess)
                        {
                            result = habitacionId;
                        }
                        else
                        {
                            var categoryInfo = ((CategoryRoomForReserv)CatAndTarif.Data);
                            int idHabitacion = ((int)habitacionId.Data);

                            //Calculo de precio solo por la habitación
                            int days = (dto.Out - dto.In).Days;
                            decimal totalForRoom = categoryInfo.PricePerNight * days;

                            //Extracción de Servicios y precio
                            var ServicesForReserv = ((ServicioPorCategoria[])servicesSelected.Data);
                            decimal totalForServices = ServicesForReserv.Sum(x => x.Precio);

                            decimal total = totalForRoom + totalForServices;
                            if (total < dto.Adelanto)
                            {
                                result.IsSuccess = false;
                                result.Message = "El adelanto no puede ser mayor al total de la reserva";
                            }
                            else if (dto.Adelanto < (total * 0.3m))
                            {
                                result.IsSuccess = false;
                                result.Message = "El adelanto debe ser al menos el 30% del total de la reserva";
                            }
                            else
                            {
                                Reservation reservation = new Reservation()
                                {
                                    IdHabitacion = idHabitacion,
                                    FechaCreacion = DateTime.Now,
                                    FechaEntrada = dto.In,
                                    FechaSalida = dto.Out,
                                    Observacion = dto.Observations,
                                    PrecioInicial = total,
                                    IdCliente = dto.UserID,
                                    Adelanto = dto.Adelanto,
                                    EstadoReserva = EstadoReserva.Pendiente,
                                    TotalPagado = dto.Adelanto,
                                    PrecioRestante = total - dto.Adelanto,
                                    CostoPenalidad = 0,
                                    ReservaServicios = ServicesForReserv.Select(x => new ServicioPorReservacion()
                                    {
                                        ServicioID = x.ServicioID,
                                        Precio = x.Precio
                                    }).ToList()
                                };


                                result = await _reservationRepository.SaveEntityAsync(reservation);
                                if (result.IsSuccess)
                                {
                                    /*
                                    var servicios = ServicesForReserv.Select(x => new ServicioPorReservacion()
                                    {
                                        ServicioID = x.ServicioID,
                                        Precio = x.Precio,
                                        ReservacionID = reservation.IdRecepcion
                                    });
                                    */
                                    await _notificationService.SendNotification(dto.UserID, "Su reservación ha sido creada");
                                }
                                else
                                {
                                    result.IsSuccess = false;
                                    result.Message = "No se ha podido guardar la reserva";
                                }
                            }
                        }

                    }
                }
                
            }
            catch(Exception ex)
            {
                result = await _loggingServices.LogError(ex.Message, this);
            }
            return result;
        }

        public async Task<OperationResult> Update(ReservationUpdateDTO dto)
        {
            OperationResult result = new OperationResult();
            try
            {
                var reservation = await _reservationRepository.GetEntityByIdAsync(dto.ID);
                if (reservation == null)
                {
                    result.IsSuccess = false;
                    result.Message = "No se ha encontrado la reserva a actualizar";
                    return result;
                }
                else if (reservation.EstadoReserva != EstadoReserva.Pendiente)
                {
                    result.IsSuccess = false;
                    result.Message = "No se puede modificar cuyo estado no sea pendiente";
                    return result;
                }
                else
                {
                    var tarifInfo = await _reservationRepository.GetCategoryForReservByRoom(reservation.IdHabitacion.Value, dto.In, dto.Out);
                    if (reservation.FechaEntrada != dto.In || reservation.FechaSalida != dto.Out)
                    {
                        OperationResult newRoom = await _reservationRepository.GetDisponibleRoomsOfCategoryInTimeLapse(dto.In, dto.Out, tarifInfo.Data.Id);
                        if (!newRoom.IsSuccess)
                        {
                            result = newRoom;
                        }
                        else
                        {
                            var servicesTotal = await _reservationRepository.GetTotalForServices(reservation.IdRecepcion);
                            if(!servicesTotal.IsSuccess)
                            {
                                result = servicesTotal;
                            }
                            else
                            {
                                reservation.Observacion = dto.Observations;
                                int days = (dto.Out - dto.In).Days;
                                decimal totalForRoom = tarifInfo.Data.PricePerNight * days;
                                reservation.IdHabitacion = (int)newRoom.Data;
                                reservation.FechaEntrada = dto.In;
                                reservation.FechaSalida = dto.Out;
                                reservation.CostoPenalidad += (reservation.TotalPagado + reservation.PrecioRestante) * 0.1m;
                                reservation.PrecioRestante =  (totalForRoom + reservation.CostoPenalidad + (decimal) servicesTotal.Data) - reservation.Adelanto;
                                var op = await _reservationRepository.UpdateEntityAsync(reservation);
                                if (op.IsSuccess)
                                {
                                    result.Data = reservation;
                                }
                                else
                                {
                                    result.IsSuccess = false;
                                    result.Message = "No se ha podido actualizar la reserva";
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                result = await _loggingServices.LogError(ex.Message, this);
            }

            return result;
        }

        public ReservationDTO MapToDto(Reservation reservation)
            => new ReservationDTO
            {
                UserID = reservation.IdCliente.Value,
                Advance = reservation.Adelanto,
                EntryDate = reservation.FechaEntrada,
                DepartureDate = reservation.FechaSalida,
                DepartureConfirmationDate = reservation.FechaSalidaConfirmacion,
                InitialPrice = reservation.PrecioInicial,
                State = reservation.EstadoReserva.ToString(),
                Observation = reservation.Observacion,
                RoomId = reservation.IdHabitacion,
                RemainingPrice = reservation.PrecioRestante,
                PenaltyCost = reservation.CostoPenalidad,
                ReservationId = reservation.IdRecepcion,
                TotalPaid = reservation.TotalPagado,
                ClientId = reservation.IdCliente
            };

        public ReservClientInfoDTO MapToDTOClientInfo(ReservHabitClientModel model)
        {
            return new ReservClientInfoDTO
            {
                ClientID = model.ClientID,
                ClientName = model.ClientName,
                In = model.In,
                Out = model.Out,
                ReservationID = model.ReservationID,
                RoomNumber = model.RoomNumber,
                Total = model.Total
            };
        }
    }
}

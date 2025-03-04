using HRMS.Application.DTOs.Reservation_2023_0731;
using HRMS.Application.Interfaces.Reservation_2023_0731;
using HRMS.Domain.Base;
using HRMS.Domain.Entities.Reservations;
using HRMS.Domain.Entities.Servicio;
using HRMS.Domain.InfraestructureInterfaces.Logging;
using HRMS.Domain.Repository;
using HRMS.Models.Models.ReservationModels;


namespace HRMS.Application.Services.Reservation_2023_0731
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly ILoggingServices _loggingServices;

        public ReservationService(IReservationRepository reservationRepository, ILoggingServices loggingServices)
        {
            _reservationRepository = reservationRepository;
            _loggingServices = loggingServices;
        }

        public async Task<OperationResult> CancelReservation(int id)
        {
            var result = new OperationResult();
            try
            {
                Reservation resv = await _reservationRepository.GetEntityByIdAsync(id);
                if (resv == null)
                {
                    result.IsSuccess = false;
                    result.Message = "No se ha encontrado la reserva";
                    return result;
                }
                else if (resv.EstadoReserva == EstadoReserva.Confirmada)
                {
                    result.IsSuccess = false;
                    result.Message = "No se puede cancelar una reserva confirmada";
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
                Reservation resv = await _reservationRepository.GetEntityByIdAsync(dto.ReservationId);
                if(resv == null)
                {
                    result.IsSuccess = false;
                    result.Message = "No se ha encontrado la reserva";
                     
                }
                else if (resv.EstadoReserva == EstadoReserva.Cancelada)
                {
                    result.IsSuccess = false;
                    result.Message = "No se puede confirmar una reserva cancelada";
                }
                else if(resv.EstadoReserva == EstadoReserva.Confirmada)
                {
                    result.IsSuccess = false;
                    result.Message = "La reserva ya ha sido confirmada";
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
                var resv = await _reservationRepository.GetEntityByIdAsync(id);
                result.Data = resv;
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
                var resv = await _reservationRepository.GetReservationsByClientId(id);
                result.Data = resv;
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
                var servicesSelected = await _reservationRepository.GetPricesForServicesinRoomCategory(dto.RoomCategoryID, dto.Services);
                var userExists = await _reservationRepository.ExistUser(dto.UserID);
                OperationResult CatAndTarif = await _reservationRepository.GetCategoryForReserv(dto.RoomCategoryID, dto.PeopleNumber,dto.In, dto.Out);
                if (!servicesSelected.IsSuccess)
                {
                    result = servicesSelected;
                }
                else if (!CatAndTarif.IsSuccess)
                {
                    result.IsSuccess = false;
                    result.Message = "No se ha encontrado la categoria de habitación seleccionada";
                }
                else if(userExists.IsSuccess)
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
                    result.Message = "No se ha encontrado la reserva";
                    return result;
                }
                else if (reservation.EstadoReserva == EstadoReserva.Cancelada)
                {
                    result.IsSuccess = false;
                    result.Message = "No se puede modificar una reserva cancelada";
                    return result;
                }
                else if(reservation.EstadoReserva == EstadoReserva.Confirmada)
                {
                    result.IsSuccess = false;
                    result.Message = "No se puede modificar una reserva confirmada";
                    return result;
                }
                else
                {
                    var tarifInfo = await _reservationRepository.GetCategoryForReservByRoom(reservation.IdHabitacion.Value, 1, dto.In, dto.Out);
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
                TotalPaid = reservation.TotalPagado
            };
    }
}

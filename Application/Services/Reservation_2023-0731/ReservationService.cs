using HRMS.Application.DTOs.Reservation_2023_0731;
using HRMS.Application.Interfaces.Reservation_2023_0731;
using HRMS.Domain.Base;
using HRMS.Domain.InfraestructureInterfaces.Logging;
using HRMS.Domain.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Application.Services.Reservation_2023_0731
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly ILoggingServices _loggingServices;


        public Task<OperationResult> CancelReservation(int id)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> ConfirmReservation(int id)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> GetReservationByClientID(int id)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> Remove(ReservationRemoveDTO dto)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResult> Save(ReservationAddDTO dto)
        {
            var result = new OperationResult();
            var servivesSelected = _reservationRepository.GetPricesForServicesinRoomCategory(dto.RoomCategoryID, dto.Services);

            throw new NotImplementedException();
        }

        public Task<OperationResult> Update(ReservationUpdateDTO dto)
        {
            throw new NotImplementedException();
        }
    }
}

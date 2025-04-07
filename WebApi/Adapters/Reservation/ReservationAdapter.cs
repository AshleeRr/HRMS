using HRMS.WebApi.Logging;
using HRMS.WebApi.Models;
using HRMS.WebApi.Models.Reservation_2023_0731;
using WebApi.ServicesInterfaces.Reservation;
using WebApi.Validators;

namespace WebApi.Adapters.Reservation
{
    public class ReservationAdapter : AdapterBase<int, ReservationDTO, ReservationAddDTO, ReservationUpdateDTO>, IReservationRepository
    {
        private readonly IValidatorProvider _validatorProvider;
        public ReservationAdapter(IHttpClientFactory httpClientFactory,ILoggingServices loggingServices, IConfiguration configuration, IValidatorProvider validatorProvider)
            : base(httpClientFactory,loggingServices, "Reservations")
        {
            _validatorProvider = validatorProvider;
        }

        public async Task<OperationResult> Create(ReservationAddDTO dto)
        {
            var validator = _validatorProvider.GetValidator<ReservationAddDTO>();
            var validRes =  validator.Validate(dto);
            if (!validRes.IsSuccess)
            {
                return validRes;
            }
            return await base.Post(dto, "CreateReservation");
        }

        public Task<OperationResult> GetAll() 
            => base.GetAll();
        

        public Task<OperationResult> GetById(int id)
        {
            return base.GetById(id);
        }

        public async Task<OperationResult> Update(ReservationUpdateDTO dto)
        {
            var validator = _validatorProvider.GetValidator<ReservationUpdateDTO>();
            var validRes = validator.Validate(dto);
            if (!validRes.IsSuccess)
            {
                return validRes;
            }
            return await base.Put(dto, "UpdateReservation");
        }
    }

}

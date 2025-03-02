using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator.ServiceValidations;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Persistence.Repositories.RoomRepository;

public class TarifaRepository : BaseRepository<Tarifas, int> ,  ITarifaRepository
{

    public TarifaRepository(HRMSContext context) : base(context) 
    {
    }


    public Task<OperationResult> GetTarifasVigentesAsync(DateTime fecha)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult> GetHabitacionByPrecioAsync(decimal precio)
    {
        throw new NotImplementedException();
    }
}
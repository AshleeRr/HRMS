using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HRMS.Persistence.Repositories.RoomRepository;

public class TarifaRepository : BaseRepository<Tarifas, int> ,  ITarifaRepository
{
    
    private readonly ILogger<TarifaRepository> _logger;
    private readonly IConfiguration _configuration;
    private  IValidator<Tarifas> _validator;


    public TarifaRepository(HRMSContext context ,  ILogger<TarifaRepository> logger,
        IConfiguration configuration ,  IValidator<Tarifas> validator) : base(context)
    {
        _logger = logger;
        _configuration = configuration;
        _validator = validator;
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
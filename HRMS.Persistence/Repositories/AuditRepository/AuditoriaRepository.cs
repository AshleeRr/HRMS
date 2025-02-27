using HRMS.Domain.Base;
using HRMS.Domain.Entities.Audit;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IAuditRepository;
using HRMS.Persistence.Repositories.ValidationsRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace HRMS.Persistence.Repositories.AuditRepository
{
    public class AuditoriaRepository : BaseRepository<Auditoria, int>, IAuditoriaRepository
    {
        private readonly ILogger<AuditoriaRepository> _logger;
        private readonly IConfiguration _configuration;

        public AuditoriaRepository(HRMSContext context, ILogger<AuditoriaRepository> logger,
                                                     IConfiguration configuration) : base(context)
        {
            _logger = logger;
            _configuration = configuration;
        }
        public override async Task<bool> ExistsAsync(Expression<Func<Auditoria, bool>> filter)
        {
            if(filter == null)
            {
                return false;
            }
            return await base.ExistsAsync(filter);

        }
        public override async Task<OperationResult> GetAllAsync(Expression<Func<Auditoria, bool>> filter)
        {
            OperationResult result = new OperationResult();
            try
            {
                var auditorias = await _context.Users.Where(a => a.Estado == true).ToListAsync();
                if (!auditorias.Any())
                {
                    _logger.LogWarning("No se encontraron auditorias oficiales");
                }
                result.Data = auditorias;
                result.IsSuccess = true;
            }
            catch (Exception ex)
            {
                result.IsSuccess = false;
                result.Message = _configuration["ErrorAuditoriaRepository: GetAllAsync"];
                _logger.LogError(result.Message, ex.ToString());
            }

            return result;
        }
        public override async Task<Auditoria> GetEntityByIdAsync(int id) // registro por id
        {
            var entity = await _context.Auditorias.FindAsync(id);
            if (entity == null)
            {
                _logger.LogWarning("No se encontró un registro de auditoria con ese id");
            }
            return entity;
        }
        public async Task<OperationResult> LogAuditAsync(string accion, int idUsuario)
        {
            OperationResult result = new OperationResult();
            try
            {
                if (!Validation.ValidateAction(accion, result))
                    return result;
                var registroAutoria = new Auditoria
                {
                    Accion = accion,
                    IdUsuario = idUsuario,
                    FechaRegistro = DateTime.UtcNow
                };
                await _context.Auditorias.AddAsync(registroAutoria);
                await _context.SaveChangesAsync();
                result.IsSuccess = true;
            }
            catch (Exception e)
            {
                result.Message = _configuration["ErrorAuditoriaRepository: LogAuditAsync"];
                result.IsSuccess = false;
                _logger.LogError(result.Message, e.ToString());

            }
            return result;
        }
        public async Task<List<Auditoria>> GetAuditByUserIdAsync(int idUsuario)
        {
           var auditoriasByUserId = await _context.Auditorias.Where(adt => adt.IdUsuario == idUsuario).ToListAsync();
           if (!auditoriasByUserId.Any())
           {
                _logger.LogWarning("No se encontraron registros de auditoria para el usuario solicitado");
           }
            return auditoriasByUserId;
        }

        public async Task<List<Auditoria>> GetAuditByDateTime(DateTime fechaRegistro)
        {
            var auditorias = await _context.Auditorias.Where(adt => adt.FechaRegistro.Date == fechaRegistro.Date).ToListAsync();
           
            if (!auditorias.Any())
            {
                _logger.LogWarning("No se encontraron registros de auditoria para la fecha solicitada");
            }
            return auditorias;
        }
    }
}

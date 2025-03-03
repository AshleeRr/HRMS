using HRMS.Application.Base;
using HRMS.Application.DTOs.RoomManagementDto.TarifaDto;
using HRMS.Domain.Base;

namespace HRMS.Application.Interfaces.RoomManagementService;

public interface ITarifaService : IBaseServices<CreateTarifaDto, UpdateTarifaDto, DeleteTarifaDto>
{
    Task<OperationResult> GetTarifasVigentes(DateTime fecha);
    Task<OperationResult> GetHabitacionesByPrecio(decimal precio);
    Task<OperationResult> GetTarifasByCategoria(int idCategoria);
}
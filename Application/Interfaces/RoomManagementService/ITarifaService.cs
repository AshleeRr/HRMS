using HRMS.Application.Base;
using HRMS.Application.DTOs.RoomManagementDto.TarifaDtos;
using HRMS.Domain.Base;

namespace HRMS.Application.Interfaces.RoomManagementService;

public interface ITarifaService : IBaseServices<CreateTarifaDto, UpdateTarifaDto, DeleteTarifaDto>
{
    Task<OperationResult> GetTarifasVigentes(String fechaInput);
    Task<OperationResult> GetHabitacionesByPrecio(decimal precio);
}
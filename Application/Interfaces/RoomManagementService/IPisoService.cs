using HRMS.Application.Base;
using HRMS.Application.DTOs.RoomManagementDto.PisoDtos;
using HRMS.Domain.Base;

namespace HRMS.Application.Interfaces.RoomManagementService;

public interface IPisoService : IBaseServices<CreatePisoDto, UpdatePisoDto, DeletePisoDto>
{
    Task<OperationResult> GetPisoByDescripcion(string descripcion);
    Task<OperationResult> GetPisosWithHabitaciones();
    Task<OperationResult> GetPisoOcupacion();
}
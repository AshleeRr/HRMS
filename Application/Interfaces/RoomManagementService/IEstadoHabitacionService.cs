using HRMS.Application.Base;
using HRMS.Application.DTOs.RoomManagementDto.EstadoHabitacionDto;
using HRMS.Domain.Base;

namespace HRMS.Application.Interfaces.RoomManagementService;

public interface IEstadoHabitacionService : IBaseServices<CreateEstadoHabitacionDto, 
    UpdateEstadoHabitacionDto, DeleteEstadoHabitacionDto>
{
    Task<OperationResult> GetEstadoByDescripcion(string descripcion);
    Task<OperationResult> GetEstadosConHabitaciones();
    Task<OperationResult> GetEstadisticasPorEstado();
}
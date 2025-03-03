using HRMS.Application.Base;
using HRMS.Application.DTOs.RoomManagementDto.HabitacionDto;
using HRMS.Domain.Base;

namespace HRMS.Application.Interfaces.RoomManagementService;

public interface IHabitacionService : IBaseServices<CreateHabitacionDTo, UpdateHabitacionDto, DeleteHabitacionDto>
{
    Task<OperationResult> GetByPiso(int idPiso);
    Task<OperationResult> GetByNumero(string numero);
    Task<OperationResult> GetByCategoria(string categoria);
    Task<OperationResult> GetDisponibles(DateTime fechaInicio, DateTime fechaFin);
    Task<OperationResult> CambiarEstado(int idHabitacion, int idEstadoHabitacion);
}
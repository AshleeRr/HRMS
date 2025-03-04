using HRMS.Application.Base;
using HRMS.Application.DTOs.RoomManagementDto.HabitacionDtos;
using HRMS.Domain.Base;

namespace HRMS.Application.Interfaces.RoomManagementService;

public interface IHabitacionService : IBaseServices<CreateHabitacionDTo, UpdateHabitacionDto, DeleteHabitacionDto>
{
    Task<OperationResult> GetByPiso(int idPiso);
    Task<OperationResult> GetByNumero(string numero);
    Task<OperationResult> GetByCategoria(string categoria);
    Task<OperationResult> GetInfoHabitacionesAsync();
}
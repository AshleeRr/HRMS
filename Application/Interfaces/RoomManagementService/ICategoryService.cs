﻿using HRMS.Application.Base;
using HRMS.Application.DTOs.RoomManagementDto.CategoriaDTOS;
using HRMS.Domain.Base;

namespace HRMS.Application.Interfaces.RoomManagementService;

public interface ICategoryService :  IBaseServices<CreateCategoriaDto, UpdateCategoriaDto, DeleteCategoriaDto>
{
    Task<OperationResult> GetCategoriaByServicio(string nombreServicio);
    Task<OperationResult> GetCategoriaByDescripcion(string descripcion);
    Task<OperationResult> GetHabitacionesByCapacidad(int capacidad);
}
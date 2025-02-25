﻿using HRMS.Domain.Base;
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
    public async Task<OperationResult> GetTarifasVigentesAsync(DateTime fecha)
    {
        var result = new OperationResult();
        try
        {
            var datos = await _context.Set<Tarifas>()
                .Where(t => t.FechaInicio <= fecha && t.FechaFin >= fecha)
                .ToListAsync();
            result.Data = datos;
        }
        catch (Exception)
        {
            result.IsSuccess = false;
            result.Message = "Ocurrió un error obteniendo las tarifas vigentes.";
        }
        return result;
    }
    public async Task<OperationResult> GetTarifasPorHabitacionAsync(int idHabitacion)
    {
        var result = new OperationResult();
        try
        {
            var datos = await _context.Set<Tarifas>()
                .Where(t => t.IdHabitacion == idHabitacion)
                .ToListAsync();
            result.Data = datos;
        }
        catch (Exception)
        {
            result.IsSuccess = false;
            result.Message = "Ocurrió un error obteniendo las tarifas por habitación.";
        }   
        return result;
    }
    public async Task<OperationResult> GetTarifaActivaByHabitacionAsync(int idHabitacion)
    {
        var result = new OperationResult();
        try
        {
            var dato = await _context.Set<Tarifas>()
                .Where(t => t.IdHabitacion == idHabitacion 
                            && t.Estado == true 
                            && t.FechaInicio <= DateTime.Now 
                            && t.FechaFin >= DateTime.Now)
                .FirstOrDefaultAsync();
            result.Data = dato;
        }
        catch (Exception)
        {
            result.IsSuccess = false;
            result.Message = "Ocurrió un error obteniendo la tarifa activa.";
        }
        return result;
    }
}
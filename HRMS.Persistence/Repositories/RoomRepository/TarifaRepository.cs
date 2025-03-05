﻿using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.EntityFrameworkCore;
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
    
    public override async Task<List<Tarifas>> GetAllAsync()
    {
        return await _context.Tarifas.Where(t=> t.Estado == true).ToListAsync();
    }
    
    public override async Task<Tarifas> GetEntityByIdAsync(int id)
    {
        return (id != 0 ? await _context.Set<Tarifas>().FindAsync(id) : null) ?? throw new InvalidOperationException();    
    }

    public override async Task<OperationResult> SaveEntityAsync(Tarifas tarifas)
    {
        var result = ValidarTarifa(tarifas);
        if (!result.IsSuccess) return result;

        try
        {
            if (await ExistsAsync(t => t.PrecioPorNoche == tarifas.PrecioPorNoche))
            {
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = $"Ya existe una tarifa con el precio '{tarifas.PrecioPorNoche}'."
                };
            }

            _context.Tarifas.Add(tarifas);
            await _context.SaveChangesAsync();

            return new OperationResult
            {
                IsSuccess = true,
                Message = "Tarifa guardada exitosamente.",
                Data = tarifas
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al guardar la tarifa.");
            return new OperationResult
            {
                IsSuccess = false,
                Message = $"Error al guardar la tarifa: {ex.Message}"
            };
        }
    }

    public override async Task<OperationResult> UpdateEntityAsync(Tarifas tarifas)
    {
        var result = ValidarTarifa(tarifas);
        if (!result.IsSuccess) return result;

        try
        {
            var existingTarifa = await _context.Tarifas.FindAsync(tarifas.IdTarifa);
            if (existingTarifa == null)
            {
                return new OperationResult
                {
                    IsSuccess = false,
                    Message = $"No se encontró la tarifa con ID: {tarifas.IdTarifa}."
                };
            }

            UpdateTarifa(existingTarifa, tarifas);
            await _context.SaveChangesAsync();

            return new OperationResult
            {
                IsSuccess = true,
                Message = "Habitación actualizada correctamente.",
                Data = existingTarifa
            };
        }
        catch (DbUpdateConcurrencyException)
        {
            _logger.LogError("Error al actualizar la tarifa. La tarifa ya ha sido modificada por otro usuario.");
            return new OperationResult
            {
                IsSuccess = false,
                Message = "La tarifa ya ha sido modificada por otro usuario. Intente nuevamente."
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al actualizar la tarifa.");
            return new OperationResult
            {
                IsSuccess = false,
                Message = $"Error al actualizar la tarifa: {ex.Message}"
            };
        }

    }

    public async Task<OperationResult> GetTarifasVigentesAsync(string fechaInput)
    {
        try
        {
            _logger.LogInformation("Validando el formato de la fecha ingresada: {FechaInput}", fechaInput);

            var fechaValidacion = ValidateFechaFormat(fechaInput);
            if (!fechaValidacion.IsSuccess) return fechaValidacion;

            DateTime fecha = (DateTime)fechaValidacion.Data;

            var tarifas = await _context.Tarifas
                .Where(t => t.Estado == true && t.FechaInicio <= fecha && t.FechaFin >= fecha)
                .ToListAsync();

            return tarifas.Any()
                ? new OperationResult { IsSuccess = true, Data = tarifas }
                : new OperationResult { IsSuccess = false, Message = $"No se encontraron tarifas vigentes para la fecha {fecha:yyyy-MM-dd}." };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al obtener tarifas vigentes.");
            return new OperationResult { IsSuccess = false, Message = "Error al obtener tarifas vigentes." };
        }
    }

    public async Task<OperationResult> GetHabitacionByPrecioAsync(decimal precio)
    {
        try
        {
            if (precio <= 0)
                return new OperationResult
                    { IsSuccess = false, Message = "El precio de la tarifa debe ser mayor a 0." };

            _logger.LogInformation($"Buscando tarifas con precio: {precio}, fecha actual: {DateTime.Now}");

            var fechaActual = DateTime.Now;
            var tarifas = await _context.Tarifas
                .Where(t => t.PrecioPorNoche == precio && t.Estado == true
                                                       && t.FechaInicio <= fechaActual && t.FechaFin >= fechaActual)
                .ToListAsync();

            _logger.LogInformation($"Tarifas encontradas: {tarifas.Count}");

            if (!tarifas.Any())
            {
                var todasTarifas = await _context.Tarifas
                    .Where(t => t.PrecioPorNoche == precio && t.Estado == true)
                    .ToListAsync();

                _logger.LogInformation($"Tarifas con precio {precio} sin filtro de fecha: {todasTarifas.Count}");

                if (todasTarifas.Any())
                {
                    foreach (var t in todasTarifas)
                    {
                        _logger.LogInformation(
                            $"Tarifa ID: {t.IdTarifa}, FechaInicio: {t.FechaInicio}, FechaFin: {t.FechaFin}, Vigente: {t.FechaInicio <= fechaActual && t.FechaFin >= fechaActual}");
                    }
                }

                return new OperationResult
                    { IsSuccess = false, Message = "No se encontraron tarifas vigentes con el precio indicado." };
            }

            var categoriaIds = tarifas.Select(t => t.IdCategoria).ToList();
            _logger.LogInformation($"Categorías encontradas para las tarifas: {string.Join(", ", categoriaIds)}");

            var habitaciones = await _context.Habitaciones
                .Where(h => h.Estado == true && h.IdCategoria.HasValue && categoriaIds.Contains(h.IdCategoria.Value))
                .ToListAsync();

            _logger.LogInformation($"Habitaciones encontradas: {habitaciones.Count}");

            if (!habitaciones.Any())
            {
                var todasHabitacionesEnCategorias = await _context.Habitaciones
                    .Where(h => h.IdCategoria.HasValue && categoriaIds.Contains(h.IdCategoria.Value))
                    .ToListAsync();

                _logger.LogInformation(
                    $"Total de habitaciones en esas categorías (incluso inactivas): {todasHabitacionesEnCategorias.Count}");

                foreach (var h in todasHabitacionesEnCategorias)
                {
                    _logger.LogInformation(
                        $"Habitación ID: {h.IdHabitacion}, Categoría: {h.IdCategoria}, Estado: {h.Estado}");
                }
            }

            return habitaciones.Any()
                ? new OperationResult { IsSuccess = true, Data = habitaciones }
                : new OperationResult
                    { IsSuccess = false, Message = "No se encontraron habitaciones con el precio de tarifa indicado." };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al buscar habitaciones por precio de tarifa.");
            return new OperationResult
                { IsSuccess = false, Message = $"Error al buscar habitaciones por precio de tarifa: {ex.Message}" };
        }
    }

    private OperationResult ValidarTarifa(Tarifas tarifa)
    {
        var validation =  _validator.Validate(tarifa);
        return validation.IsSuccess ? new OperationResult() { IsSuccess = true } : validation;
    }
    
    private void UpdateTarifa(Tarifas entity, Tarifas entityToUpdate)
    {
        entityToUpdate.PrecioPorNoche = entity.PrecioPorNoche;
        entityToUpdate.FechaInicio = entity.FechaInicio;
        entityToUpdate.FechaFin = entity.FechaFin;
        entityToUpdate.Estado = entity.Estado;
    }
    
    private static OperationResult ValidateFechaFormat(string fechaInput)
    {
        if (string.IsNullOrWhiteSpace(fechaInput))
            return new OperationResult { IsSuccess = false, Message = "La fecha no puede estar vacía." };

        DateTime fecha;
        string[] formatosValidos = { "dd/MM/yyyy", "yyyy-MM-dd", "MM/dd/yyyy", "dd-MM-yyyy" };

        if (!DateTime.TryParseExact(fechaInput, formatosValidos, 
                System.Globalization.CultureInfo.InvariantCulture, 
                System.Globalization.DateTimeStyles.None, out fecha))
        {
            return new OperationResult
            {
                IsSuccess = false,
                Message = "El formato de la fecha es incorrecto. Usa formatos válidos: dd/MM/yyyy, yyyy-MM-dd, MM/dd/yyyy, dd-MM-yyyy."
            };
        }

        return new OperationResult { IsSuccess = true, Data = fecha };
    }

}
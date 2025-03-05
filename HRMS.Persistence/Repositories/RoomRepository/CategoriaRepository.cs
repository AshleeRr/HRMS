﻿using HRMS.Domain.Base;
using HRMS.Domain.Base.Validator;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Domain.Entities.Servicio;
using HRMS.Persistence.Base;
using HRMS.Persistence.Context;
using HRMS.Persistence.Interfaces.IRoomRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HRMS.Persistence.Repositories.RoomRepository
{
    public class CategoriaRepository : BaseRepository<Categoria, int>, ICategoryRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IValidator<Categoria> _validator;

        public CategoriaRepository(HRMSContext context, IConfiguration configuration, IValidator<Categoria> validator) 
            : base(context)
        {
            _configuration = configuration;
            _validator = validator;
        }

        public override async Task<List<Categoria>> GetAllAsync() =>
            await _context.Categorias.Where(c => c.Estado == true).ToListAsync();

        public override async Task<OperationResult> SaveEntityAsync(Categoria categoria) =>
            await ExecuteOperationAsync(async () =>
            {
                var validation = _validator.Validate(categoria);
                if (!validation.IsSuccess) return validation;

                if (await ExistsAsync(c => c.Descripcion == categoria.Descripcion))
                    return Failure($"Ya existe una categoría con la descripción '{categoria.Descripcion}'.");

                await _context.Categorias.AddAsync(categoria);
                await _context.SaveChangesAsync();

                return Success(categoria, "Categoría guardada correctamente.");
            });

        public override async Task<OperationResult> UpdateEntityAsync(Categoria categoria) =>
            await ExecuteOperationAsync(async () =>
            {
                var validation = _validator.Validate(categoria);
                if (!validation.IsSuccess) return validation;

                var existingCategoria = await _context.Categorias.FindAsync(categoria.IdCategoria);
                if (existingCategoria == null) return Failure("La categoría no existe.");

                if (await ExistsAsync(c => c.Descripcion == categoria.Descripcion && c.IdCategoria != categoria.IdCategoria))
                    return Failure($"Ya existe otra categoría con la descripción '{categoria.Descripcion}'.");

                UpdateCategoria(existingCategoria, categoria);
                await _context.SaveChangesAsync();

                return Success(existingCategoria, "Categoría actualizada correctamente.");
            });

        public async Task<OperationResult> GetCategoriaByServiciosAsync(string nombre) =>
            await ExecuteOperationAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(nombre))
                    return Failure("El nombre del servicio no puede estar vacío.");

                var categorias = await _context.Categorias
                    .Join(_context.Servicios,
                        c => c.IdServicio,
                        s => s.IdServicio,
                        (c, s) => new { Categoria = c, Servicio = s })
                    .Where(cs => cs.Servicio.Nombre.Contains(nombre) && cs.Categoria.Estado == true)
                    .Select(cs => cs.Categoria)
                    .ToListAsync();

                return Success(categorias,
                    categorias.Any() ? null : $"No se encontraron categorías para el servicio '{nombre}'.");
            });

        public async Task<OperationResult> GetServiciosByDescripcionAsync(string descripcion) =>
            await ExecuteOperationAsync(async () =>
            {
                if (string.IsNullOrWhiteSpace(descripcion)) 
                    return Failure("La descripción del servicio no puede estar vacía.");

                var servicios = await _context.Servicios
                    .Where(s => s.Descripcion.Contains(descripcion) && s.Estado == true)
                    .ToListAsync();

                return Success(servicios, servicios.Any() ? null : "No se encontraron servicios con esa descripción.");
            });
        
        public async Task<OperationResult> GetHabitacionByCapacidad(int capacidad) =>
            await ExecuteOperationAsync(async () =>
            {
                if (capacidad <= 0) return Failure("La capacidad debe ser mayor que cero.");

                var categoriasIds = await _context.Categorias
                    .Where(c => c.Capacidad == capacidad && c.Estado == true)
                    .Select(c => c.IdCategoria)
                    .ToListAsync();

                if (!categoriasIds.Any())
                    return Failure($"No se encontraron categorías con capacidad para {capacidad} personas.");

                var habitaciones = await _context.Habitaciones
                    .Where(h => h.IdCategoria.HasValue && categoriasIds.Contains(h.IdCategoria.Value) && h.Estado == true)
                    .ToListAsync();

                return Success(habitaciones, habitaciones.Any() ? null : $"No se encontraron habitaciones con capacidad para {capacidad} personas.");
            });


        private static void UpdateCategoria(Categoria existing, Categoria updated)
        {
            existing.Descripcion = updated.Descripcion;
            existing.IdServicio = updated.IdServicio;
            existing.Capacidad = updated.Capacidad;
            existing.Estado = updated.Estado;
        }

        private async Task<OperationResult> ExecuteOperationAsync(Func<Task<OperationResult>> operation)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex)
            {
                return Failure($"Ocurrió un error: {ex.Message}");
            }
        }

        private static OperationResult Success(object data = null, string message = null) =>
            new() { IsSuccess = true, Data = data, Message = message };

        private static OperationResult Failure(string message) =>
            new() { IsSuccess = false, Message = message };
    }
}

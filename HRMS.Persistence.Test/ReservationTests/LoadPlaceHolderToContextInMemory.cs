using HRMS.Domain.Entities.RoomManagement;
using HRMS.Domain.Entities.Servicio;
using HRMS.Domain.Entities.Users;
using HRMS.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRMS.Persistence.Test.ReservationTests
{
    public static class LoadPlaceHolderToContextInMemory
    {
        public static void LoadPlaceHolders(this HRMSContext _context)
        {
            var CategoryPlaceholder = new Categoria { IdCategoria = 1, Capacidad = 4, Descripcion = "Deluxe", FechaCreacion = DateTime.Now, Estado = true };
            var pisoPlaceholder = new Piso { IdPiso = 1, Descripcion = "Primer Piso" };
            var RoomPlaceholder = new Habitacion
            {
                IdHabitacion = 1,
                IdCategoria = 1,
                IdEstadoHabitacion = 1,
                Numero = "101",
                Detalle = "Habitacion de lujo",
                Estado = true,
                FechaCreacion = DateTime.Now,
                IdPiso = 1,
            };

            var serv1 = new Servicios
            {
                IdSercicio = 1,
                Nombre = "SPA",
                Descripcion = "Prueba 1",
                Estado = true,
                FechaCreacion = DateTime.Now
            };
            var serv2 = new Servicios
            {
                IdSercicio = 2,
                Nombre = "CINE",
                Descripcion = "Prueba 2",
                Estado = true,
                FechaCreacion = DateTime.Now
            };
            var TarifaPlaceholder = new Tarifas
            {
                IdTarifa = 1,
                IdCategoria = 1,
                PrecioPorNoche = 1000,
                Estado = true,
                Descripcion = "",
                FechaInicio = DateTime.Now.AddDays(2),
                FechaCreacion = DateTime.Now,
                FechaFin = DateTime.Now.AddDays(200),
                Descuento = 0
            };
   
            var clientePlaceholder = new Client
            {
                Correo = "fulano@gmail.com",
                Documento = "1314252",
                NombreCompleto = "Fulano de Tal",
                Estado = true,
                TipoDocumento = "Cedula",
                FechaCreacion = DateTime.Now,
            };
            _context.Categorias.Add(CategoryPlaceholder);
            _context.Pisos.Add(pisoPlaceholder);
            _context.Habitaciones.Add(RoomPlaceholder);
            _context.Tarifas.Add(TarifaPlaceholder);
            _context.Clients.Add(clientePlaceholder);
            _context.Servicios.Add(serv1);
            _context.Servicios.Add(serv2);
        }
    }
}

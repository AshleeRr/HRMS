using HRMS.Domain.Entities.RoomManagement;
using HRMS.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace HRMS.Persistence.Context
{
    public class HRMSContext : DbContext
    {
        public HRMSContext(DbContextOptions<HRMSContext> options) : base(options){}
            // Todas las entidades
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        
        public DbSet<Habitacion> Habitaciones { get; set; }
        public DbSet<EstadoHabitacion> EstadoHabitaciones { get; set; }
        public DbSet<Piso> Pisos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Tarifas> Tarifas { get; set; }

    }
}

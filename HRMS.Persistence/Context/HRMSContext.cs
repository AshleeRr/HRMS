using HRMS.Domain.Entities.Servicio;
using HRMS.Domain.Entities.Reservations;
using HRMS.Domain.Entities.RoomManagement;
using HRMS.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using HRMS.Persistence.Context.Configurations;

namespace HRMS.Persistence.Context
{
    public class HRMSContext : DbContext
    {
        public HRMSContext(DbContextOptions<HRMSContext> options) : base(options){}

        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<User> Users { get; set; } 
        public DbSet<Client> Clients { get; set; }
        public DbSet<Servicios> Servicios { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Habitacion> Habitaciones { get; set; }
        public DbSet<EstadoHabitacion> EstadoHabitaciones { get; set; }
        public DbSet<Piso> Pisos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Tarifas> Tarifas { get; set; }
        public DbSet<ServicioPorCategoria> ServicioPorCategorias { get; set; }
        public DbSet<ServicioPorReservacion> ServicioPorReservacions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new ServiciePerReservationConfig());
            modelBuilder.ApplyConfiguration(new ServicePerCategoryConfig());

            base.OnModelCreating(modelBuilder);
        }
    }
}

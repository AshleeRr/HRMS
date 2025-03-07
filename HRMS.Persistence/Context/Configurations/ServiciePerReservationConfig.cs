using HRMS.Domain.Entities.Servicio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace HRMS.Persistence.Context.Configurations
{
    public class ServiciePerReservationConfig : IEntityTypeConfiguration<ServicioPorReservacion>
    {
        public void Configure(EntityTypeBuilder<ServicioPorReservacion> builder)
        {
            builder.HasKey(rs => new { rs.ReservacionID, rs.ServicioID });

            builder.HasOne(rs => rs.Reserva)
                .WithMany(r => r.ReservaServicios)
                .HasForeignKey(rs => rs.ReservacionID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rs => rs.Servicio)
                .WithMany()
                .HasForeignKey(rs => rs.ServicioID);
        }
    }
}

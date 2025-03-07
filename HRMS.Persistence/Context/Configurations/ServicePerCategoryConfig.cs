using HRMS.Domain.Entities.Servicio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRMS.Persistence.Context.Configurations
{
    public class ServicePerCategoryConfig : IEntityTypeConfiguration<ServicioPorCategoria>
    {
        public void Configure(EntityTypeBuilder<ServicioPorCategoria> builder)
        {
            builder.HasKey(rs => new { rs.CategoriaID, rs.ServicioID });
        }
    }
}

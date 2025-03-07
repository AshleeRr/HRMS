

using System.ComponentModel.DataAnnotations.Schema;

namespace HRMS.Domain.Entities.Servicio
{
    [Table("ServiciosPorCategoria")]
    public class ServicioPorCategoria
    {
        public short ServicioID { get; set; }
        public int CategoriaID { get; set; }
        public decimal Precio { get; set; }
    }
}

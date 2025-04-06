using WebApi.Models.BaseDTO;

namespace WebApi.Models.RoomModels;

public class CategoriaModel : DtoBase
{

        public int IdCategoria { get; set; }
        public string Descripcion { get; set; }
        public short IdServicio { get; set; }
        public int Capacidad { get; set; }

}
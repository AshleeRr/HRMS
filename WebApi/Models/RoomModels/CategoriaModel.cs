namespace WebApi.Models.RoomModels;

public class CategoriaModel
{

        public int IdCategoria { get; set; }
        public string Descripcion { get; set; }
        public short IdServicio { get; set; }
        public int Capacidad { get; set; }
        public DateTime ChangeTime { get; set; }
        public int UserID { get; set; }
    
}
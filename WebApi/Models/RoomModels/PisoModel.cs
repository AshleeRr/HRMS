using WebApi.Models.BaseDTO;

namespace WebApi.Models.RoomModels;

public class PisoModel : DtoBase
{
    public int IdPiso { get; set; }
    public string Descripcion { get; set; }
  
}
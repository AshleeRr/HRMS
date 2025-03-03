namespace HRMS.Application.DTOs.RoomManagementDto.CategoriaDTO;

public class CreateCategoriaDto : DTOBase
{
    public string Descripcion { get; set; }
    public short IdServicio { get; set; }
    public int Capacidad { get; set; }
}
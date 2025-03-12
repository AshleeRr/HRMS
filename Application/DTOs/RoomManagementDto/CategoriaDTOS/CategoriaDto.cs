namespace HRMS.Application.DTOs.RoomManagementDto.CategoriaDTOS;

public class CategoriaDto : DTOBase
{
    public int IdCategoria { get; set; }
    public string Descripcion { get; set; }
    public short IdServicio { get; set; }
    public int Capacidad { get; set; }
}
namespace HRMS.Domain.Entities.Servicio;

public sealed class Servicios
{
    public short Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}
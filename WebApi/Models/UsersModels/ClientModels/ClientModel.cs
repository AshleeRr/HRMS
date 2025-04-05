namespace WebApi.Models.UsersModels.ClientModels
{
    public class ClientModel
    {
        public int IdCliente { get; set; }
        public int IdUsuario { get; set; }
        public string? NombreCompleto { get; set; }
        public string? Correo { get; set; }
        public string? Clave { get; set; }
        public string? Documento { get; set; }
        public string? TipoDocumento { get; set; }
        public DateTime ChangeTime { get; set; }
        public int UserID { get; set; }
    }
}
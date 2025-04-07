namespace WebApi.Models.UsersModels
{
    public class ClientModel
    {
        public int IdCliente { get; set; }
        public string TipoDocumento { get; set; }
        public string Documento { get; set; }
        public int IdUsuario { get; set; }
        public string Clave { get; set; }
        public string NombreCompleto { get; set; }
        public string Correo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public bool Estado { get; set; }
    }
}

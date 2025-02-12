namespace HRMS.Models.Models
{
    public class ClientModel
    {
        // modelo con los datos a mostrar en la vista
        public int IdCliente { get; set; }
        public string? TipoDomento { get; set; }
        public string? Documento { get; set; }
        public string? NombreCompleto { get; set; }
        public string? Correo { get; set; }

    }
}

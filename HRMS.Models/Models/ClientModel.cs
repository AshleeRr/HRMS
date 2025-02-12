namespace HRMS.Models.Models
{
    public class ClientModel : PersonModel
    {
        public int IdCliente { get; set; }
        public string? TipoDomento { get; set; }
        public string? Documento { get; set; }
      

    }
}

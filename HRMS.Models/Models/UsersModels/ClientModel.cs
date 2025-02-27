namespace HRMS.Models.Models.UsersModels
{
    public class ClientModel : PersonModel
    {
        public int IdCliente { get; set; }
        public string? TipoDomento { get; set; }
        public string? Documento { get; set; }
        public int? IdUsuario { get; set; }


    }
}

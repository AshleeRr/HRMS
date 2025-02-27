
namespace HRMS.Models.Models.UsersModels.UsersModels
{
    public class ClientUpdatedModel : PersonModel
    {
        public bool Estado { get; set; }
        public string TipoDocumento { get; set; }
        public string Documento { get; set; }

    }
}

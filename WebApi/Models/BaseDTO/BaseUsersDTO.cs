using WebApi.Models.BaseDTO;

namespace HRMS.Application.DTOs.BaseDTO
{
    public class BaseUsersDTO : DtoBase
    {
        public string NombreCompleto { get; set; }
        public string Correo { get; set; }
        public string Clave { get; set; }
        public string TipoDocumento { get; set; }
        public string Documento { get; set; }
    }
}
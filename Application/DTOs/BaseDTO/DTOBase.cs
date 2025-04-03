namespace HRMS.Application.DTOs.BaseDTO
{
    public abstract class DTOBase
    {
        public DateTime? ChangeTime { get; set; }
        public int UserID { get; set; }
    }
}

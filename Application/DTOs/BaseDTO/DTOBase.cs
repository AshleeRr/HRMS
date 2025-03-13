namespace HRMS.Application.DTOs
{
    public abstract class DTOBase
    {
        public DateTime ChangeTime { get; set; }
        public int UserID { get; set; }
        public bool Deleted { get; set; } 
    }
}

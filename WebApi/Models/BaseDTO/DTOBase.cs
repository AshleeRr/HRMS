namespace WebApi.Models.BaseDTO
{
    public abstract class DtoBase
    {
        public DateTime? ChangeTime { get; set; }
        public int? UserID { get; set; }
    }
}

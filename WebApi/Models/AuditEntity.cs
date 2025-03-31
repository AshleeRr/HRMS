namespace WebApi.Models;

public abstract class AuditEntity
{
    public DateTime ChangeTime { get; set; }
    public int UserID { get; set; }

}
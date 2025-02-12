namespace Base.Domain;

public class BaseEntity
{
    public int Id { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public string UpdatedBy { get; set; } = default!;
    public DateTime UpdatedAt { get; set; }
}
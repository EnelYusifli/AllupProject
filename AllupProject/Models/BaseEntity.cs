namespace AllupProject.Models;

public class BaseEntity
{
    public int Id { get; set; }
    public bool IsDeactive { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
}

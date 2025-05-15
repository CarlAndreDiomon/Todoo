using System.ComponentModel.DataAnnotations;

public class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Status { get; set; }
    public DateTime Deadline { get; set; }
    public bool IsCompleted { get; set; }
    public string UserId { get; set; }
}

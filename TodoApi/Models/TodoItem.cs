namespace TodoApi.Models;

public class TodoItem
{
    public required long TodoListId { get; set; }
    public required long ItemId { get; set; }
    public required string Description { get; set; }
    public bool Completed { get; set; }
}
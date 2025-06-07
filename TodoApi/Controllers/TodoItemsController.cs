using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Dtos;
using TodoApi.Models;

namespace TodoApi.Controllers;

[Route("api/todolists/{todoListId}/todos")]
[ApiController]
public class TodoItemsController : ControllerBase
{
    private readonly TodoContext _context;

    public TodoItemsController(TodoContext context)
    {
        _context = context;
    }

    // GET: api/todolists/5/todos
    [HttpGet]
    public async Task<ActionResult<IList<TodoItem>>> GetTodoItems(long todoListId)
    {
        if (!await TodoListExists(todoListId))
        {
            return NotFound();
        }

        var items = await _context.TodoItems
            .Where(i => i.TodoListId == todoListId)
            .ToListAsync();

        return Ok(items);
    }

    // GET: api/todolists/5/todos/3
    [HttpGet("{itemId}")]
    public async Task<ActionResult<TodoItem>> GetTodoItem(long todoListId, long itemId)
    {
        var todoItem = await _context.TodoItems
            .FirstOrDefaultAsync(i => i.TodoListId == todoListId && i.ItemId == itemId);

        if (todoItem == null)
        {
            return NotFound();
        }

        return Ok(todoItem);
    }

    // PUT: api/todolists/5/todos/3
    [HttpPut("{itemId}")]
    public async Task<ActionResult> PutTodoItem(long todoListId, long itemId, UpdateTodoItem payload)
    {
        var todoItem = await _context.TodoItems
            .FirstOrDefaultAsync(i => i.TodoListId == todoListId && i.ItemId == itemId);

        if (todoItem == null)
        {
            return NotFound();
        }

        todoItem.Description = payload.Description;
        todoItem.Completed = payload.Completed;

        await _context.SaveChangesAsync();

        return Ok(todoItem);
    }

    // PATCH: api/todolists/5/todos/3/done
    [HttpPatch("{itemId}/done")]
    public async Task<ActionResult> MarkTodoItemAsDone(long todoListId, long itemId)
    {
        var todoItem = await _context.TodoItems
            .FirstOrDefaultAsync(i => i.TodoListId == todoListId && i.ItemId == itemId);

        if (todoItem == null)
        {
            return NotFound();
        }

        todoItem.Completed = true;
        await _context.SaveChangesAsync();

        return Ok(todoItem);
    }

    // POST: api/todolists/5/todos
    [HttpPost]
    public async Task<ActionResult<TodoItem>> PostTodoItem(long todoListId, CreateTodoItem payload)
    {
        if (!await TodoListExists(todoListId))
        {
            return NotFound();
        }

        // Get the next ItemId for this TodoList
        var nextItemId = await _context.TodoItems
            .Where(i => i.TodoListId == todoListId)
            .MaxAsync(i => (long?)i.ItemId) ?? 0;

        var todoItem = new TodoItem
        {
            TodoListId = todoListId,
            ItemId = nextItemId + 1,
            Description = payload.Description,
            Completed = payload.Completed
        };

        _context.TodoItems.Add(todoItem);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetTodoItem),
            new { todoListId, itemId = todoItem.ItemId },
            todoItem
        );
    }

    // DELETE: api/todolists/5/todos/3
    [HttpDelete("{itemId}")]
    public async Task<ActionResult> DeleteTodoItem(long todoListId, long itemId)
    {
        var todoItem = await _context.TodoItems
            .FirstOrDefaultAsync(i => i.TodoListId == todoListId && i.ItemId == itemId);

        if (todoItem == null)
        {
            return NotFound();
        }

        _context.TodoItems.Remove(todoItem);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<bool> TodoListExists(long id)
    {
        return await _context.TodoList.AnyAsync(e => e.Id == id);
    }
} 
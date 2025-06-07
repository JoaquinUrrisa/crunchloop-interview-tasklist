using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Controllers;
using TodoApi.Models;

namespace TodoApi.Tests;

#nullable disable
public class TodoItemsControllerTests
{
    private DbContextOptions<TodoContext> DatabaseContextOptions()
    {
        return new DbContextOptionsBuilder<TodoContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    private void PopulateDatabaseContext(TodoContext context)
    {
        // Create a todo list
        var todoList = new TodoList { Id = 1, Name = "Test List" };
        context.TodoList.Add(todoList);

        // Add some todo items
        context.TodoItems.Add(new TodoItem 
        { 
            TodoListId = 1, 
            ItemId = 1, 
            Description = "Item 1", 
            Completed = false 
        });
        context.TodoItems.Add(new TodoItem 
        { 
            TodoListId = 1, 
            ItemId = 2, 
            Description = "Item 2", 
            Completed = true 
        });
        context.SaveChanges();
    }

    [Fact]
    public async Task GetTodoItems_WhenCalled_ReturnsTodoItemsList()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoItemsController(context);

            var result = await controller.GetTodoItems(1);

            Assert.IsType<OkObjectResult>(result.Result);
            var items = (result.Result as OkObjectResult).Value as IList<TodoItem>;
            Assert.Equal(2, items.Count);
        }
    }

    [Fact]
    public async Task GetTodoItems_WhenTodoListDoesNotExist_ReturnsNotFound()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoItemsController(context);

            var result = await controller.GetTodoItems(999);

            Assert.IsType<NotFoundResult>(result.Result);
        }
    }

    [Fact]
    public async Task GetTodoItem_WhenCalled_ReturnsTodoItem()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoItemsController(context);

            var result = await controller.GetTodoItem(1, 1);

            Assert.IsType<OkObjectResult>(result.Result);
            var item = (result.Result as OkObjectResult).Value as dynamic;
            Assert.Equal(1, item.TodoListId);
            Assert.Equal(1, item.ItemId);
            Assert.Equal("Item 1", item.Description);
            Assert.False(item.Completed);
        }
    }

    [Fact]
    public async Task GetTodoItem_WhenItemDoesNotExist_ReturnsNotFound()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoItemsController(context);

            var result = await controller.GetTodoItem(1, 999);

            Assert.IsType<NotFoundResult>(result.Result);
        }
    }

    [Fact]
    public async Task PutTodoItem_WhenCalled_UpdatesTodoItem()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoItemsController(context);

            var result = await controller.PutTodoItem(
                1,
                1,
                new Dtos.UpdateTodoItem { Description = "Updated Item", Completed = true }
            );

            Assert.IsType<OkObjectResult>(result);
            var updatedItem = await context.TodoItems.FindAsync(1L, 1L);
            Assert.Equal("Updated Item", updatedItem.Description);
            Assert.True(updatedItem.Completed);
        }
    }

    [Fact]
    public async Task PutTodoItem_WhenItemDoesNotExist_ReturnsNotFound()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoItemsController(context);

            var result = await controller.PutTodoItem(
                1,
                999,
                new Dtos.UpdateTodoItem { Description = "Updated Item", Completed = true }
            );

            Assert.IsType<NotFoundResult>(result);
        }
    }

    [Fact]
    public async Task PostTodoItem_WhenCalled_CreatesTodoItem()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoItemsController(context);

            var result = await controller.PostTodoItem(
                1,
                new Dtos.CreateTodoItem { Description = "New Item", Completed = false }
            );

            Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(3, context.TodoItems.Count());
        }
    }

    [Fact]
    public async Task PostTodoItem_WhenTodoListDoesNotExist_ReturnsNotFound()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoItemsController(context);

            var result = await controller.PostTodoItem(
                999,
                new Dtos.CreateTodoItem { Description = "New Item", Completed = false }
            );

            Assert.IsType<NotFoundResult>(result.Result);
        }
    }

    [Fact]
    public async Task DeleteTodoItem_WhenCalled_RemovesTodoItem()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoItemsController(context);

            var result = await controller.DeleteTodoItem(1, 1);

            Assert.IsType<NoContentResult>(result);
            Assert.Equal(1, context.TodoItems.Count());
        }
    }

    [Fact]
    public async Task DeleteTodoItem_WhenItemDoesNotExist_ReturnsNotFound()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoItemsController(context);

            var result = await controller.DeleteTodoItem(1, 999);

            Assert.IsType<NotFoundResult>(result);
        }
    }

    [Fact]
    public async Task MarkTodoItemAsDone_WhenCalled_MarksItemAsDone()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoItemsController(context);

            var result = await controller.MarkTodoItemAsDone(1, 1);

            Assert.IsType<OkObjectResult>(result);
            var updatedItem = await context.TodoItems.FindAsync(1L, 1L);
            Assert.True(updatedItem.Completed);
        }
    }

    [Fact]
    public async Task MarkTodoItemAsDone_WhenItemDoesNotExist_ReturnsNotFound()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoItemsController(context);

            var result = await controller.MarkTodoItemAsDone(1, 999);

            Assert.IsType<NotFoundResult>(result);
        }
    }
} 
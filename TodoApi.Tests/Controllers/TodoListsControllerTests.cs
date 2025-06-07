using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Controllers;
using TodoApi.Models;

namespace TodoApi.Tests;

#nullable disable
public class TodoListsControllerTests
{
    private DbContextOptions<TodoContext> DatabaseContextOptions()
    {
        return new DbContextOptionsBuilder<TodoContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    private void PopulateDatabaseContext(TodoContext context)
    {
        // Create todo lists with items
        var todoList1 = new TodoList { Id = 1, Name = "Task 1" };
        var todoList2 = new TodoList { Id = 2, Name = "Task 2" };

        todoList1.Items.Add(new TodoItem 
        { 
            TodoListId = 1, 
            ItemId = 1, 
            Description = "Item 1", 
            Completed = false 
        });
        todoList1.Items.Add(new TodoItem 
        { 
            TodoListId = 1, 
            ItemId = 2, 
            Description = "Item 2", 
            Completed = true 
        });

        todoList2.Items.Add(new TodoItem 
        { 
            TodoListId = 2, 
            ItemId = 1, 
            Description = "Item 3", 
            Completed = false 
        });

        context.TodoList.Add(todoList1);
        context.TodoList.Add(todoList2);
        context.SaveChanges();
    }

    [Fact]
    public async Task GetTodoList_WhenCalled_ReturnsTodoListList()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoListsController(context);

            var result = await controller.GetTodoLists();

            Assert.IsType<OkObjectResult>(result.Result);
            var lists = (result.Result as OkObjectResult).Value as IList<TodoList>;
            Assert.Equal(2, lists.Count);
            
            // Verify items are included
            var firstList = lists[0];
            var itemsList = firstList.Items.ToList();
            Assert.Equal(2, itemsList.Count);
            Assert.Equal("Item 1", itemsList[0].Description);
            Assert.Equal("Item 2", itemsList[1].Description);
        }
    }

    [Fact]
    public async Task GetTodoList_WhenCalled_ReturnsTodoListById()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoListsController(context);

            var result = await controller.GetTodoList(1);

            Assert.IsType<OkObjectResult>(result.Result);
            var todoList = (result.Result as OkObjectResult).Value as dynamic;
            Assert.Equal(1, todoList.Id);
            Assert.Equal("Task 1", todoList.Name);
            Assert.Equal(2, todoList.Items.Count);
            Assert.Equal("Item 1", todoList.Items[0].Description);
            Assert.Equal("Item 2", todoList.Items[1].Description);
        }
    }

    [Fact]
    public async Task PutTodoList_WhenTodoListDoesntExist_ReturnsBadRequest()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoListsController(context);

            var result = await controller.PutTodoList(
                3,
                new Dtos.UpdateTodoList { Name = "Task 3" }
            );

            Assert.IsType<NotFoundResult>(result);
        }
    }

    [Fact]
    public async Task PutTodoList_WhenCalled_UpdatesTheTodoList()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoListsController(context);

            var todoList = await context.TodoList.Where(x => x.Id == 2).FirstAsync();
            var result = await controller.PutTodoList(
                todoList.Id,
                new Dtos.UpdateTodoList { Name = "Changed Task 2" }
            );

            Assert.IsType<OkObjectResult>(result);
            var updatedList = (result as OkObjectResult).Value as dynamic;
            Assert.Equal("Changed Task 2", updatedList.Name);
            Assert.Equal(1, updatedList.Items.Count);
        }
    }

    [Fact]
    public async Task PostTodoList_WhenCalled_CreatesTodoList()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoListsController(context);

            var result = await controller.PostTodoList(new Dtos.CreateTodoList { Name = "Task 3" });

            Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(3, context.TodoList.Count());
            var newList = await context.TodoList.FindAsync(3L);
            Assert.Empty(newList.Items);
        }
    }

    [Fact]
    public async Task DeleteTodoList_WhenCalled_RemovesTodoList()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            PopulateDatabaseContext(context);

            var controller = new TodoListsController(context);

            var result = await controller.DeleteTodoList(2);

            Assert.IsType<NoContentResult>(result);
            Assert.Equal(1, context.TodoList.Count());
            Assert.Equal(2, context.TodoItems.Count()); // Only items from list 1 remain
        }
    }

    [Fact]
    public async Task DeleteTodoList_CascadesToItems_RemovesAllItems()
    {
        using (var context = new TodoContext(DatabaseContextOptions()))
        {
            // Arrange
            PopulateDatabaseContext(context);
            var controller = new TodoListsController(context);

            // Act
            var result = await controller.DeleteTodoList(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            Assert.Null(await context.TodoList.FindAsync(1L));
            Assert.Empty(context.TodoItems.Where(i => i.TodoListId == 1));
        }
    }
}

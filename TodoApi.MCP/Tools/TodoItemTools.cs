using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;
using System.Net.Http.Json;
using TodoApi.Models;
using TodoApi.MCP.Extensions;

namespace TodoApi.MCP.Tools;

[McpServerToolType]
public static class TodoItemTools
{
    [McpServerTool, Description("Get all todo items in a todo list.")]
    public static async Task<string> GetTodoItems(
        HttpClient client,
        [Description("The ID of the todo list.")] long todoListId)
    {
        using var jsonDocument = await client.ReadJsonDocumentAsync($"/api/todolists/{todoListId}/todos");
        var items = jsonDocument.RootElement.EnumerateArray();

        if (!items.Any())
        {
            return "No todo items found in this list.";
        }

        return string.Join("\n---\n", items.Select(item => $"""
                List ID: {item.GetProperty("todoListId").GetInt64()}
                Item ID: {item.GetProperty("itemId").GetInt64()}
                Description: {item.GetProperty("description").GetString()}
                Completed: {item.GetProperty("completed").GetBoolean()}
                """));
    }

    [McpServerTool, Description("Get a specific todo item from a todo list.")]
    public static async Task<string> GetTodoItem(
        HttpClient client,
        [Description("The ID of the todo list.")] long todoListId,
        [Description("The ID of the todo item.")] long itemId)
    {
        using var jsonDocument = await client.ReadJsonDocumentAsync($"/api/todolists/{todoListId}/todos/{itemId}");
        var item = jsonDocument.RootElement;

        return $"""
                List ID: {item.GetProperty("todoListId").GetInt64()}
                Item ID: {item.GetProperty("itemId").GetInt64()}
                Description: {item.GetProperty("description").GetString()}
                Completed: {item.GetProperty("completed").GetBoolean()}
                """;
    }

    [McpServerTool, Description("Create a new todo item in a todo list.")]
    public static async Task<string> CreateTodoItem(
        HttpClient client,
        [Description("The ID of the todo list.")] long todoListId,
        [Description("The description of the todo item.")] string description,
        [Description("Whether the todo item is completed.")] bool completed = false)
    {
        var todoItem = new { description, completed };
        var response = await client.PostAsJsonAsync($"/api/todolists/{todoListId}/todos", todoItem);
        response.EnsureSuccessStatusCode();

        using var jsonDocument = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var item = jsonDocument.RootElement;

        return $"""
                Created new todo item:
                List ID: {item.GetProperty("todoListId").GetInt64()}
                Item ID: {item.GetProperty("itemId").GetInt64()}
                Description: {item.GetProperty("description").GetString()}
                Completed: {item.GetProperty("completed").GetBoolean()}
                """;
    }

    [McpServerTool, Description("Update an existing todo item in a todo list.")]
    public static async Task<string> UpdateTodoItem(
        HttpClient client,
        [Description("The ID of the todo list.")] long todoListId,
        [Description("The ID of the todo item to update.")] long itemId,
        [Description("The new description of the todo item.")] string description,
        [Description("Whether the todo item is completed.")] bool completed)
    {
        var todoItem = new { description, completed };
        var response = await client.PutAsJsonAsync($"/api/todolists/{todoListId}/todos/{itemId}", todoItem);
        response.EnsureSuccessStatusCode();

        using var jsonDocument = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var item = jsonDocument.RootElement;

        return $"""
                Updated todo item:
                List ID: {item.GetProperty("todoListId").GetInt64()}
                Item ID: {item.GetProperty("itemId").GetInt64()}
                Description: {item.GetProperty("description").GetString()}
                Completed: {item.GetProperty("completed").GetBoolean()}
                """;
    }

    [McpServerTool, Description("Delete a todo item from a todo list.")]
    public static async Task<string> DeleteTodoItem(
        HttpClient client,
        [Description("The ID of the todo list.")] long todoListId,
        [Description("The ID of the todo item to delete.")] long itemId)
    {
        var response = await client.DeleteAsync($"/api/todolists/{todoListId}/todos/{itemId}");
        response.EnsureSuccessStatusCode();

        return $"Successfully deleted todo item {itemId} from list {todoListId}.";
    }
} 
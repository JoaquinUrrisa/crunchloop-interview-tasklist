using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;
using System.Net.Http.Json;
using TodoApi.Models;
using TodoApi.MCP.Extensions;

namespace TodoApi.MCP.Tools;

[McpServerToolType]
public static class TodoListTools
{
    [McpServerTool, Description("Get all todo lists.")]
    public static async Task<string> GetTodoLists(
        HttpClient client)
    {
        using var jsonDocument = await client.ReadJsonDocumentAsync("/api/todolists");
        var lists = jsonDocument.RootElement.EnumerateArray();

        if (!lists.Any())
        {
            return "No todo lists found.";
        }

        return string.Join("\n---\n", lists.Select(list => $"""
                ID: {list.GetProperty("id").GetInt64()}
                Name: {list.GetProperty("name").GetString()}
                Items: {list.GetProperty("items").GetArrayLength()}
                """));
    }

    [McpServerTool, Description("Get a specific todo list by ID.")]
    public static async Task<string> GetTodoList(
        HttpClient client,
        [Description("The ID of the todo list.")] long id)
    {
        using var jsonDocument = await client.ReadJsonDocumentAsync($"/api/todolists/{id}");
        var list = jsonDocument.RootElement;
        var items = list.GetProperty("items").EnumerateArray();

        var itemsText = items.Any() 
            ? "\nItems:\n" + string.Join("\n", items.Select(item => $"""
                - {item.GetProperty("description").GetString()} (ID: {item.GetProperty("itemId").GetInt64()}, Completed: {item.GetProperty("completed").GetBoolean()})
                """))
            : "\nNo items in this list.";

        return $"""
                List Details:
                ID: {list.GetProperty("id").GetInt64()}
                Name: {list.GetProperty("name").GetString()}
                {itemsText}
                """;
    }

    [McpServerTool, Description("Create a new todo list.")]
    public static async Task<string> CreateTodoList(
        HttpClient client,
        [Description("The name of the todo list.")] string name)
    {
        var todoList = new { name };
        var response = await client.PostAsJsonAsync("/api/todolists", todoList);
        response.EnsureSuccessStatusCode();

        using var jsonDocument = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var list = jsonDocument.RootElement;

        return $"""
                Created new todo list:
                ID: {list.GetProperty("id").GetInt64()}
                Name: {list.GetProperty("name").GetString()}
                """;
    }

    [McpServerTool, Description("Update an existing todo list.")]
    public static async Task<string> UpdateTodoList(
        HttpClient client,
        [Description("The ID of the todo list to update.")] long id,
        [Description("The new name of the todo list.")] string name)
    {
        var todoList = new { name };
        var response = await client.PutAsJsonAsync($"/api/todolists/{id}", todoList);
        response.EnsureSuccessStatusCode();

        using var jsonDocument = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());
        var list = jsonDocument.RootElement;

        return $"""
                Updated todo list:
                ID: {list.GetProperty("id").GetInt64()}
                Name: {list.GetProperty("name").GetString()}
                """;
    }

    [McpServerTool, Description("Delete a todo list and all its items.")]
    public static async Task<string> DeleteTodoList(
        HttpClient client,
        [Description("The ID of the todo list to delete.")] long id)
    {
        var response = await client.DeleteAsync($"/api/todolists/{id}");
        response.EnsureSuccessStatusCode();

        return $"Successfully deleted todo list with ID {id} and all its items.";
    }
} 
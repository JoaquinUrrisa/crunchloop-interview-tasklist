# dotnet-interview / TodoApi

[![Open in Coder](https://dev.crunchloop.io/open-in-coder.svg)](https://dev.crunchloop.io/templates/fly-containers/workspace?param.Git%20Repository=git@github.com:crunchloop/dotnet-interview.git)

This is a simple Todo List API built in .NET 8. This project is currently being used for .NET full-stack candidates.

## Features

- Create, read, update and delete todo lists
- Create, read, update and delete todo items within lists
- Mark todo items as completed
- Model Context Protocol (MCP) server for API integration

## API Endpoints

### Todo Lists
- `GET /api/todolists` - Get all todo lists with their items
- `GET /api/todolists/{id}` - Get a specific todo list with its items
- `POST /api/todolists` - Create a new todo list
- `PUT /api/todolists/{id}` - Update a todo list
- `DELETE /api/todolists/{id}` - Delete a todo list and its items

### Todo Items
- `GET /api/todolists/{todoListId}/todos` - Get all items in a todo list
- `GET /api/todolists/{todoListId}/todos/{itemId}` - Get a specific todo item
- `POST /api/todolists/{todoListId}/todos` - Create a new todo item
- `PUT /api/todolists/{todoListId}/todos/{itemId}` - Update a todo item
- `PATCH /api/todolists/{todoListId}/todos/{itemId}/done` - Mark a todo item as completed
- `DELETE /api/todolists/{todoListId}/todos/{itemId}` - Delete a todo item

## Database

The project comes with a devcontainer that provisions a SQL Server database. If you are not going to use the devcontainer, make sure to provision a SQL Server database and
update the connection string.

## Build

To build the application:

`dotnet build`

## Run the API

To run the TodoApi in your local environment:

`dotnet run --project TodoApi`

## Test

To run tests:

`dotnet test`

Check integration tests at: (https://github.com/crunchloop/interview-tests)

## MCP Integration

The project includes a Model Context Protocol (MCP) server. The MCP tools are available in the `TodoApi.MCP` project and include:

### TodoList Tools
- Get all todo lists
- Get a specific todo list with its items
- Create a new todo list
- Update an existing todo list
- Delete a todo list and its items

### TodoItem Tools
- Get all items in a todo list
- Get a specific todo item
- Create a new todo item
- Update an existing todo item
- Delete a todo item

### Claude Integration

To integrate the MCP server with Claude Desktop:

1. Make sure the Todo API and the database are up:
```bash
dotnet run --project TodoApi
```

2. Configure Claude Desktop:
   - Open your Claude Desktop App configuration at:
     - macOS/Linux: `~/Library/Application Support/Claude/claude_desktop_config.json`
     - Windows: `%AppData%\Claude\claude_desktop_config.json`
   - Add the following configuration:
```json
{
  "mcpServers": {
    "todo-api": {
      "command": "dotnet",
      "args": [
        "run",
        "--project",
        "/ABSOLUTE/PATH/TO/TodoApi.MCP",
        "--no-build"
      ]
    }
  }
}
```

3. Restart Claude Desktop to apply the changes

Claude Desktop will automatically start the MCP server. The MCP server will be available in Claude Desktop, allowing you to interact with the Todo API using natural language. You can ask Claude to:
- List all todo lists
- Create new todo lists and items
- Update existing items
- Mark items as completed
- Delete lists and items

For more detailed instructions and troubleshooting, refer to the [MCP Server Quickstart Guide](https://modelcontextprotocol.io/quickstart/server#testing-your-server-with-claude-for-desktop-5).

## Contact

- Martín Fernández (mfernandez@crunchloop.io)

## About Crunchloop

![crunchloop](https://crunchloop.io/logo-blue.png)

We strongly believe in giving back :rocket:. Let's work together [`Get in touch`](https://crunchloop.io/contact).

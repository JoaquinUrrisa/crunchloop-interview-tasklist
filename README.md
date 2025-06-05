# dotnet-interview / TodoApi

[![Open in Coder](https://dev.crunchloop.io/open-in-coder.svg)](https://dev.crunchloop.io/templates/fly-containers/workspace?param.Git%20Repository=git@github.com:crunchloop/dotnet-interview.git)

This is a simple Todo List API built in .NET 8. This project is currently being used for .NET full-stack candidates.

## Features

- Create, read, update and delete todo lists
- Create, read, update and delete todo items within lists
- Mark todo items as completed

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

## Contact

- Martín Fernández (mfernandez@crunchloop.io)

## About Crunchloop

![crunchloop](https://crunchloop.io/logo-blue.png)

We strongly believe in giving back :rocket:. Let's work together [`Get in touch`](https://crunchloop.io/contact).

# Task Management API
---

Task Management API is a project built using Clean Architecture principles, designed to manage and organize tasks efficiently. The application is structured into layers, ensuring high maintainability, testability, and scalability. By following Clean Architecture, the project enables separation of concerns and ensures the independence of frameworks and libraries, making it adaptable and easy to modify or extend.

## Table of Contents

- [Task Management API](#task-management-api)
  - [Features](#features)
    - [Technical Features](#technical-features)
    - [Functional Features](#functional-features)
  - [Prerequisites](#prerequisites)
  - [Technologies](#technologies)
  - [Project Structure / Folder Structure](#project-structure--folder-structure)
  - [Entity Relationship Diagram](#entity-relationship-diagram)
  - [API Endpoints](#api-endpoints)
    - [Authentication](#authentication)
    - [User Management](#user-management)


## Features

### Technical Features

- **Clean Architecture**: Implements separation of concerns with distinct layers for Domain, Application, Infrastructure, and Presentation.
- **API Versioning**: Supports multiple API versions using the `Asp.Versioning` package for backward compatibility.
- **JWT Authentication**: Provides secure API access with JSON Web Tokens (JWT) for authentication and session management.
- **Entity Framework Core**: Database management with SQLite or other providers, including migration and advanced querying support.
- **Caching**: Integrates memory caching to enhance performance by reducing redundant operations.
- **Centralized Error Handling**: Middleware for consistent and meaningful error responses.
- **HATEOAS Support**: Adds navigational links to API responses for enhanced discoverability of resources.
- **Serilog Logging**: Configurable logging system with email notification support for critical errors.
- **Swagger Documentation**: Automatically generates interactive API documentation with JWT token integration.
- **Dependency Injection**: Leverages DI for improved modularity and easier testing.

### Functional Features

- **User Authentication**: 
  - User login and registration with secure JWT token generation.
- **User Authorization**: 
  - Role-based access control (RBAC) to enforce user permissions (e.g., Admin, User).
- **User Management**: 
  - Full CRUD functionality for managing users.
  - Admin approval required for new user registrations, if enabled.
- **Password Security**: 
  - Uses `BCrypt` for secure password hashing to prevent plaintext storage.
- **Pagination**: 
  - Supports paginated data responses for efficient resource usage.
- **Data Validation**: 
  - Ensures proper formatting for email and phone fields using strongly typed value objects.
- **Middleware**: 
  - Includes custom middleware for logging, validation, and token extraction.
- **Developer Tools**: 
  - Integrated Swagger UI for testing APIs and exploring different API versions.
  - Built-in database seeding and migration tools.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQLite or another configured database provider
- Visual Studio or another IDE with .NET support

## Technologies

This project is built with the following technologies:

- .NET 6.0
- ASP.NET Core
- Entity Framework Core
- Sqlite
- AutoMapper
- Swagger for API documentation


## Project Structure / Folder Structure

```
├── TaskManagement.Domain         # Core domain logic
├── TaskManagement.Application    # Business logic and services
├── TaskManagement.Infrastructure # Data access and external dependencies
└── TaskManagement.Presenter      # Presentation layer (API, Filters, Middleware)
```


```plaintext
/TaskManagement
│
├── /TaskManagement.Domain
│   ├── /Common
│   │   ├── /JWT
│   │   │   ├── IJwtHelper.cs          # Interface for JWT operations
│   │   │   ├── JwtData.cs             # JWT Data model
│   │   │   └── JwtSettings.cs         # JWT Settings model
│   │   └── IPaginatedList.cs          # Interface for paginated list
│   ├── /Entities
│   │   ├── User.cs                    # User entity model
│   │   └── /Shared
│   │       └── BaseEntity.cs          # Base class for common entity fields
│   ├── /Repositories
│   │   └── IUserRepository.cs         # Interface for User repository
│   ├── /ValueObjects
│   │   ├── Email.cs                   # Email value object (validation logic)
│   │   └── Phone.cs                   # Phone value object (validation logic)
│   └── TaskManagement.Domain.csproj
│
├── /TaskManagement.Application
│   ├── /DTOs
│   │   ├── /User
│   │   │   ├── Login.cs               # Login DTO
│   │   │   ├── Register.cs            # Register DTO
│   │   │   ├── UserCreateDto.cs       # User create DTO
│   │   │   ├── UserReadDto.cs         # User read DTO
│   │   │   └── UserUpdateDto.cs       # User update DTO
│   ├── /Services
│   │   └── UserService.cs             # Business logic for user operations
│   ├── /Utilities
│   │   ├── JwtHelper.cs               # Helper class for JWT operations
│   │   └── PaginatedList.cs           # Helper class for Paginated List
│   └── TaskManagement.Application.csproj
│
├── /TaskManagement.Presenter
│   ├── /Controllers
│   │   ├── UserController.cs          # API controller for user-related endpoints
│   ├── /RouteTransformer
│   │   └── SpinCaseTransformer.cs     # Model for handling error responses
│   └── TaskManagement.Presenter.csproj
│
├── /TaskManagement.Infrastructure
│   ├── /AppSettings
│   │   └── EmailConfig.cs             # Contains email configuration settings
│   ├── /DataContext
│   │   │   ├── AppDbContext.cs        # Defines the application's DbContext for EF Core 
│   │   │   ├── AutoMapperProfile.cs   # Contains mappings for AutoMapper configuration
│   ├── /Repositories
│   │   └── UserRepository.cs          # Implements data access logic
│   ├── appsettings.json               # Configuration file for app settings
│   ├── Program.cs                     # Contains the main entry point and app setup
│   └── TaskManagement.Infrastructure.csproj
│
├── /TaskManagement.Tests
│   ├── UserServiceTests.cs            # Unit tests for UserService
│   └── TaskManagement.Tests.csproj
└── TaskManagement.sln
```



```plaintext
CleanArchitecture/
├── TaskManagement.sln                # Solution file
├── src/                                 # Source files for the application
│   ├── TaskManagement.Api/           # Web API project
│   │   ├── Controllers/                  # API controllers
│   │   ├── DTOs/                         # Data Transfer Objects
│   │   ├── Middleware/                   # Custom middleware (e.g., error handling)
│   │   ├── Properties/                   # Project properties (e.g., launch settings)
│   │   ├── Program.cs                    # Entry point for the application
│   │   ├── Startup.cs                    # Application configuration and services
│   │   └── appsettings.json              # Application configuration settings
│   ├── TaskManagement.Application/     # Application layer
│   │   ├── Interfaces/                   # Service interfaces
│   │   ├── Services/                     # Service implementations
│   │   ├── DTOs/                         # Data Transfer Objects for the application layer
│   │   ├── Utilties/                     # Utility classes (e.g., JWT handling)
│   │   ├── Specifications/                # Business logic specifications
│   │   └── Validators/                   # Fluent validation rules
│   ├── TaskManagement.Domain/          # Domain layer
│   │   ├── Entities/                     # Domain entities
│   │   ├── Enums/                        # Domain enums
│   │   ├── Interfaces/                   # Domain interfaces (e.g., repositories)
│   │   └── ValueObjects/                 # Value objects
│   └── TaskManagement.Infrastructure/  # Infrastructure layer
│       ├── Data/                         # Database context and migrations
│       ├── Repositories/                 # Repository implementations
│       ├── Services/                     # External services (e.g., email, payment)
│       └── Migrations/                   # Entity Framework migrations
├── tests/                                # Test projects
│   ├── TaskManagement.Application.Tests/  # Unit tests for the Application layer
│   ├── TaskManagement.Domain.Tests/     # Unit tests for the Domain layer
│   └── TaskManagement.Infrastructure.Tests/  # Unit tests for the Infrastructure layer
└── README.md                             # Project documentation
```


## Entity Relationship Diagram

![Clean Architecture Template](https://github.com/ahmad-act/myproject/blob/main/ERD.png?raw=true)

## API Endpoints

### Authentication

- **POST /api/v1/users/register**: Registers a new user. Requires a username, password, first name, last name, and email.
  
  - **Parameters**:
     - `username` (required): The username of the new user.
     - `password` (required): The password for the new user.
     - `email` (required): The email address of the new user.

  - **Responses**:
     - `201`: User successfully registered.
     - `400`: Bad request (e.g., missing or invalid parameters).
     - `409`: Conflict (e.g., username or email already exists).
     - `500`: Internal server error.

  - **Authentication**: None required for registration.

- **POST /api/v1/users/login**: Logs in a user. Requires username and password. Returns a JWT token if successful.
  
  - **Parameters**:
     - `username` (required): The username of the user.
     - `password` (required): The password of the user.

  - **Responses**:
     - `200`: Login successful. Returns a JWT token.
     - `401`: Unauthorized (invalid credentials).
     - `400`: Bad request (e.g., missing username or password).
     - `500`: Internal server error.

  - **Authentication**: None required for login (login is for authentication).

- **POST /api/v1/users/logout**: Logs out a user by invalidating their current session or JWT token.

  - **Parameters**:
     - None.

  - **Responses**:
     - `200`: Logout successful.
     - `401`: Unauthorized (e.g., if the token is missing or invalid).
     - `500`: Internal server error.

  - **Authentication**: Valid JWT token required in the Authorization header (e.g., Bearer <token>).

### User Management

- **GET /api/v1/users**: Retrieves a list of users, with support for pagination, searching, and sorting.
  
  - **Parameters**:
     - `searchTerm` (optional): Filter users by search term.
     - `page` (optional): Page number for pagination.
     - `pageSize` (optional): Number of items per page.
     - `sortColumn` (optional): Column by which to sort.
     - `sortOrder` (optional): Sort direction (asc/desc).
     - **Version**: `v1` is mandatory.

  - **Responses**:
     - `200`: List of users retrieved successfully.
     - `404`: No users found.
     - `401`: Invalid token.
     - `500`: Internal server error.
  
  - **Authentication**: Required, roles: Admin, User. 

- **GET /api/v1/users/{id}**: Retrieves a specific user by ID.
  
  - **Parameters**:
     - `id` (required): The unique identifier of the user.
     - **Version**: `v1` is mandatory.

  - **Responses**:
     - `200`: User retrieved successfully.
     - `404`: User not found.
     - `401`: Invalid token.
     - `500`: Internal server error.

  - **Authentication**: Required, roles: Admin, User.

- **POST /api/v1/users**: Creates a new user.
  
  - **Parameters**:
     - `username` (required): The username of the new user.
     - `password` (required): The password of the new user.
     - `email` (required): The email of the new user.
     - `userType` (optional): The type of user (Admin, User, etc.).
     - **Version**: `v1` is mandatory.

  - **Responses**:
     - `201`: User created successfully.
     - `400`: Bad request (e.g., invalid data).
     - `401`: Invalid token.
     - `500`: Internal server error.

  - **Authentication**: Required, roles: Admin.

- **PUT /api/v1/users/{id}**: Updates an existing user.
  
  - **Parameters**:
     - `id` (required): The unique identifier of the user.
     - `username` (optional): The new username of the user.
     - `password` (optional): The new password of the user.
     - `email` (optional): The new email of the user.
     - `userType` (optional): The new type of user (Admin, User, etc.).
     - **Version**: `v1` is mandatory.

  - **Responses**:
     - `200`: User updated successfully.
     - `404`: User not found.
     - `400`: Bad request (e.g., invalid data).
     - `401`: Invalid token.
     - `500`: Internal server error.

  - **Authentication**: Required, roles: Admin.

- **DELETE /api/v1/users/{id}**: Deletes a user by ID.
  
  - **Parameters**:
     - `id` (required): The unique identifier of the user to be deleted.
     - **Version**: `v1` is mandatory.

  - **Responses**:
     - `200`: User deleted successfully.
     - `404`: User not found.
     - `401`: Invalid token.
     - `500`: Internal server error.

  - **Authentication**: Required, roles: Admin.

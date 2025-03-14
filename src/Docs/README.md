##Region Code Rules
- Private Params: Begin with "_" (_userId,_locationId,...)
- Public Params: Normal (userId, locationId,...)
- Use camel case for naming params'name (<EXAMPLE> : locationName)
- Always return type of API (Service,Controller): Acknowledgement, Acknowledgement<T>
- Controllers, Services,... must be inherit BaseClass (BaseController,BaseService,...)
- No magic params - meaning: convert all values to enums,settings,... (<Wrong>: i=> i.Type == "SomeValue" => <True>: i => i.Type == EnumType.SomeValue)
- Avoid double code => Write common functions
- 
- 
##EndRegion




MyAspNetCoreSolution/
│
├── MyAspNetCore.Web/           # ASP.NET Core Web Application (MVC or API)
│   ├── Controllers/            # Controllers for handling HTTP requests
│   ├── Views/                  # Views for rendering HTML (MVC only)
│   ├── Pages/                  # Razor Pages (if using Razor Pages)
│   ├── wwwroot/                # Static files (CSS, JS, images)
│   ├── Program.cs              # Entry point of the application
│   └── Startup.cs              # Application configuration and middleware
│
├── MyAspNetCore.Core/          # Core Library (Domain Layer)
│   ├── Entities/               # Domain entities and business models
│   ├── Interfaces/             # Interfaces for repositories and services
│   ├── Dtos/                   # Data Transfer Objects (DTOs)
│   ├── Enums/                  # Enumerations and constants
│   └── Exceptions/             # Custom exceptions for the domain
│
├── MyAspNetCore.Infrastructure/ # Infrastructure Layer (Data Access, Repositories)
│   ├── Data/                   # DbContext and Entity Framework configurations
│   ├── Repositories/           # Repository implementations
│   ├── Services/               # Infrastructure services (e.g., email, logging)
│   ├── Configuration/          # Configuration for services (e.g., DI setup)
│   └── Migrations/             # Entity Framework Core migrations
│
├── MyAspNetCore.Application/    # Application Layer (Business Logic)
│   ├── Services/               # Business services that use repositories
│   ├── Mappers/                # Mapping profiles for AutoMapper
│   ├── Validators/             # Validation logic for business rules
│   └── Interfaces/             # Service interfaces for dependency injection
│
└── MyAspNetCore.Tests/          # Test Project (Unit and Integration Tests)
    ├── UnitTests/              # Unit tests for individual components
    ├── IntegrationTests/       # Integration tests for end-to-end scenarios
    └── TestUtilities/          # Helper classes and utilities for testing

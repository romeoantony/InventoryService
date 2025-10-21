# Copilot Instructions for InventoryService

## Overview

This is an ASP.NET Core MVC application for managing products and shops, using Entity Framework Core with SQL Server. The project is organized by standard conventions, but has some project-specific patterns and workflows.

## Architecture

- **Controllers**: Handle HTTP requests and responses. Main controllers: `ProductsController`, `ShopsController`, `HomeController`.
- **Models**: Core domain entities: `Product`, `Shop` (many-to-many relationship).
- **ViewModels**: Used to pass data between controllers and views, e.g., `ProductViewModel`, `ShopAssignmentViewModel`.
- **Data**: `InventoryDbContext` configures EF Core and exposes `DbSet<Product>` and `DbSet<Shop>`.
- **Migrations**: Database schema changes tracked in `Migrations/`.
- **Views**: Razor views for each controller in `Views/`.
- **Configuration**: Connection strings and settings in `appsettings.json`.

## Key Patterns & Conventions

- **Entity Relationships**: Products and Shops are many-to-many. See `Product` and `Shop` models and the `ProductShop` join table in migrations.
- **ViewModels**: Use `ProductViewModel` to combine a `Product` with shop assignments for create/edit forms.
- **Routing**: Attribute routing is used for some actions (e.g., `[HttpGet("/Products")]`).
- **Dependency Injection**: Controllers receive `InventoryDbContext` via constructor injection.

## Developer Workflows

- **Build**: Run `dotnet build` in the project root.
- **Run**: Use `dotnet run` to start the development server.
- **Database Migrations**:
  - Add migration: `dotnet ef migrations add <MigrationName>`
  - Update database: `dotnet ef database update`
- **Configuration**: Update connection strings in `appsettings.json` or `appsettings.Development.json`.
- **No tests**: There are currently no automated tests in this codebase.

## Integration Points

- **Database**: SQL Server via EF Core. Connection string in `appsettings.json`.
- **Static Assets**: Served from `wwwroot/`.

## Examples

- To add a new product with shop assignments, see `ProductsController.Create` and `ProductViewModel`.
- To update the schema, add a migration and update the database as above.

## Project-Specific Notes

- The many-to-many relationship is managed via EF Core conventions and migrations, not explicit join entity classes.
- Views are strongly typed to ViewModels for create/edit forms.

---

For more details, see the relevant files in each directory. If you are unsure about a pattern, check the existing controllers and viewmodels for examples.

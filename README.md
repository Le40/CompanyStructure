# Company Structure REST API

REST API for managing company organizational structure and employees.

The API supports:
- Companies, Divisions, Projects, Departments, Employees
- Leaders for organization nodes
- Validation and business rules
- JWT authentication and role-based authorization
- SQL Server database storage
- Scalar API documentation
- TeaPie API tests
- xUnit integration tests
- GitHub Actions CI pipeline
- Docker support

## Notes

- API uses DTOs for request and response models.
- Service layer returns controlled results using `ServiceResult`.
- Request logging and centralized exception handling are implemented through custom middleware.
- Integration tests use custom test factories and fake authentication handlers for protected endpoint testing.

## Technologies

- .NET / ASP.NET Core Web API
- C#
- Entity Framework Core
- Microsoft SQL Server
- JWT Authentication
- Scalar
- TeaPie
- xUnit
- Docker

## Prerequisites

Recommended:
- Docker Desktop

For local development without Docker:

- .NET 10 SDK
- Microsoft SQL Server / SQL Server Express
- SQL Server Management Studio
- TeaPie CLI (optional)

## Authentication

The API uses JWT Bearer authentication.

Two roles are supported:
- `Admin`
- `User`

Example protected endpoints:

```csharp
[Authorize]
```

```csharp
[Authorize(Roles = "Admin")]
```

### Login endpoint

```http
POST /api/auth/login
```

Example request:

```json
{
  "username": "admin",
  "password": "admin123"
}
```

Example response:

```json
{
  "token": "jwt-token",
  "role": "Admin"
}
```

Test users:

| Username | Password | Role |
|---|---|---|
| admin | admin123 | Admin |
| user | user123 | User |


## Getting started
### 1. Clone repository
```powershell
git clone https://github.com/Le40/CompanyStructure
cd CompanyStructure
```
### Option A. Run with Docker, recommended

Make sure Docker Desktop is running.

From the solution root, run:

```powershell
docker compose up --build
```
The API will be available at:
```
http://localhost:8080/scalar
```
Docker Compose starts both:

the Web API
SQL Server database

EF Core migrations are applied automatically on startup.

To stop Docker containers

```powershell
docker compose down
```


### Option B. Run locally without Docker
### 2. Configure database connection
Open
```
CompanyStructure.WebAPI/appsettings.json
```
Check or update the connection string:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost\\SQLExpress;Database=CompanyStructureDB;Trusted_Connection=True;TrustServerCertificate=True;"
}
```
If your SQL Server instance is different, update the Server value.

### 3. Database setup
### Option 1: EF Core migrations
```powershell
dotnet ef database update --project .\CompanyStructure.Infrastructure --startup-project .\CompanyStructure.WebAPI
```
### Option 2: SQL Script
Open SQL Server Management Studio, connect to SQL Server, open:
```
Database/create-database.sql
```
Select the correct database/server and execute the script.

### 4. Run the API
From solution root:
```powershell
dotnet run --project .\CompanyStructure.WebAPI
```
By default, the API runs on:
```
http://localhost:8080
```

### API Documentation
```
http://localhost:8080/scalar
```
or https:
```
https://localhost:7150/scalar
```
### Running automated tests

```powershell
dotnet test
```



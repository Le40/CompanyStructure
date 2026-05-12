# Company Structure REST API

REST API for managing company organizational structure and employees.

The API supports:
- Companies, Divisions, Projects, Departments, Employees
- Leaders for organization nodes
- Basic validation and business rules
- SQL Server database storage
- Scalar API documentation
- TeaPie API tests
- xUnit integration tests
- GitHub Actions CI pipeline

## Notes
- API uses DTOs for request and response models.
- Service layer returns controlled results using ServiceResult.
- Authentication and authorization were not implemented because they were outside the assignment scope. In a production environment, endpoints managing employee data should be protected using JWT/OIDC authentication and role-based authorization.

## Technologies

- .NET / ASP.NET Core Web API
- C#
- Entity Framework Core
- Microsoft SQL Server
- Scalar
- TeaPie

## Prerequisites

Recommended:
- Docker Desktop

For local development without Docker:

- .NET 10 SDK
- Microsoft SQL Server / SQL Server Express
- SQL Server Management Studio
- TeaPie CLI, optional for running API tests

TeaPie installation:

```powershell
dotnet tool install -g TeaPie.Tool
```

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

### Running TeaPie tests
Tests are located in:
```
\Tests
```
Before running tests, reset test data. Open SQL Server Management Studio and execute:
```
database/reset-test-data.sql
```
Then run the API and execute tests from solution root:
```powershell
teapie test .\Tests
```
TeaPie uses:
```
Tests/env.json
```
to configure the base API URL. If your API runs on a different port, update:
```json
{
  "$shared": {
    "baseUrl": "http://localhost:8080"
  }
}
```



using CompanyStructure.Application.Employees.DTOs;
using CompanyStructure.Application.Nodes.DTOs;
using CompanyStructure.IntegrationTests.Helpers;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CompanyStructure.IntegrationTests;

public class CompanyTests
    : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly TestApiClient _apiClient;
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

    public CompanyTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _apiClient = new TestApiClient(_client);    
    }

    private record LoginResponse(string Token, string Role);
    private record ErrorResponse(string Code, string Message);

    private async Task AuthorizeAsAdminAsync()
    {
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login",
            new
            {
                Username = "admin",
                Password = "admin123"
            },
            cancellationToken: _cancellationToken);

        var login = await loginResponse.Content
            .ReadFromJsonAsync<LoginResponse>(
                cancellationToken: _cancellationToken);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", login!.Token);
    }

    [Fact]
    public async Task CreateDivision_MissingCompany_ShouldReturnNotFound()
    {
        var request = new CreateNodeRequest
        {
            Name = "IT Division",
            Code = "IT",
            LeaderId = null
        };

        var response = await _client.PostAsJsonAsync("/api/companies/999/divisions", request,
            cancellationToken: _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(
            cancellationToken: _cancellationToken);
        error!.Code.Should().Be("Company.NotFound");
    }

    [Fact]
    public async Task CreateDivision_WithValidLeader_ShouldReturnCreated()
    {
        var company = await _apiClient.CreateCompanyAsync();
        var employee = await _apiClient.CreateEmployeeAsync(company.Id);

        var request = new CreateNodeRequest
        {
            Name = "IT Division",
            Code = "IT",
            LeaderId = employee.Id
        };

        var response = await _client.PostAsJsonAsync($"/api/companies/{company.Id}/divisions", request,
            cancellationToken: _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<NodeResponse>(
            cancellationToken: _cancellationToken);

        body.Should().NotBeNull();
        body.Name.Should().Be("IT Division");
        body.Code.Should().Be("IT");
        body.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task CreateProject_MissingDivision_ShouldReturnNotFound()
    {
        var request = new CreateNodeRequest
        {
            Name = "IT Project",
            Code = "ITP",
            LeaderId = null
        };

        var response = await _client.PostAsJsonAsync("/api/divisions/999/projects", request,
            cancellationToken: _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(
            cancellationToken: _cancellationToken);
        error!.Code.Should().Be("Division.NotFound");
    }

    [Fact]
    public async Task CreateProject_InvalidLeader_ShouldReturnBadRequest()
    {
        var company1 = await _apiClient.CreateCompanyAsync();
        var employee1 = await _apiClient.CreateEmployeeAsync(company1.Id);

        var company2 = await _apiClient.CreateCompanyAsync();
        var employee2 = await _apiClient.CreateEmployeeAsync(company2.Id);

        var division = await _apiClient.CreateDivisionAsync(company2.Id);

        var request = new CreateNodeRequest
        {
            Name = "IT Project",
            Code = "ITP",
            LeaderId = employee1.Id
        };

        var response = await _client.PostAsJsonAsync($"/api/divisions/{division.Id}/projects", request,
            cancellationToken: _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(
            cancellationToken: _cancellationToken);
        error!.Code.Should().Be("Project.InvalidLeader");
    }

    [Fact]
    public async Task CreateEmployee_WhenCompanyDoesntExist_ShouldReturnNotFound()
    {
        var request = new CreateEmployeeRequest
        {
            Degree = "MSc",
            Name = "John",
            Surname = "Doe",
            Email = "jd@down.com",
            PhoneNumber = "+421900000000"
        };

        var response = await _client.PostAsJsonAsync("/api/companies/999/employees", request,
            cancellationToken: _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(
            cancellationToken: _cancellationToken);
        error!.Code.Should().Be("Company.NotFound");
    }

    // -------------------------
    // 1. COMPANY CORE TESTS
    // -------------------------

    [Fact]
    public async Task CreateCompany_ShouldReturnCreated()
    {
        var unique = Guid.NewGuid().ToString("N")[..8];

        var request = new CreateCompanyRequest
        {
            Name = $"Company {unique}",
            Code = $"C-{unique}"
        };

        var response = await _client.PostAsJsonAsync("/api/companies", request, _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<NodeResponse>(_cancellationToken);

        body.Should().NotBeNull();
        body!.Id.Should().BeGreaterThan(0);
        body.Name.Should().Be(request.Name);
        body.Code.Should().Be(request.Code);
    }

    [Fact]
    public async Task CreateCompany_WhenCodeAlreadyExists_ShouldReturnConflict()
    {
        var unique = Guid.NewGuid().ToString("N")[..8];

        var request = new CreateCompanyRequest
        {
            Name = $"Company {unique}",
            Code = $"C-{unique}"
        };

        await _client.PostAsJsonAsync("/api/companies", request, _cancellationToken);

        var response = await _client.PostAsJsonAsync("/api/companies", request, _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(_cancellationToken);
        error!.Code.Should().Be("Company.DuplicateCode");
    }

    [Fact]
    public async Task GetCompany_WhenMissing_ShouldReturnNotFound()
    {
        var response = await _client.GetAsync("/api/companies/999999", _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateCompany_ShouldReturnOk()
    {
        var company = await _apiClient.CreateCompanyAsync();

        var request = new UpdateNodeRequest
        {
            Name = "Updated Company",
            Code = $"UPD-{Guid.NewGuid().ToString("N")[..8]}"
        };

        var response = await _client.PutAsJsonAsync($"/api/companies/{company.Id}", request, _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<NodeResponse>(_cancellationToken);

        body.Should().NotBeNull();
        body!.Name.Should().Be(request.Name);
        body.Code.Should().Be(request.Code);
    }

    [Fact]
    public async Task DeleteCompany_ShouldReturnNoContent()
    {
        var company = await _apiClient.CreateCompanyAsync();

        var response = await _client.DeleteAsync($"/api/companies/{company.Id}", _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    // -------------------------
    // 2. EMPLOYEE CORE TESTS
    // -------------------------

    [Fact]
    public async Task CreateEmployee_ShouldReturnCreated()
    {
        var company = await _apiClient.CreateCompanyAsync();

        var request = new CreateEmployeeRequest
        {
            Name = "John",
            Surname = "Doe",
            Email = $"john.{Guid.NewGuid():N}@test.com",
            PhoneNumber = "+421900123456"
        };

        var response = await _client.PostAsJsonAsync(
            $"/api/companies/{company.Id}/employees",
            request,
            _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<EmployeeResponse>(_cancellationToken);

        body.Should().NotBeNull();
        body!.Id.Should().BeGreaterThan(0);
        body.Email.Should().Be(request.Email);
    }

    [Fact]
    public async Task CreateEmployee_WhenCompanyMissing_ShouldReturnNotFound()
    {
        var request = new CreateEmployeeRequest
        {
            Name = "John",
            Surname = "Doe",
            Email = $"john.{Guid.NewGuid():N}@test.com",
            PhoneNumber = "+421900123456"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/companies/999999/employees",
            request,
            _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateEmployee_WhenEmailAlreadyExists_ShouldReturnConflict()
    {
        var company = await _apiClient.CreateCompanyAsync();
        var email = $"john.{Guid.NewGuid():N}@test.com";

        await _apiClient.CreateEmployeeAsync(company.Id, email);

        var request = new CreateEmployeeRequest
        {
            Name = "John",
            Surname = "Duplicate",
            Email = email,
            PhoneNumber = "+421900123456"
        };

        var response = await _client.PostAsJsonAsync(
            $"/api/companies/{company.Id}/employees",
            request,
            _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(_cancellationToken);
        error!.Code.Should().Be("Employee.DuplicateEmail");
    }

    [Fact]
    public async Task CreateEmployee_WithInvalidEmail_ShouldReturnBadRequest()
    {
        var company = await _apiClient.CreateCompanyAsync();

        var request = new CreateEmployeeRequest
        {
            Name = "John",
            Surname = "Doe",
            Email = "not-valid-email",
            PhoneNumber = "+421900123456"
        };

        var response = await _client.PostAsJsonAsync(
            $"/api/companies/{company.Id}/employees",
            request,
            _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteEmployee_WhenEmployeeIsLeader_ShouldSetLeaderToNull()
    {
        var company = await _apiClient.CreateCompanyAsync();
        var employee = await _apiClient.CreateEmployeeAsync(company.Id);
        var division = await _apiClient.CreateDivisionAsync(company.Id, employee.Id);

        var response = await _client.DeleteAsync($"/api/employees/{employee.Id}", _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getDivisionResponse = await _client.GetAsync($"/api/divisions/{division.Id}", _cancellationToken);
        getDivisionResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await getDivisionResponse.Content.ReadFromJsonAsync<NodeResponse>(_cancellationToken);

        body.Should().NotBeNull();

        // Adjust this depending on your response DTO.
        body!.LeaderId.Should().BeNull();
    }

    // -------------------------
    // 3. HIERARCHY CREATE / ERROR TESTS
    // -------------------------

    [Fact]
    public async Task CreateDivision_ShouldReturnCreated()
    {
        var company = await _apiClient.CreateCompanyAsync();
        var employee = await _apiClient.CreateEmployeeAsync(company.Id);

        var unique = Guid.NewGuid().ToString("N")[..8];

        var request = new CreateNodeRequest
        {
            Name = $"Division {unique}",
            Code = $"DIV-{unique}",
            LeaderId = employee.Id
        };

        var response = await _client.PostAsJsonAsync(
            $"/api/companies/{company.Id}/divisions",
            request,
            _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await response.Content.ReadFromJsonAsync<NodeResponse>(_cancellationToken);

        body.Should().NotBeNull();
        body!.Name.Should().Be(request.Name);
        body.Code.Should().Be(request.Code);
    }

    [Fact]
    public async Task CreateDivision_WhenCompanyMissing_ShouldReturnNotFound()
    {
        var request = new CreateNodeRequest
        {
            Name = "Division",
            Code = $"DIV-{Guid.NewGuid().ToString("N")[..8]}",
            LeaderId = null
        };

        var response = await _client.PostAsJsonAsync(
            "/api/companies/999999/divisions",
            request,
            _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateProject_WhenDivisionMissing_ShouldReturnNotFound()
    {
        var request = new CreateNodeRequest
        {
            Name = "Project",
            Code = $"PRJ-{Guid.NewGuid().ToString("N")[..8]}",
            LeaderId = null
        };

        var response = await _client.PostAsJsonAsync(
            "/api/divisions/999999/projects",
            request,
            _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateDepartment_WhenProjectMissing_ShouldReturnNotFound()
    {
        var request = new CreateNodeRequest
        {
            Name = "Department",
            Code = $"DEP-{Guid.NewGuid().ToString("N")[..8]}",
            LeaderId = null
        };

        var response = await _client.PostAsJsonAsync(
            "/api/projects/999999/departments",
            request,
            _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task CreateProject_WhenLeaderFromDifferentCompany_ShouldReturnBadRequest()
    {
        var company1 = await _apiClient.CreateCompanyAsync();
        var wrongLeader = await _apiClient.CreateEmployeeAsync(company1.Id);

        var company2 = await _apiClient.CreateCompanyAsync();
        var validLeader = await _apiClient.CreateEmployeeAsync(company2.Id);
        var division = await _apiClient.CreateDivisionAsync(company2.Id, validLeader.Id);

        var request = new CreateNodeRequest
        {
            Name = "Project",
            Code = $"PRJ-{Guid.NewGuid().ToString("N")[..8]}",
            LeaderId = wrongLeader.Id
        };

        var response = await _client.PostAsJsonAsync(
            $"/api/divisions/{division.Id}/projects",
            request,
            _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(_cancellationToken);
        error!.Code.Should().Be("Project.InvalidLeader");
    }

    [Fact]
    public async Task CreateDivision_WhenCodeAlreadyExists_ShouldReturnConflict()
    {
        var company = await _apiClient.CreateCompanyAsync();

        var code = $"DIV-{Guid.NewGuid().ToString("N")[..8]}";

        await _apiClient.CreateDivisionAsync(company.Id, code: code);

        var request = new CreateNodeRequest
        {
            Name = "Duplicate Division",
            Code = code,
            LeaderId = null
        };

        var response = await _client.PostAsJsonAsync(
            $"/api/companies/{company.Id}/divisions",
            request,
            _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>(_cancellationToken);
        error!.Code.Should().Be("Division.DuplicateCode");
    }

    // -------------------------
    // 4. READ / UPDATE / DELETE REPRESENTATIVE TESTS
    // -------------------------

    [Fact]
    public async Task GetCompanyStructure_ShouldReturnHierarchy()
    {
        var company = await _apiClient.CreateCompanyAsync();
        var employee = await _apiClient.CreateEmployeeAsync(company.Id);
        var division = await _apiClient.CreateDivisionAsync(company.Id, employee.Id);
        var project = await _apiClient.CreateProjectAsync(division.Id, employee.Id);
        var department = await _apiClient.CreateDepartmentAsync(project.Id, employee.Id);

        var response = await _client.GetAsync($"/api/companies/{company.Id}/structure", _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<CompanyStructureResponse>(_cancellationToken);

        body.Should().NotBeNull();

        body!.Id.Should().Be(company.Id);
        body.Name.Should().Be(company.Name);
        body.Code.Should().Be(company.Code);

        body.Divisions.Should().ContainSingle();

        var returnedDivision = body.Divisions.Single();
        returnedDivision.Id.Should().Be(division.Id);
        returnedDivision.Name.Should().Be(division.Name);
        returnedDivision.Code.Should().Be(division.Code);

        returnedDivision.Projects.Should().ContainSingle();

        var returnedProject = returnedDivision.Projects.Single();
        returnedProject.Id.Should().Be(project.Id);
        returnedProject.Name.Should().Be(project.Name);
        returnedProject.Code.Should().Be(project.Code);

        returnedProject.Departments.Should().ContainSingle();

        var returnedDepartment = returnedProject.Departments.Single();
        returnedDepartment.Id.Should().Be(department.Id);
        returnedDepartment.Name.Should().Be(department.Name);
        returnedDepartment.Code.Should().Be(department.Code);
    }

    [Fact]
    public async Task UpdateDivision_ShouldReturnOk()
    {
        var company = await _apiClient.CreateCompanyAsync();
        var division = await _apiClient.CreateDivisionAsync(company.Id);

        var unique = Guid.NewGuid().ToString("N")[..8];

        var request = new UpdateNodeRequest
        {
            Name = $"Updated Division {unique}",
            Code = $"UDIV-{unique}",
            LeaderId = null
        };

        var response = await _client.PutAsJsonAsync(
            $"/api/divisions/{division.Id}",
            request,
            _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<NodeResponse>(_cancellationToken);

        body.Should().NotBeNull();
        body!.Name.Should().Be(request.Name);
        body.Code.Should().Be(request.Code);
    }

    [Fact]
    public async Task DeleteProject_ShouldReturnNoContent()
    {
        var company = await _apiClient.CreateCompanyAsync();
        var division = await _apiClient.CreateDivisionAsync(company.Id);
        var project = await _apiClient.CreateProjectAsync(division.Id);

        var response = await _client.DeleteAsync($"/api/projects/{project.Id}", _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetMissingNode_ShouldReturnNotFound()
    {
        var response = await _client.GetAsync("/api/divisions/999999", _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    // -------------------------
    // 5. VALIDATION TESTS
    // -------------------------

    [Fact]
    public async Task CreateCompany_WithMissingName_ShouldReturnBadRequest()
    {
        var request = new CreateCompanyRequest
        {
            Name = "",
            Code = $"C-{Guid.NewGuid().ToString("N")[..8]}"
        };

        var response = await _client.PostAsJsonAsync("/api/companies", request, _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateEmployee_WithMissingSurname_ShouldReturnBadRequest()
    {
        var company = await _apiClient.CreateCompanyAsync();

        var request = new CreateEmployeeRequest
        {
            Name = "John",
            Surname = "",
            Email = $"john.{Guid.NewGuid():N}@test.com",
            PhoneNumber = "+421900123456"
        };

        var response = await _client.PostAsJsonAsync(
            $"/api/companies/{company.Id}/employees",
            request,
            _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task CreateDivision_WithMissingCode_ShouldReturnBadRequest()
    {
        var company = await _apiClient.CreateCompanyAsync();

        var request = new CreateNodeRequest
        {
            Name = "Division",
            Code = "",
            LeaderId = null
        };

        var response = await _client.PostAsJsonAsync(
            $"/api/companies/{company.Id}/divisions",
            request,
            _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

}

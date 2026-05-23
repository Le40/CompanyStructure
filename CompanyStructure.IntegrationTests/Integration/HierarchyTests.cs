using CompanyStructure.Application.Employees.DTOs;
using CompanyStructure.Application.Nodes.DTOs;
using CompanyStructure.IntegrationTests.Factories;
using CompanyStructure.IntegrationTests.Helpers;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CompanyStructure.IntegrationTests;

public class HierarchyTests : IClassFixture<AuthenticatedCustomWebApplicationFactory>
{
    private readonly AuthenticatedCustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly TestApiClient _apiClient;
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

    public HierarchyTests()
    {
        _factory = new AuthenticatedCustomWebApplicationFactory();
        _client = _factory.CreateClient();
        _apiClient = new TestApiClient(_client);
    }

    private record LoginResponse(string Token, string Role);
    private record ErrorResponse(string Code, string Message);


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


}
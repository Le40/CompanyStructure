using CompanyStructure.Application.Employees.DTOs;
using CompanyStructure.Application.Nodes.DTOs;
using CompanyStructure.IntegrationTests.Factories;
using CompanyStructure.IntegrationTests.Helpers;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CompanyStructure.IntegrationTests;

public class RUDTests : IClassFixture<AuthenticatedCustomWebApplicationFactory>
{
    private readonly AuthenticatedCustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly TestApiClient _apiClient;
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

    public RUDTests(AuthenticatedCustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _factory = factory;
        _apiClient = new TestApiClient(_client);
    }
    private record LoginResponse(string Token, string Role);
    private record ErrorResponse(string Code, string Message);

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
}

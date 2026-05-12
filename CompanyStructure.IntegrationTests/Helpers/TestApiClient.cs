using CompanyStructure.Application.Employees;
using CompanyStructure.Application.Employees.DTOs;
using CompanyStructure.Application.Nodes;
using CompanyStructure.Application.Nodes.DTOs;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace CompanyStructure.IntegrationTests.Helpers;

public class TestApiClient
{
    private readonly HttpClient _client;

    public TestApiClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<NodeResponse> CreateCompanyAsync(
        string? name = null,
        string? code = null)
    {
        var unique = Guid.NewGuid().ToString("N")[..8];

        var request = new CreateCompanyRequest
        {
            Name = name ?? $"Company {unique}",
            Code = code ?? $"C-{unique}"
        };

        var response = await _client.PostAsJsonAsync("/api/companies",request,
            cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var company = await response.Content.ReadFromJsonAsync<NodeResponse>(
            cancellationToken: TestContext.Current.CancellationToken);

        company.Should().NotBeNull();

        return company!;
    }

    public async Task<EmployeeResponse> CreateEmployeeAsync(int companyId, string? email = null)
    {
        var unique = Guid.NewGuid().ToString("N")[..8];

        var request = new CreateEmployeeRequest
        {
            Name = "John",
            Surname = "Doe",
            Email = email ?? $"john.doe.{unique}@test.com",
            PhoneNumber = "+421900123456"
        };

        var response = await _client.PostAsJsonAsync($"/api/companies/{companyId}/employees",
            request,
            cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var employee = await response.Content.ReadFromJsonAsync<EmployeeResponse>(
            cancellationToken: TestContext.Current.CancellationToken);

        employee.Should().NotBeNull();

        return employee!;
    }

    public async Task<NodeResponse> CreateDivisionAsync(
        int companyId,
        int? leaderId = null,
        string? name = null,
        string? code = null)
    {
        var unique = Guid.NewGuid().ToString("N")[..8];

        var request = new CreateNodeRequest
        {
            Name = name ?? $"IT Division {unique}",
            Code = code ?? $"DIV-{unique}",
            LeaderId = leaderId
        };

        var response = await _client.PostAsJsonAsync(
            $"/api/companies/{companyId}/divisions",
            request,
            cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var division = await response.Content.ReadFromJsonAsync<NodeResponse>(
            cancellationToken: TestContext.Current.CancellationToken);

        division.Should().NotBeNull();

        return division!;
    }

    public async Task<NodeResponse> CreateProjectAsync(
        int divisionId,
        int? leaderId = null,
        string? name = null,
        string? code = null)
    {
        var unique = Guid.NewGuid().ToString("N")[..8];

        var request = new CreateNodeRequest
        {
            Name = name ?? $"ITP Project {unique}",
            Code = code ?? $"PRJ-{unique}",
            LeaderId = leaderId
        };

        var response = await _client.PostAsJsonAsync(
            $"/api/divisions/{divisionId}/projects",
            request,
            cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var project = await response.Content.ReadFromJsonAsync<NodeResponse>(
            cancellationToken: TestContext.Current.CancellationToken);

        project.Should().NotBeNull();

        return project!;
    }

    public async Task<NodeResponse> CreateDepartmentAsync(
        int projectId,
        int? leaderId = null,
        string? name = null,
        string? code = null)
    {
        var unique = Guid.NewGuid().ToString("N")[..8];

        var request = new CreateNodeRequest
        {
            Name = name ?? $"IT Department {unique}",
            Code = code ?? $"DEP-{unique}",
            LeaderId = leaderId
        };

        var response = await _client.PostAsJsonAsync(
            $"/api/projects/{projectId}/departments",
            request,
            cancellationToken: TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var department = await response.Content.ReadFromJsonAsync<NodeResponse>(
            cancellationToken: TestContext.Current.CancellationToken);

        department.Should().NotBeNull();

        return department!;
    }
}

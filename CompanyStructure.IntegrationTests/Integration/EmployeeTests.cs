using CompanyStructure.Application.Employees.DTOs;
using CompanyStructure.Application.Nodes.DTOs;
using CompanyStructure.IntegrationTests.Factories;
using CompanyStructure.IntegrationTests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CompanyStructure.IntegrationTests;

public class EmployeeTests : IClassFixture<AuthenticatedCustomWebApplicationFactory>
{
    private readonly AuthenticatedCustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly TestApiClient _apiClient;
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

    public EmployeeTests(AuthenticatedCustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _factory = factory;
        _apiClient = new TestApiClient(_client);
    }

    private record LoginResponse(string Token, string Role);
    private record ErrorResponse(string Code, string Message);


    [Fact]
    public async Task CreateEmployee_ShouldReturnCreated()
    {
        using var scope = _factory.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<TestDataSeeder>();

        var seed = await seeder.SeedCompanyAsync(cancellationToken: _cancellationToken);
        //var company = await _apiClient.CreateCompanyAsync();

        var request = new CreateEmployeeRequest
        {
            Name = "John",
            Surname = "Doe",
            Email = $"john.{Guid.NewGuid():N}@test.com",
            PhoneNumber = "+421900123456"
        };

        var response = await _client.PostAsJsonAsync(
            $"/api/companies/{seed.Id}/employees",
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
        using var scope = _factory.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<TestDataSeeder>();

        var company = await seeder.SeedCompanyAsync(cancellationToken: _cancellationToken);
        var employee = await seeder.SeedEmployeeAsync(company.Id,cancellationToken: _cancellationToken);

        //var company = await _apiClient.CreateCompanyAsync();
        //var email = $"john.{Guid.NewGuid():N}@test.com";

        //await _apiClient.CreateEmployeeAsync(company.Id, email);

        var request = new CreateEmployeeRequest
        {
            Name = "John",
            Surname = "Duplicate",
            Email = employee.Email,
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
        using var scope = _factory.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<TestDataSeeder>();

        var company = await seeder.SeedCompanyAsync(cancellationToken: _cancellationToken);

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
        using var scope = _factory.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<TestDataSeeder>();

        var division = await seeder.SeedDivisionAsync(cancellationToken: _cancellationToken);
        var employee = await seeder.SeedEmployeeAsync(division.CompanyId, cancellationToken: _cancellationToken);

        /*var company = await _apiClient.CreateCompanyAsync();
        var employee = await _apiClient.CreateEmployeeAsync(company.Id);
        var division = await _apiClient.CreateDivisionAsync(company.Id, employee.Id);*/

        var response = await _client.DeleteAsync($"/api/employees/{employee.Id}", _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var getDivisionResponse = await _client.GetAsync($"/api/divisions/{division.Id}", _cancellationToken);
        getDivisionResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await getDivisionResponse.Content.ReadFromJsonAsync<NodeResponse>(_cancellationToken);

        body.Should().NotBeNull();

        // Adjust this depending on your response DTO.
        body!.LeaderId.Should().BeNull();
    }

}
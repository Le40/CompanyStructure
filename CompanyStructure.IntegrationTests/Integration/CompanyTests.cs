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

public class CompanyTests : IClassFixture<AuthenticatedCustomWebApplicationFactory>
{
    private readonly AuthenticatedCustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly TestApiClient _apiClient;
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

    public CompanyTests(AuthenticatedCustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _factory = factory;
        _apiClient = new TestApiClient(_client);
    }

    private record LoginResponse(string Token, string Role);
    private record ErrorResponse(string Code, string Message);

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
        using var scope = _factory.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<TestDataSeeder>();

        string testCode = "AAAA";
        var seed = await seeder.SeedCompanyAsync(code: testCode, cancellationToken: _cancellationToken);

        var request = new CreateCompanyRequest
        {
            Name = "Company",
            Code = testCode
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
        using var scope = _factory.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<TestDataSeeder>();

        var seed = await seeder.SeedCompanyAsync(cancellationToken: _cancellationToken);

        var request = new UpdateNodeRequest
        {
            Name = "Updated Company",
            Code = seed.Code
        };

        var response = await _client.PutAsJsonAsync($"/api/companies/{seed.Id}", request, _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await response.Content.ReadFromJsonAsync<NodeResponse>(_cancellationToken);

        body.Should().NotBeNull();
        body!.Name.Should().Be(request.Name);
        body.Code.Should().Be(request.Code);
    }

    [Fact]
    public async Task DeleteCompany_ShouldReturnNoContent()
    {
        using var scope = _factory.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<TestDataSeeder>();

        var seed = await seeder.SeedCompanyAsync(cancellationToken: _cancellationToken);

        var response = await _client.DeleteAsync($"/api/companies/{seed.Id}", _cancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}

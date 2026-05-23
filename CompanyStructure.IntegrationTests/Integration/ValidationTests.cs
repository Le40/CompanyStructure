using CompanyStructure.Application.Employees.DTOs;
using CompanyStructure.Application.Nodes.DTOs;
using CompanyStructure.IntegrationTests.Helpers;
using FluentAssertions;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CompanyStructure.IntegrationTests;

public class ValidationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;
    private readonly TestApiClient _apiClient;
    private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

    public ValidationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        _factory = factory;
        _apiClient = new TestApiClient(_client);
    }

    private record LoginResponse(string Token, string Role);
    private record ErrorResponse(string Code, string Message);

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

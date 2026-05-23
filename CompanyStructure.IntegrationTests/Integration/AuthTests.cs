using CompanyStructure.IntegrationTests.Factories;
using CompanyStructure.IntegrationTests.Helpers;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

namespace CompanyStructure.IntegrationTests.Integration
{
    public class AuthTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;
        private readonly TestApiClient _apiClient;
        private readonly CancellationToken _cancellationToken = TestContext.Current.CancellationToken;

        public AuthTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
            _factory = factory;
            _apiClient = new TestApiClient(_client);
        }

        [Fact]
        public async Task ProtectedEndpoint_WithoutToken_ReturnsUnauthorized()
        {
            var response = await _client.DeleteAsync(
                "/api/companies/1",
                _cancellationToken);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task PublicEndpoint_WithoutToken_ReturnsOK()
        {
            var response = await _client.GetAsync(
                "/api/companies",
                _cancellationToken);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Login_WithAdminCredentials_ReturnsToken()
        {
            var response = await _client.PostAsJsonAsync(
                "/api/auth/login",
                new { Username = "admin", Password = "admin123" },
                _cancellationToken);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
        {
            var response = await _client.PostAsJsonAsync(
                "/api/auth/login",
                new { Username = "wrong", Password = "wrong" },
                _cancellationToken);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task AdminOnlyEndpoint_WithUserToken_ReturnsForbidden()
        {
            var token = await LoginAndGetTokenAsync("user", "user123");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.DeleteAsync(
                "/api/companies/1",
                _cancellationToken);

            response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task UserEndpoint_WithUserToken_DoesNotReturnForbidden()
        {
            var token = await LoginAndGetTokenAsync("user", "user123");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.DeleteAsync(
                "/api/employees",
                _cancellationToken);

            response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
            response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task AdminOnlyEndpoint_WithAdminToken_DoesNotReturnForbidden()
        {
            var token = await LoginAndGetTokenAsync("admin", "admin123");

            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await _client.DeleteAsync(
                "/api/companies/1",
                _cancellationToken);

            response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
            response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden);
        }

        private async Task<string> LoginAndGetTokenAsync(string username, string password)
        {
            var response = await _client.PostAsJsonAsync(
                "/api/auth/login",
                new { Username = username, Password = password },
                _cancellationToken);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<LoginResponse>(
                _cancellationToken);

            return body!.Token;
        }

        private record LoginResponse(string Token, string Role);
    }
}

namespace GamersWorld.WebApp.Services;

using GamersWorld.Domain.Dtos;
using GamersWorld.WebApp.Models;

public class IdentityServiceClient(HttpClient httpClient, ILogger<IdentityServiceClient> logger)
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly ILogger<IdentityServiceClient> _logger = logger;
    public async Task<GetTokenResponse> GetToken(LoginDto loginDto)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/login/", loginDto);
        _logger.LogInformation("Login response status code {StatusCode}", response.StatusCode);
        if (!response.IsSuccessStatusCode)
        {
            return null; //TODO@buraksenyurt It dosen't make sense. Use strongly return type.
        }
        var gtResponse = await response.Content.ReadFromJsonAsync<GetTokenResponse>();
        return gtResponse;
    }
}

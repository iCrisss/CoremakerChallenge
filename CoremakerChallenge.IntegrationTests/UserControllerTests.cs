using CoremakerChallenge.Models.Api;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Xunit;

namespace CoremakerChallenge.IntegrationTests;

public class UserControllerTests
{
    [Fact]
    public async Task Register_Login_GetUserDetailsFlow()
    {
        var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var registerRequest = new RegisterRequest
        {
            Name = "name",
            Email = "c@c.com",
            Password = "password"
        };
        var registerResponse = await client.PostAsJsonAsync("api/user/register", registerRequest);
        Assert.True(registerResponse.IsSuccessStatusCode);

        var loginRequest = new LoginRequest
        {
            Email = registerRequest.Email,
            Password = registerRequest.Password
        };
        var loginResponse = await client.PostAsJsonAsync("api/user/login", loginRequest);
        Assert.True(loginResponse.IsSuccessStatusCode);

        var loginResponseContent = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResponseContent.Token);
        var userDetailsResponse = await client.GetFromJsonAsync<UserDetailsResponse>("api/user");

        Assert.NotNull(userDetailsResponse);
        Assert.Equal(registerRequest.Name, userDetailsResponse.Name);
        Assert.Equal(registerRequest.Email, userDetailsResponse.Email);
    }
}

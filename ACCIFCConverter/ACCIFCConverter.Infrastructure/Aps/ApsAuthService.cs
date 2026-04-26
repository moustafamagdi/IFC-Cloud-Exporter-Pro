using System.Diagnostics;
using System.Net;
using System.Text;
using ACCIFCConverter.Domain.Contracts;
using Newtonsoft.Json.Linq;

namespace ACCIFCConverter.Infrastructure.Aps;

public sealed class ApsAuthService(HttpClient httpClient, string clientId, string clientSecret, string callbackUrl) : IApsAuthService
{
    private string? _accessToken;
    private string? _refreshToken;
    private DateTimeOffset _expiresAt;
    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(_accessToken) && _expiresAt > DateTimeOffset.UtcNow.AddMinutes(1);

    public async Task<bool> LoginAsync(CancellationToken cancellationToken = default)
    {
        var state = Guid.NewGuid().ToString("N");
        var authUrl = $"https://developer.api.autodesk.com/authentication/v2/authorize?response_type=code&client_id={Uri.EscapeDataString(clientId)}&redirect_uri={Uri.EscapeDataString(callbackUrl)}&scope=data:read data:write bucket:read bucket:create code:all&state={state}";
        Process.Start(new ProcessStartInfo(authUrl) { UseShellExecute = true });
        using var listener = new HttpListener();
        listener.Prefixes.Add(callbackUrl.EndsWith('/') ? callbackUrl : callbackUrl + '/');
        listener.Start();
        var context = await listener.GetContextAsync();
        var code = context.Request.QueryString["code"];
        var responseBytes = Encoding.UTF8.GetBytes("Login complete. You can close this browser tab.");
        context.Response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
        context.Response.Close();
        if (string.IsNullOrWhiteSpace(code)) return false;

        var form = new Dictionary<string, string>
        {
            ["grant_type"] = "authorization_code",
            ["code"] = code,
            ["redirect_uri"] = callbackUrl,
            ["client_id"] = clientId,
            ["client_secret"] = clientSecret
        };

        var tokenResponse = await httpClient.PostAsync("https://developer.api.autodesk.com/authentication/v2/token", new FormUrlEncodedContent(form), cancellationToken);
        tokenResponse.EnsureSuccessStatusCode();
        var tokenJson = JObject.Parse(await tokenResponse.Content.ReadAsStringAsync(cancellationToken));
        _accessToken = tokenJson.Value<string>("access_token");
        _refreshToken = tokenJson.Value<string>("refresh_token");
        _expiresAt = DateTimeOffset.UtcNow.AddSeconds(tokenJson.Value<int>("expires_in"));
        return true;
    }

    public Task LogoutAsync(CancellationToken cancellationToken = default)
    {
        _accessToken = null;
        _refreshToken = null;
        _expiresAt = DateTimeOffset.MinValue;
        return Task.CompletedTask;
    }

    public async Task<string?> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (IsAuthenticated) return _accessToken;
        if (string.IsNullOrWhiteSpace(_refreshToken)) return null;

        var form = new Dictionary<string, string>
        {
            ["grant_type"] = "refresh_token",
            ["refresh_token"] = _refreshToken,
            ["client_id"] = clientId,
            ["client_secret"] = clientSecret
        };

        var response = await httpClient.PostAsync("https://developer.api.autodesk.com/authentication/v2/token", new FormUrlEncodedContent(form), cancellationToken);
        response.EnsureSuccessStatusCode();
        var tokenJson = JObject.Parse(await response.Content.ReadAsStringAsync(cancellationToken));
        _accessToken = tokenJson.Value<string>("access_token");
        _refreshToken = tokenJson.Value<string>("refresh_token") ?? _refreshToken;
        _expiresAt = DateTimeOffset.UtcNow.AddSeconds(tokenJson.Value<int>("expires_in"));
        return _accessToken;
    }
}

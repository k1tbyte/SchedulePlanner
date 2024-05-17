using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SchedulePlanner.Tools;

internal static class Endpoints
{
    public static readonly Uri Base = new("http://localhost:5254/api/v1/");
    public const string Login = "auth/login";
    public const string Register = "auth/register";
    public const string RefreshSession = "auth/session/refresh";
    public const string TerminateSession = "auth/session/terminate";
}

public sealed class WebService : IDisposable
{
    private const string SessionFileName = "session.json";
    
    internal const byte MaxConnections        = 5;
    public readonly HttpClient Client;
    private readonly HttpClientHandler HttpClientHandler;
    public event Action? OnUnauthorized;
    private readonly JwtSecurityTokenHandler _jwtHandler = new();
    private Session? session;
    
    public WebService()
    {
        HttpClientHandler = new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
            MaxConnectionsPerServer = MaxConnections
        };

        Client = new HttpClient(HttpClientHandler, false);
        Client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/90.0.4430.212 Safari/537.36");
        Client.DefaultRequestHeaders.AcceptEncoding.ParseAdd("gzip, deflate");
        Client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-US,en;q=0.9");
        Client.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
        Client.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
    }

    private async Task<WebResult> _send<T>(HttpRequestMessage request, Action<T> onSuccess)
    {
        HttpResponseMessage response;
        try
        {
            response  = await Client.SendAsync(request);
        }
        catch (Exception e)
        {
            return new WebResult(false,null, HttpStatusCode.InternalServerError);
        }
        
        request.Dispose();
        if (!response.IsSuccessStatusCode)
        {
            return new WebResult(false, response.Content, response.StatusCode);
        }
        var result = await response.Content.ReadFromJsonAsync<T>(App.JsonOptions);
        onSuccess(result!);
        return new WebResult(true);
    }
    
    public Task<WebResult> Request<T>(
        HttpMethod method, string url, HttpContent? content, Action<T> onSuccess)
    {
        var request = new HttpRequestMessage(method, new Uri(Endpoints.Base, url));
        request.Content = content;
        return _send(request, onSuccess);
    }

    public async Task<WebResult> AuthorizedRequest<T>(
        HttpMethod method,string url, HttpContent content, Action<T> onSuccess)
    {
        if (session?.JwtAccess == null)
        {
            return new WebResult(false);
        }

        if (session.JwtAccess.ValidTo <= DateTime.Now + TimeSpan.FromSeconds(20))
        {
            var result = await RefreshSession();
            if (!result.Success)
            {
                OnUnauthorized?.Invoke();
                return result;
            }
        }
        
        using var request = new HttpRequestMessage(method, new Uri(Endpoints.Base,url) );
        request.Content = content;
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessToken);
        return await _send(request, onSuccess);
    }

    public async Task<WebResult> Authorize(string? username = null, string? password = null)
    {
        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            return await Request<Session>(HttpMethod.Post, Endpoints.Login,
                ToJson(new { username, password }), 
                StoreSession);
        }
        if (!File.Exists(SessionFileName) || 
            (session = JsonSerializer.Deserialize<Session>(await File.ReadAllTextAsync(SessionFileName))) == null ||
            !_jwtHandler.CanReadToken(session.AccessToken) ||
            !(await RefreshSession()).Success)
        {
            return new(false);
        }
        return new(true);
    }

    public static HttpContent ToJson(object obj) => 
        new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");

    private Task<WebResult> RefreshSession() =>
        Request<Session>(HttpMethod.Post, 
            Endpoints.RefreshSession + $"?refreshToken={session!.RefreshToken}",
            null, StoreSession);

    public void StoreSession(Session newSession)
    {
        session = newSession;
        session.JwtAccess = _jwtHandler.ReadToken(newSession.AccessToken) as JwtSecurityToken;
        File.WriteAllText(SessionFileName, JsonSerializer.Serialize(newSession));
    }
    
    public void Dispose()
    {
        HttpClientHandler.Dispose();
        Client.Dispose();
    }

    public static void HandleResult(WebResult result ,string byDefault, 
        Action onSuccess,
        Action<string> onFail)
    {
        if (result.Success)
        {
            onSuccess();
            return;
        }

        if (result.Code == null)
        {
            onFail(byDefault);
            return;
        }
        
        var code = (int)result.Code;
        if (code is >= 500 and < 600)
        {
            onFail("A server error has occurred");
            return;
        }
        onFail(byDefault);
    }

    public class Session
    {
        public required string AccessToken { get; init; }
        public required string RefreshToken { get; init; }
        
        [JsonIgnore]
        public JwtSecurityToken? JwtAccess { get; set; }
    }

    public record WebResult(bool Success, HttpContent? Content = null, HttpStatusCode? Code = null);
}
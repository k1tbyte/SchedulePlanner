using System;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reactive;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SchedulePlanner.Tools.Backend;

public sealed class WebService : IDisposable
{
    private const string SessionFileName = "session.json";
    
    internal const byte MaxConnections        = 5;
    public readonly HttpClient Client;
    private readonly HttpClientHandler HttpClientHandler;
    public event Action? OnUnauthorized;
    private readonly JwtSecurityTokenHandler _jwtHandler = new();
    public Session? CurrentSession { get; private set; }
    
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

    private async Task<WebResult> _send<T>(HttpRequestMessage httpRequest, BackendRequest<T?> req)
    {
        HttpResponseMessage response;
        try
        {
            response  = await Client.SendAsync(httpRequest);
        }
        catch (Exception)
        {
            req.OnFailed?.Invoke();
            return new WebResult(false,null, HttpStatusCode.InternalServerError);
        }
        
        httpRequest.Dispose();
        if (!response.IsSuccessStatusCode)
        {
            req.OnFailed?.Invoke();
            return new WebResult(false, response.Content, response.StatusCode);
        }

        if (typeof(T) == typeof(Unit))
        {
            req.OnSuccess?.Invoke(default);
            return new WebResult(true);
        }
            
        
        var result = await response.Content.ReadFromJsonAsync<T>(App.JsonOptions);
        req.OnSuccess?.Invoke(result!);

        return new WebResult(true);
    }
    
    public Task<WebResult> Request<T>(BackendRequest<T?> req)
    {
        var httpRequest = new HttpRequestMessage(req.Method, req.Uri);
        httpRequest.Content = req.Content;
        return _send(httpRequest, req);
    }

    public async Task<WebResult> AuthorizedRequest<T>(BackendRequest<T?> req)
    {
        if (CurrentSession?.JwtAccess == null)
        {
            return new WebResult(false);
        }

        if (CurrentSession.JwtAccess.ValidTo <= DateTime.UtcNow + TimeSpan.FromSeconds(20))
        {
            var result = await RefreshSession();
            if (!result.Success)
            {
                OnUnauthorized?.Invoke();
                return result;
            }
        }
        
        using var httpReq = new HttpRequestMessage(req.Method, req.Uri);
        httpReq.Content = req.Content;
        httpReq.Headers.Authorization = new AuthenticationHeaderValue("Bearer", CurrentSession.AccessToken);
        return await _send(httpReq, req);
    }

    public async Task<WebResult> Authorize(string? username = null, string? password = null)
    {
        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            return await Request<Session>(new (Endpoints.Login,HttpMethod.Post)
                { Content = ToJson(new { username, password }), OnSuccess  = StoreSession });
        }
        if (!File.Exists(SessionFileName) || 
            (CurrentSession = JsonSerializer.Deserialize<Session>(await File.ReadAllTextAsync(SessionFileName))) == null ||
            !_jwtHandler.CanReadToken(CurrentSession.AccessToken) ||
            !(await RefreshSession()).Success)
        {
            return new(false);
        }
        return new(true);
    }

    public async Task<WebResult> SignOut()
    {
        File.Delete(SessionFileName);
        return await AuthorizedRequest<Unit>(new(Endpoints.TerminateSession +
                                                 $"?refreshToken={CurrentSession!.RefreshToken}", HttpMethod.Delete));
    }

    public static HttpContent ToJson(object obj) => 
        new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");

    private Task<WebResult> RefreshSession() =>
        Request<Session>(new(Endpoints.RefreshSession + $"?refreshToken={CurrentSession!.RefreshToken}",
            HttpMethod.Post){ OnSuccess = StoreSession});

    public void StoreSession(Session? newSession)
    {
        CurrentSession = newSession ?? throw new ArgumentNullException(nameof(newSession));
        CurrentSession.JwtAccess = _jwtHandler.ReadToken(newSession.AccessToken) as JwtSecurityToken;
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
}
using System;
using System.Net.Http;

namespace SchedulePlanner.Tools.Backend;

public class BackendRequest<T>
{
    public BackendRequest(string url)
    {
        Url = url;
    }
    public BackendRequest(string url, HttpMethod method)
    {
        Url = url;
        Method = method;
    }


    public string Url { get; }
    public HttpMethod Method { get; } = HttpMethod.Get;
    public HttpContent? Content = null;
    public Action<T?>? OnSuccess { get; init; }
    public Action? OnFailed { get; init; }
    public Uri Uri => new(Endpoints.Base, Url);
}
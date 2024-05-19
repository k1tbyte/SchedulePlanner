using System.Net;
using System.Net.Http;

namespace SchedulePlanner.Tools.Backend;

public class WebResult(bool success, HttpContent? content = null, HttpStatusCode? code = null)
{
    public bool Success { get; init; } = success;
    public HttpContent? Content { get; init; } = content;
    public HttpStatusCode? Code { get; init; } = code;

    public static implicit operator bool(WebResult result) => result.Success;
}
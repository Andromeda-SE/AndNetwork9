using System;
using System.Net;

namespace AndNetwork9.Shared.Backend.Rabbit;

public class FailedCallException : Exception
{
    public FailedCallException(HttpStatusCode code)
    {
        Code = code;
    }

    public HttpStatusCode Code { get; }
    public string? Description { get; init; }
}
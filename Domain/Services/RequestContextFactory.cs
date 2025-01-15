using Microsoft.AspNetCore.SignalR;

namespace Domain.Services;

public class RequestContextFactory
{
    public static RequestContext Build(HubCallerContext context)
    {
        return Build(context.GetHttpContext()!.Request);
    }

    public static bool TryBuild(HttpRequest request, out RequestContext? requestContext)
    {
        var userId = FindRequestUserId(request);
        if (userId is null)
        {
            requestContext = null;
            return false;
        }

        var userName = FindRequestUserName(request);
        if (userName is null)
        {
            requestContext = null;
            return false;
        }

        requestContext = new RequestContext();
        requestContext.AddHeader(RequestContextKeys.UserId, userId.ToString());
        requestContext.AddHeader(RequestContextKeys.UserName, userName);
        return true;
    }

    public static RequestContext Build(HttpRequest request)
    {
        var requestContext = new RequestContext();
        var userId = GetUserIdOrThrow(request);
        var userName = GetUserNameOrThrow(request);
        requestContext.AddHeader(RequestContextKeys.UserId, userId.ToString());
        requestContext.AddHeader(RequestContextKeys.UserName, userName);

        return requestContext;
    }

    private static Guid GetUserIdOrThrow(HttpRequest request)
    {
        return FindRequestUserId(request) ?? throw new KeyNotFoundException();
    }

    private static string GetUserNameOrThrow(HttpRequest request)
    {
        return FindRequestUserName(request) ?? throw new KeyNotFoundException();
    }

    private static Guid? FindRequestUserId(HttpRequest request)
    {
        var value = request.Cookies[RequestContextKeys.UserId];
        return value is null
            ? null
            : Guid.Parse(value);
    }

    private static string? FindRequestUserName(HttpRequest request)
    {
        return request.Cookies[RequestContextKeys.UserName];
    }
}
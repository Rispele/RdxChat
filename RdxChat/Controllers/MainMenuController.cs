using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace RdxChat.Controllers;

[Route("main")]
public class MainMenuController : Controller
{
    private readonly IMessageService _messageService;

    public MainMenuController(IMessageService messageService)
    {
        _messageService = messageService;
    }

    [HttpGet("")]
    public async Task<IActionResult> Main()
    {
        if (!RequestContextFactory.TryBuild(ControllerContext.HttpContext.Request, out var requestContext))
            requestContext = await ProvideNewUser("Unknown");

        try
        {
            return View((requestContext!.GetUserId(), requestContext!.GetUserName()));
        }
        catch (InvalidOperationException e)
        {
            var recreatedRequestContext = await ProvideNewUser("Unknown");
            return View((recreatedRequestContext.GetUserId(), recreatedRequestContext.GetUserName()));
        }
    }

    [HttpPost("save-name")]
    public async Task SaveName([FromBody] string name)
    {
        if (!RequestContextFactory.TryBuild(ControllerContext.HttpContext.Request, out var requestContext))
        {
            throw new InvalidOperationException();
        }
        requestContext!.AddHeader(RequestContextKeys.UserName, name);
        Response.Cookies.Append(RequestContextKeys.UserName, name);
    }

    [HttpGet("find-companion")]
    public async Task<bool> FindCompanion(Guid companionId)
    {
        return true;
    }
    
    private async Task<RequestContext> ProvideNewUser(string userName)
    {
        var userId = Guid.NewGuid();

        var requestContext = new RequestContext();
        requestContext.AddHeader(RequestContextKeys.UserId, userId.ToString());
        requestContext.AddHeader(RequestContextKeys.UserName, userName);
        Response.Cookies.Append(RequestContextKeys.UserId, userId.ToString());
        Response.Cookies.Append(RequestContextKeys.UserName, userName);
        
        return requestContext;
    }
}
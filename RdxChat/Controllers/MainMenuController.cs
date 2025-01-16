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
    public IActionResult Main()
    {
        if (!RequestContextFactory.TryBuild(ControllerContext.HttpContext.Request, out var requestContext))
            requestContext = ProvideNewUser("Unknown");

        try
        {
            var userId = requestContext!.GetUserId();
            var userName = requestContext!.GetUserName();
            _messageService.SaveCredentials(userId, userName);
            return View((requestContext!.GetUserId(), requestContext!.GetUserName()));
        }
        catch (InvalidOperationException e)
        {
            var recreatedRequestContext = ProvideNewUser("Unknown");
            var userId = recreatedRequestContext.GetUserId();
            var userName = recreatedRequestContext.GetUserName();
            _messageService.SaveCredentials(userId, userName);
            return View((userId, userName));
        }
    }

    [HttpPost("save-name")]
    public void SaveName([FromBody] string name)
    {
        if (!RequestContextFactory.TryBuild(ControllerContext.HttpContext.Request, out var requestContext))
        {
            throw new InvalidOperationException();
        }
        requestContext!.AddHeader(RequestContextKeys.UserName, name);
        Response.Cookies.Append(RequestContextKeys.UserName, name);
        _messageService.SaveCredentials(requestContext!.GetUserId(), name);
    }

    [HttpGet("find-companion")]
    public async Task<bool> FindCompanion(Guid companionId)
    {
        return true;
    }
    
    private RequestContext ProvideNewUser(string userName)
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
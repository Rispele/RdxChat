using Domain.Services;
using Microsoft.AspNetCore.Mvc;

namespace RdxChat.Controllers;

[Route("main")]
public class MainMenuController : Controller
{
    private readonly IUserService _userService;
    
    public MainMenuController(IUserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet("")]
    public async Task<IActionResult> Main()
    {
        if (!RequestContextFactory.TryBuild(ControllerContext.HttpContext.Request, out var requestContext))
        {
            requestContext = await ProvideNewUser();
        }

        try
        {
            return View(requestContext!.GetUserId());
        }
        catch (InvalidOperationException e)
        {
            var recreatedRequestContext = await ProvideNewUser();
            return View(recreatedRequestContext.GetUserId());
        }
    }
    
    private async Task<RequestContext> ProvideNewUser()
    {
        var userId = await _userService.CreateUserAsync("Аноним");

        var requestContext = new RequestContext();
        requestContext.AddHeader(RequestContextKeys.UserId, userId.ToString());

        Response.Cookies.Append(RequestContextKeys.UserId, userId.ToString());
        return requestContext;
    }

    [HttpPost("save-name")]
    public async Task SaveName(string name)
    {
        var userId = Guid.Parse(Request.Cookies[RequestContextKeys.UserId] ?? throw new InvalidOperationException());
        await _userService.SetUserName(userId, name);
    }
    
    [HttpGet("find-companion")]
    public async Task<bool> FindCompanion(Guid companionId)
    {
        var found = await _userService.FindUser(companionId);
        return found;
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Taxi_App;

public class MessagesController : BaseApiController
{
    private readonly IMessageService _messageService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MessagesController(IMessageService messageService, IHttpContextAccessor httpContextAccessor)
    {
        _messageService = messageService;
        _httpContextAccessor = httpContextAccessor;
    }

    [HttpPost]
    [Authorize(Roles = "Driver, User")]
    public async Task<IActionResult> CreateMessage(CreateMessageDto createMessageDto)
    {
        var currUser = _httpContextAccessor.HttpContext.User;
        var currUsername = currUser.GetUsername();

        var result = await _messageService.CreateMessageAsync(currUsername, createMessageDto);
        if (!result.IsSuccess) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet]
    [Authorize(Roles = "Driver, User")]
    public async Task<IActionResult> GetMessages([FromQuery]string container)
    {
        var currUser = _httpContextAccessor.HttpContext.User;
        var currUsername = currUser.GetUsername();

        var result = await _messageService.GetMessagesForUserAsync(container, currUsername);
         if (!result.IsSuccess) return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpGet("thread/{username}")]
    public async Task<IActionResult> GetMessageThread(string username)
    {
        var currUser = _httpContextAccessor.HttpContext.User;
        var currUsername = currUser.GetUsername();

        return Ok(await _messageService.GetMessageThreadAsync(currUsername, username));
    }

}

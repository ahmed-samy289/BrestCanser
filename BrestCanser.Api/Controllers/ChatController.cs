using BrestCanser.Api.Authentication.Filter;

namespace BrestCanser.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting(RateLimiters.SensitivePolicy)]

public class ChatController : ControllerBase
{
	private readonly IChatService _chatService;
	public ChatController(IChatService chatService)
	{
		_chatService = chatService;
	}
	[HttpPost("ask")]
    [HasPermission(Permissions.AskChat)]
    public async Task<IActionResult> Ask([FromBody] ChatRequest request)
	{
		if (string.IsNullOrWhiteSpace(request.Prompt))
		{
			return BadRequest("Prompt cannot be empty.");
		}
		try
		{

			var answer = await _chatService.GetResponseAsync(request.Prompt);
			return Ok(new ChatResponse(answer));
		}
		catch (Exception ex)
		{
			return StatusCode(500, $"Internal Server Error {ex.Message}");
		}

	}
	public record ChatRequest(string Prompt);
}

using BrestCanser.Api.Authentication.Filter;

namespace BrestCanser.Api.Controllers;

[Route("/[Controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting(RateLimiters.GeneralPolicy)]

public class AccountController : ControllerBase
{
	private readonly IUserService _userService;
	public AccountController(IUserService userService)
	{
		_userService = userService;
	}

	[HttpGet("profile")]
    [HasPermission(Permissions.GetProfile)]
    public async Task<IActionResult> Info()
	{
		var result = await _userService.GetProfileAsync(User.GetUserId()!);

		return Ok(result.Value);
	}

	[HttpPut("update-profile")]
    [HasPermission(Permissions.UpdateProfile)]
    public async Task<IActionResult> Update([FromBody] UpdateProfileRequest request)
	{
		var result = await _userService.UpdateProfileAsync(User.GetUserId()!, request);

		return result.IsSuccess ? NoContent() : result.ToProblem();
	}

	[HttpPut("change-password")]
	[EnableRateLimiting(RateLimiters.SensitivePolicy)]
    [HasPermission(Permissions.ChangePassword)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
	{
		var result = await _userService.ChangePasswordAsync(User.GetUserId()!, request);

		return result.IsSuccess ? NoContent() : result.ToProblem();
	}
}
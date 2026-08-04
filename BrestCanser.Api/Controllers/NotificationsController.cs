using BrestCanser.Api.Authentication.Filter;

namespace BrestCanser.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class NotificationsController : ControllerBase
{
	private readonly INotificationService _notificationService;

	public NotificationsController(INotificationService notificationService)
	{
		_notificationService = notificationService;
	}

	[HttpGet("")]
    [HasPermission(Permissions.GetNotifications)]
    public async Task<IActionResult> GetNotifications()
	{
		var result = await _notificationService.GetNotificationsAsync(User.GetUserId()!);

		return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
	}

	[HttpPut("{id}/mark-read")]
    [HasPermission(Permissions.MarkNotificationAsRead)]
    public async Task<IActionResult> MarkAsRead(int id)
	{
		var result = await _notificationService.MarkAsReadAsync(id, User.GetUserId()!);

		return result.IsSuccess ? NoContent() : result.ToProblem();
	}

	[HttpPut("mark-all-read")]
    [HasPermission(Permissions.MarkAllNotificationsAsRead)]
    public async Task<IActionResult> MarkAllAsRead()
	{
		var result = await _notificationService.MarkAllAsReadAsync(User.GetUserId()!);

		return result.IsSuccess ? NoContent() : result.ToProblem();
	}
}
using BrestCanser.Api.Clients.MLModel.Contracts.MLModel;


namespace BrestCanser.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
[EnableRateLimiting(RateLimiters.SensitivePolicy)]

public class MLController(IMLService _mlService) : ControllerBase
{
	[HttpPost("")]
	public async Task<IActionResult> Predict([FromForm] PredictRequest request, CancellationToken cancellationToken)
	{
		var result = await _mlService.PredictAsync(request, User.GetUserId()!, cancellationToken);

		return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
	}
}
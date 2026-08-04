using BrestCanser.Api.Authentication.Filter;
using BrestCanser.Api.Contracts.RiskAssessment;

namespace BrestCanser.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RiskAssessmentController(IRiskAssessmentService assessmentService) : Controller
{
	private readonly IRiskAssessmentService _assessmentService = assessmentService;

	[HttpPost("assess")]
    [HasPermission(Permissions.RiskAssessment)]
    public async Task<IActionResult> Assess([FromBody] RiskAssessmentRequest request, CancellationToken cancellationToken)
	{
		var result = await _assessmentService.AssessAsync(request, cancellationToken);

		return result.IsSuccess ? Ok(result.Value) : result.ToProblem();
	}
}
namespace BrestCanser.Api.Authentication;
public interface IJwtProvider
{
	(string token, int expiresIn) GenerateToken(ApplicationUser user, IEnumerable<string> roles, IEnumerable<string> permissions);
    string? GetUserIdFromExpiredToken(string token);
    string? ValidateToken(string token);

}
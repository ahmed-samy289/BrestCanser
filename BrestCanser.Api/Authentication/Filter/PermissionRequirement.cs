namespace BrestCanser.Api.Authentication.Filter
{
    public class PermissionRequirement(string permission):IAuthorizationRequirement
    {
        public string Permission { get; } = permission;
    }
}

namespace BrestCanser.Api.Entites
{
    public class ApplicationRole : IdentityRole
    {
        public bool IsDefault { get; set; }

        public bool IsDeleted { get; set; }
    }
}

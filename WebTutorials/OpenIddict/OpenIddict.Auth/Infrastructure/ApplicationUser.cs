using Microsoft.AspNetCore.Identity;

namespace OpenIddict.Auth.Infrastructure
{
    public class ApplicationUser : IdentityUser<Guid>
    {
    }
    
    public class ApplicationRole : IdentityRole<Guid>
    {}
}
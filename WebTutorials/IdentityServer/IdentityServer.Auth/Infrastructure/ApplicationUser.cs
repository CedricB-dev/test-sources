using System;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer.Auth.Infrastructure
{
    public class ApplicationUser : IdentityUser<Guid>
    {
    }
    
    public class ApplicationRole : IdentityRole<Guid>
    {}
}
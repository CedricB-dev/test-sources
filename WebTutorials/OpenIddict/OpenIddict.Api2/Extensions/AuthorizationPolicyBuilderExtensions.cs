using Microsoft.AspNetCore.Authorization;

namespace OpenIddict.Api2.Extensions;

public static class AuthorizationPolicyBuilderExtensions
{
    private static readonly IEnumerable<string> _scopeClaimTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase) {
        "http://schemas.microsoft.com/identity/claims/scope",
        "scope"
    };
    public static AuthorizationPolicyBuilder RequireScope(this AuthorizationPolicyBuilder builder, params string[] scopes) {
        return builder.RequireAssertion(context =>
            context.User
                .Claims
                .Where(c => _scopeClaimTypes.Contains(c.Type))
                .SelectMany(c => c.Value.Split(' '))
                .Any(s => scopes.Contains(s, StringComparer.Ordinal))
        );
    }
}
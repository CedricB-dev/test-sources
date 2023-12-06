// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer.Auth.Infrastructure;
using IdentityServer4;

namespace IdentityServer.Auth
{
    public static class Config
    {
        public static IEnumerable<ApiScope> ApiScopes =>
            new ApiScope[]
            {
                new("api.read"),
                new("api.write"),
                new("api.delete")
            };

        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[]
            {
                new("api1", "Api 1")
                {
                    Scopes = new[] { "api.read", "api.write", "api.delete" }
                },
                new("api2", "Api 2")
                {
                    Scopes = new[] { "api.read", "api.write", "api.delete" }
                }
            };

        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource("roles", "Roles", new[] { ClaimTypes.Role })
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                // m2m client credentials flow client
                new Client
                {
                    ClientId = "console-read",
                    ClientName = "Console read",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("console-read-secret".Sha256()) },
                    AllowedScopes = { "api.read" },
                },
                new Client
                {
                    ClientId = "console-full",
                    ClientName = "Console Full",

                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = { new Secret("console-full-secret".Sha256()) },
                    AllowedScopes = { "api.read", "api.write", "api.delete" },
                },
                // interactive client using code flow + pkce
                new Client
                {
                    ClientId = "web-read",
                    ClientSecrets = { new Secret("web-read-secret".Sha256()) },

                    AllowedGrantTypes = GrantTypes.Code,

                    RedirectUris = { "https://localhost:44300/signin-oidc" },
                    //FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
                    PostLogoutRedirectUris = { "https://localhost:44300/signout-callback-oidc" },
                    AllowOfflineAccess = true,
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "roles",
                        "api.read"
                    },
                },
            };
    }
}
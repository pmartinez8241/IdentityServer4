// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;

namespace IdentityServerDemo
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            new IdentityResource[]
            { 
                new IdentityResources.OpenId()
            };

        public static IEnumerable<ApiScope> ApiScopes =>
            new List<ApiScope> { 
                new ApiScope("WebApi1","FirstIdentityWebApi")
            };

        public static IEnumerable<Client> Clients =>
            new List<Client>
            {
                new Client{ 
                    ClientId = "DemoWebApi",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets={
                      new Secret("PasswordToDemoWebApi".Sha256())
                    },
                    AllowedScopes = { "WebApi1" }
                }
            };
    }
}
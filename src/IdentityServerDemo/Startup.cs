// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Reflection;

namespace IdentityServerDemo
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public IConfiguration Configuration;

        public Startup(IWebHostEnvironment environment, IConfiguration config)
        {
            Environment = environment;
            Configuration = config;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            string ConnectionString = Configuration.GetConnectionString("IdentityServer4");

            var build = services.AddIdentityServer(options => { options.IssuerUri = "http://identityserver.local"; })
            .AddConfigurationStore(options =>
   {
       options.ConfigureDbContext = b => b.UseMySql(ConnectionString, x => x.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name));
       options.ApiResourceClaim.Name = "api_claim";
       options.ApiResourceProperty.Name = "api_property";
       options.ApiResource.Name = "api_resource";
       options.ApiScopeClaim.Name = "api_scope_claim";
       options.ApiResourceScope.Name = "api_resource_scope";
       options.ApiScopeProperty.Name = "api_scope_name";
       options.ApiScope.Name = "api_scope";
       options.ApiResourceSecret.Name = "api_resource_secret";
       options.ClientClaim.Name = "client_claim";
       options.ClientCorsOrigin.Name = "client_cors_origin";
       options.ClientGrantType.Name = "client_grant_type";
       options.ClientIdPRestriction.Name = "client_id_prestriction";
       options.ClientPostLogoutRedirectUri.Name = "client_post_logout_redirect_uri";
       options.ClientProperty.Name = "client_property";
       options.ClientRedirectUri.Name = "client_redirect_uri";
       options.Client.Name = "client";
       options.ClientScopes.Name = "client_scope";
       options.ClientSecret.Name = "client_secret";
       options.IdentityResourceClaim.Name = "identity_resource_claim";
       options.IdentityResourceProperty.Name = "identity_property";
       options.IdentityResource.Name = "identity_resource";
   })
   .AddOperationalStore(options =>
   {
       options.ConfigureDbContext = b => b.UseMySql(ConnectionString, x => x.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name));
       options.PersistedGrants.Name = "persisted_grant";
       options.DeviceFlowCodes.Name = "device_flow_codes";

   });


            // not recommended for production - you need to store your key material somewhere secure
            build.AddDeveloperSigningCredential();
        }

        private void InitializeDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {

                var context = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                if (!context.Clients.Any())
                {
                    foreach (var client in Config.Clients)
                    {
                        context.Clients.Add(client.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.IdentityResources.Any())
                {
                    foreach (var resource in Config.IdentityResources)
                    {
                        context.IdentityResources.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }

                if (!context.ApiScopes.Any())
                {
                    foreach (var resource in Config.ApiScopes)
                    {
                        context.ApiScopes.Add(resource.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }

        public void Configure(IApplicationBuilder app)
        {


            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var fordwardedHeaderOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            };
            fordwardedHeaderOptions.KnownNetworks.Clear();
            fordwardedHeaderOptions.KnownProxies.Clear();
            app.UseForwardedHeaders(fordwardedHeaderOptions);

            app.UseIdentityServer();

        }
    }
}

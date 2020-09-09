using System.Linq;
using System.Security.Claims;
using IdentityServer.Configurations;
using IdentityServer.Models;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Data
{
    public static class SeedDatabase
    {
        public static void SetUser(IServiceScope scope)
        {
            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<IdentityUser>>();

            var user = new IdentityUser("bob");
            userManager.CreateAsync(user, "password").GetAwaiter().GetResult();
            // This claim will be added to identity token
            userManager.AddClaimAsync(user,
                    new Claim("rc.grandma", "big.Cookie"))
                .GetAwaiter().GetResult();
            // This claim we will add to access token
            userManager.AddClaimAsync(user,
                    new Claim("rc.api.grandma", "big.api.Cookie"))
                .GetAwaiter().GetResult();
        }

        public static void SeedDb(IServiceScope scope)
        {
            scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

            var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

            context.Database.Migrate();

            if (!context.Clients.Any())
            {
                foreach (var client in Configuration.Clients)
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in Configuration.IdentityResources)
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.ApiResources.Any())
            {
                foreach (var resource in Configuration.ApiScopes)
                {
                    context.ApiScopes.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
        }
    }
}

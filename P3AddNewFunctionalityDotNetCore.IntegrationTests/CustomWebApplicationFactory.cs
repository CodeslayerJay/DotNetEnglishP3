using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using P3AddNewFunctionalityDotNetCore.Data;
using P3AddNewFunctionalityDotNetCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace P3AddNewFunctionalityDotNetCore.IntegrationTests
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup: class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Create a new service provider.
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();

                // Add a database context using an in-memory 
                // database for testing.
                services.AddDbContext<P3Referential>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                    options.UseInternalServiceProvider(serviceProvider);
                });

                // Build the service provider.
                var sp = services.BuildServiceProvider();

                // Create a scope to obtain a reference to the database
                // context (ApplicationDbContext).
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<P3Referential>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                    // Ensure the database is created.
                    db.Database.EnsureCreated();

                    // Seed our db with test data
                    try
                    {
                        //var context = scopedServices.GetRequiredService<P3Referential>();
                        //context.Database.Migrate();
                        //var identityContext = scopedServices.GetRequiredService<AppIdentityDbContext>();
                        //identityContext.Database.Migrate();
                        SeedData.Initialize(scopedServices);
                    }
                    catch (Exception ex)
                    {
                        //var logger = scopedServices.GetRequiredService<ILogger<Program>>();
                        logger.LogError(ex, "An error occurred seeding the DB.");
                    }
                }
            });
        }
    }
}

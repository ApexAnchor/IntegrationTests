using Customer.WebApi.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.MsSql;

namespace Customer.Integration.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{    
    const string connectionString = $"server=.;user id={MsSqlBuilder.DefaultUsername};password={MsSqlBuilder.DefaultPassword};database=CustomersDb";
    
    private readonly MsSqlContainer dbContainer = new MsSqlBuilder()
                                                 .WithImage("mcr.microsoft.com/mssql/server:2019-CU18-ubuntu-20.04")
                                                 .WithEnvironment("ConnectionStrings__DefaultConnection", connectionString)
                                                 .Build();
 
    private static CustomWebApplicationFactory factory;

    public static CustomWebApplicationFactory CreateFactory()
    {
        return factory = new CustomWebApplicationFactory();        
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services => 
        {
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));

            services.AddDbContext<ApplicationDbContext>(options => 
            {
                options.UseSqlServer(dbContainer.GetConnectionString());
            });
        });
    }

    public async Task Start()
    {        
        await dbContainer.StartAsync();
        
        using (var scope = factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.Migrate();
        }
    }
    public async Task Stop()
    {
        await dbContainer.StopAsync();
    }
}

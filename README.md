# Integration Testing with WebApplicationFactory and TestContainers in .NET 8

This repository demonstrates how to write integration tests using `WebApplicationFactory` and `TestContainers` in a .NET 8 application.

## Prerequisites

- Docker Desktop

## Overview

`WebApplicationFactory` is a factory for bootstrapping an application in memory for functional end to end tests.

`TestContainers` is a .NET library that supports tests, providing lightweight, throwaway instances of common databases, Selenium web browsers, or anything else that can run in a Docker container.

## Setup

1. Install the `Microsoft.AspNetCore.Mvc.Testing` NuGet package to your test project.
2. Install the `TestContainers` NuGet package to your test project.

## Writing Tests

Here is an example of a test that uses `WebApplicationFactory` to create an instance of our web application, and `TestContainers` to create a MsSQL database.

Create a CustomWebApplicationFactory class as shown below
```
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
```
In this example, we're using NUnit's `[OneTimeSetUp]` and `[OneTimeTearDown]` attributes to start and stop our database container. This ensures that the database is up and running for all our tests, and cleaned up afterwards.
```csharp
[SetUpFixture]
public class BaseIntegrationTest
{
    private readonly IServiceScope scope;
    protected readonly CustomWebApplicationFactory factory;
    protected HttpClient client;

    public BaseIntegrationTest()
    {
        factory = CustomWebApplicationFactory.CreateFactory();
        client = factory.CreateClient();        
    }   

    [OneTimeSetUp]
    public void Start()
    {
        Task.Run(async () => await factory.Start()).GetAwaiter().GetResult();          
    }

    [OneTimeTearDown]
    public void Dispose()
    {
        Task.Run(async () => await factory.Stop()).GetAwaiter().GetResult();
    }
}
```

Actual Tests
```
 [Test]
 public async Task CreateCustomerMethod_AddsCustomer_ToDatabase()
 {
     //Arrange
     var url = "api/Customer/CreateCustomer";          
     using var message = new HttpRequestMessage(HttpMethod.Post, url);            
     message.Content = JsonContent.Create(new WebApi.Models.Customer()
     {
         CustomerId = 123,
         CustomerEmail = "abc@gmail.com",
         CustomerName = "Monica Geller",
         PhoneNumber = "1234567898"
     });

     //Act
     var response = await client.SendAsync(message);

     //Assert
     Assert.IsTrue(response.IsSuccessStatusCode);
     Assert.IsNotNull(response.Content);                
 }
```

## Running Tests

To run the tests, simply use the `dotnet test` command in your terminal.

```shell
dotnet test
```

This will automatically start the containers, run the tests, and stop the containers.

## Conclusion

This is a simple demonstration of how you can use `WebApplicationFactory` and `TestContainers` to write integration tests in .NET 8. By using these tools, you can ensure that your application works correctly with real instances of your dependencies, without having to manage those dependencies manually.

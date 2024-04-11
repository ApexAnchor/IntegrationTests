using Customer.WebApi.Data;
using Microsoft.Extensions.DependencyInjection;

namespace Customer.Integration.Tests;

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

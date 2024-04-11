using Customer.WebApi.Data;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;

namespace Customer.Integration.Tests
{
    [TestFixture]
    public class CustomerControllerTests : BaseIntegrationTest
    {

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

        [Test]
        public async Task GetAllCustomer_Returns_CustomerList()
        {
            //Arrange
            var scope = factory.Services.CreateScope();
            var dbcontext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
           
            dbcontext.Database.EnsureDeleted();
            dbcontext.Database.EnsureCreated();
          
            await dbcontext.Customers.AddAsync(new WebApi.Models.Customer()
            {
                CustomerId = 456,
                CustomerEmail = "abc@gmail.com",
                CustomerName = "Monica Geller",
                PhoneNumber = "1234567898"
            });

            dbcontext.SaveChanges();

            //Act
            var response = await client.GetAsync("/api/Customer/GetAllCustomers");
            var result = await response.Content.ReadFromJsonAsync<List<WebApi.Models.Customer>>();

            Assert.IsTrue(result.Count == 1);
        }
    }
}
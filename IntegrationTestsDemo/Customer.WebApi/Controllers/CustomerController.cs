using Customer.WebApi.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Customer.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ApplicationDbContext applicationDbContext;

    public CustomerController(ApplicationDbContext applicationDbContext)
    {
        this.applicationDbContext = applicationDbContext;
    }

    [HttpGet("GetAllCustomers")]
    public async Task<IActionResult> GetAllCustomers()
    {
        var customers = await applicationDbContext.Customers.ToListAsync();

        return new JsonResult(customers);
    }

    [HttpGet("GetCustomer/{customerId:int}")]
    public async Task<IActionResult> GetCustomerById(int customerId)
    {
        var customer = await applicationDbContext.Customers.SingleOrDefaultAsync(x => x.CustomerId == customerId);
       
        if (customer is not null) 
        {
            return new JsonResult(customer);
        }
        return BadRequest("No customer found with the given id!!");
    }

    [HttpPost("CreateCustomer")]
    public async Task<IActionResult> CreateCustomer([FromBody] Models.Customer customer)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await applicationDbContext.Customers.AddAsync(customer);
                await applicationDbContext.SaveChangesAsync();
                return new JsonResult(customer.CustomerId);
            }

        }
        catch(Exception ex)
        {
            return BadRequest("Invalid Request");
        }
        
        return BadRequest("Invalid Request");
    }

    [HttpDelete("DeleteCustomer/{customerId:int}")]
    public async Task<IActionResult> Delete(int customerId)
    {
        var customer = await applicationDbContext.Customers.SingleOrDefaultAsync(x => x.CustomerId == customerId);

        if (customer is not null)
        {
            applicationDbContext.Remove(customer);
            await applicationDbContext.SaveChangesAsync();
            return Ok("Deleted sucessfully");
        }
        return BadRequest("No customer found with the given id!!");

    }
}

using Microsoft.EntityFrameworkCore;

namespace Customer.WebApi.Data;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }
    public DbSet<Models.Customer> Customers { get; set; }
}

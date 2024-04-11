using System.ComponentModel.DataAnnotations;

namespace Customer.WebApi.Models;
public class Customer
{
    [Key]
    [Required]
    public int CustomerId { get; set; }

    [Required]
    public string CustomerName { get; set; }

    [Required]
    [EmailAddress]
    public string CustomerEmail { get; set; }

    [Required]
    [Length(10, 10, ErrorMessage = "Invalid Phone Number")]
    public string PhoneNumber { get; set; }
}


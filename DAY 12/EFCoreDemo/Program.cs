// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using System.ComponentModel.DataAnnotations;
using System;
using System.Linq;

CrmContext _context = new CrmContext();

var customers = _context.Customers
    .Where(e => e.Age > 20)
    .ToList();

// Without ToList()
// Console.WriteLine("customers.GetType(): " + customers.GetType());
// Console.WriteLine($"Query: {customers.ToQueryString()}");

// With ToList()
// Console.WriteLine("customers.GetType(): " + customers.GetType());

// With AsEnumerable()
// Console.WriteLine("customers.GetType(): " + customers.GetType());

// customers.Add(new Customer { Name = "John Doe", Age = 30 });
// _context.SaveChanges();

Console.WriteLine($"Customers Count: {customers.Count()}");

customers.Add(new Customer{Name="Alice Brown", Age= 32});
_context.SaveChanges();

var john = _context.Customers.FirstOrDefault(c => c.Name == "John Doe");
if (john != null) john.Age = 31;

_context.SaveChanges();

foreach (var customer in customers)
{
    Console.WriteLine($"Id: {customer.Id} Customer: {customer.Name}, Age: {customer.Age}");
}

class CrmContext : DbContext
{
    public DbSet<Customer> Customers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(
            "Server=localhost\\SQLEXPRESS;Database=CrmDB;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;");
    }
}

public class Order
{
    [Key]
    public int OrderId { get; set; }

    [Required]
    [MaxLength(100)]
    [MinLength(3)]
    public string Product { get; set; }

    [Required]
    [Precision(18, 2)]
    public decimal Price { get; set; }

    public int CustomerId { get; set; }

    public Customer Customers { get; set; }
}

public class Customer
{
    public int Id { get; set; }

    public string Name { get; set; }

    public int Age { get; set; }
}

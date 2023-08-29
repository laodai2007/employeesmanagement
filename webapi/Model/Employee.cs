using System.Text.Json.Serialization;

namespace webapi;

public class Employee
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Position { get; set; }
    public DateTime HiringDate { get; set; }
    public decimal Salary { get; set; }
}

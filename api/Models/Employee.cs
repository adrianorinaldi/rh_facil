namespace RhFacil.Api.Models;

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public string Status { get; set; } = "Ativo"; // Ativo, Férias, Desligado
    public DateTime HireDate { get; set; } = DateTime.UtcNow;
    public ICollection<EmployeeAttachment> Attachments { get; set; } = new List<EmployeeAttachment>();
}

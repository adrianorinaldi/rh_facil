using Microsoft.EntityFrameworkCore;
using RhFacil.Api.Models;

namespace RhFacil.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Employee> Employees { get; set; }
    public DbSet<EmployeeAttachment> EmployeeAttachments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Dados Iniciais (Seed) para a demonstração
        modelBuilder.Entity<Employee>().HasData(
            new Employee { Id = 1, Name = "João Silva", Position = "Desenvolvedor Backend", Department = "TI", Salary = 8000, Status = "Ativo", HireDate = new DateTime(2022, 1, 15, 0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 2, Name = "Maria Souza", Position = "Analista de RH", Department = "Recursos Humanos", Salary = 5500, Status = "Ativo", HireDate = new DateTime(2023, 3, 10, 0, 0, 0, DateTimeKind.Utc) },
            new Employee { Id = 3, Name = "Pedro Santos", Position = "Gerente de Projetos", Department = "Operações", Salary = 12000, Status = "Férias", HireDate = new DateTime(2021, 6, 20, 0, 0, 0, DateTimeKind.Utc) }
        );
    }
}

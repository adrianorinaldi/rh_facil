using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RhFacil.Api.Data;
using RhFacil.Api.Models;

namespace RhFacil.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _context;

    public EmployeesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/employees
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Employee>>> GetEmployees()
    {
        return await _context.Employees.ToListAsync();
    }

    // GET: api/employees/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Employee>> GetEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if (employee == null)
        {
            return NotFound(new { Message = "Funcionário não encontrado." });
        }

        return employee;
    }

    // POST: api/employees
    [HttpPost]
    public async Task<ActionResult<Employee>> CreateEmployee(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
    }

    // PUT: api/employees/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEmployee(int id, Employee employee)
    {
        if (id != employee.Id)
        {
            return BadRequest(new { Message = "ID incorreto." });
        }

        _context.Entry(employee).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!EmployeeExists(id))
            {
                return NotFound(new { Message = "Funcionário não encontrado." });
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    // DELETE: api/employees/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if (employee == null)
        {
            return NotFound(new { Message = "Funcionário não encontrado." });
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/employees/5/attachments
    [HttpGet("{id}/attachments")]
    public async Task<ActionResult<IEnumerable<object>>> GetEmployeeAttachments(int id)
    {
        var attachments = await _context.EmployeeAttachments
            .Where(a => a.EmployeeId == id)
            .Select(a => new
            {
                a.Id,
                a.EmployeeId,
                a.FileName,
                a.ContentType,
                a.UploadedAt
                // Omitimos FileData para não enviar o binário na listagem
            })
            .ToListAsync();

        return Ok(attachments);
    }

    // POST: api/employees/5/attachments
    [HttpPost("{id}/attachments")]
    public async Task<IActionResult> UploadAttachment(int id, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { Message = "Nenhum arquivo enviado." });

        if (!EmployeeExists(id))
            return NotFound(new { Message = "Funcionário não encontrado." });

        using var memoryStream = new MemoryStream();
        await file.CopyToAsync(memoryStream);

        var attachment = new EmployeeAttachment
        {
            EmployeeId = id,
            FileName = file.FileName,
            ContentType = file.ContentType,
            FileData = memoryStream.ToArray(),
            UploadedAt = DateTime.UtcNow
        };

        _context.EmployeeAttachments.Add(attachment);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Arquivo enviado com sucesso.",
            AttachmentId = attachment.Id,
            FileName = attachment.FileName,
            UploadedAt = attachment.UploadedAt
        });
    }

    // GET: api/employees/attachments/5
    [HttpGet("attachments/{attachmentId}")]
    public async Task<IActionResult> DownloadAttachment(int attachmentId)
    {
        var attachment = await _context.EmployeeAttachments.FindAsync(attachmentId);

        if (attachment == null)
            return NotFound(new { Message = "Anexo não encontrado." });

        return File(attachment.FileData, attachment.ContentType, attachment.FileName);
    }

    // DELETE: api/employees/attachments/5
    [HttpDelete("attachments/{attachmentId}")]
    public async Task<IActionResult> DeleteAttachment(int attachmentId)
    {
        var attachment = await _context.EmployeeAttachments.FindAsync(attachmentId);

        if (attachment == null)
            return NotFound(new { Message = "Anexo não encontrado." });

        _context.EmployeeAttachments.Remove(attachment);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool EmployeeExists(int id)
    {
        return _context.Employees.Any(e => e.Id == id);
    }
}

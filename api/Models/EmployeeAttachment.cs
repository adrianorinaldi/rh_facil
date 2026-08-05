using System.Text.Json.Serialization;

namespace RhFacil.Api.Models;

public class EmployeeAttachment
{
    public int Id { get; set; }
    
    public int EmployeeId { get; set; }
    
    [JsonIgnore]
    public Employee? Employee { get; set; }
    
    public string FileName { get; set; } = string.Empty;
    
    public string ContentType { get; set; } = string.Empty;
    
    [JsonIgnore] // Ocultamos do JSON padrão para não sobrecarregar as requisições GET com dados binários pesados
    public byte[] FileData { get; set; } = Array.Empty<byte>();
    
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}

namespace Shared.Generics;
public class GenericResponse<T>()
{
    public required T Data { get; set; }
    public string? Message { get; set; } 
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    public List<string> Errors { get; set; } = [];
}
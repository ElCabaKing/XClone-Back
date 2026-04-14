

namespace Domain.Entities;

public class Token
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string RefreshToken { get; set; } = null!;
    public DateTime ExpiryDate { get; set; }
    public DateTime CreatedAt { get; set; }
}
namespace And9.Service.Auth.Database.Models;

public record PasswordHash
{
    public int UserId { get; set; }
    public byte[] Hash { get; set; } = Array.Empty<byte>();
}
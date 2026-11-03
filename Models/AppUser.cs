namespace ListovkiQuiz.Models;

/// <summary>A registered person. The password itself is never stored - only its hash.</summary>
public class AppUser
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

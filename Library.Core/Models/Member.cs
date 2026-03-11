namespace Library.Core.Models;

public class Member
{
    // Columns common to all tables
    public int Id { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;

    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
}

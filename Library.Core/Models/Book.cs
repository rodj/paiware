namespace Library.Core.Models;

public class Book
{
    // Columns common to all tables
    public int Id { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;

    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public string Isbn { get; set; } = "";
}

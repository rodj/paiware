namespace Library.Core.Models;

public class Checkout
{
    // Columns common to all tables
    public int Id { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.UtcNow;

    public int BookId { get; set; }
    public int MemberId { get; set; }
    public DateTime CheckedOutAt { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? ReturnedAt { get; set; }
}

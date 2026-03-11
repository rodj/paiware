namespace Library.Core.Services;

public record BookSummary(int Id, string Title, string Author, string Isbn, DateTime CreateDate, bool IsAvailable);

public record CheckoutDetail(
    int Id,
	DateTime CreateDate,
    int BookId,
	string BookTitle,
    int MemberId, 
	string MemberName,
    DateTime CheckedOutAt,
    DateTime DueDate,
    DateTime? ReturnedAt);

public record DashboardStats(
    int TotalBooks,
    int AvailableBooks,
    int CheckedOutBooks,
    int OverdueCheckouts,
    int TotalMembers);

public interface ICheckoutService
{
    IEnumerable<BookSummary> GetAllBooks();
    BookSummary AddBook(string title, string author, string isbn);
    CheckoutDetail CheckOutBook(int bookId, int memberId);
    CheckoutDetail ReturnBook(int checkoutId);
    IEnumerable<CheckoutDetail> GetOverdueCheckouts();
    DashboardStats GetDashboard();
}

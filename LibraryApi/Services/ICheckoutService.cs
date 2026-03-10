namespace LibraryApi.Services;

public record BookSummary(int Id, string Title, string Author, string Isbn, bool IsAvailable);

public record CheckoutDetail(
    int Id,
    int BookId, string BookTitle,
    int MemberId, string MemberName,
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
    CheckoutDetail CheckOutBook(int bookId, int memberId);
    CheckoutDetail ReturnBook(int checkoutId);
    IEnumerable<CheckoutDetail> GetOverdueCheckouts();
    DashboardStats GetDashboard();
}

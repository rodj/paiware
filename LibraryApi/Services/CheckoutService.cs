using Library.Core.Models;
using Library.Core.Services;
using LibraryApi.Data;

namespace LibraryApi.Services;

public class CheckoutService(InMemoryDataStore store) : ICheckoutService
{
    public IEnumerable<BookSummary> GetAllBooks()
    {
        var checkedOutBookIds = store.Checkouts
            .Where(c => c.ReturnedAt == null)
            .Select(c => c.BookId)
            .ToHashSet();

        return store.Books.Select(b => new BookSummary(
            b.Id, b.Title, b.Author, b.Isbn, b.CreateDate,
            IsAvailable: !checkedOutBookIds.Contains(b.Id)));
    }

    public BookSummary AddBook(string title, string author, string isbn)
    {
        var book = new Book
        {
            Id = store.Books.Max(b => b.Id) + 1,
            Title = title,
            Author = author,
            Isbn = isbn
        };
        store.Books.Add(book);
        return new BookSummary(book.Id, book.Title, book.Author, book.Isbn, book.CreateDate, IsAvailable: true);
    }

    public CheckoutDetail CheckOutBook(int bookId, int memberId)
    {
        var book = store.Books.FirstOrDefault(b => b.Id == bookId)
            ?? throw new KeyNotFoundException($"Book {bookId} not found.");

        var member = store.Members.FirstOrDefault(m => m.Id == memberId)
            ?? throw new KeyNotFoundException($"Member {memberId} not found.");

        bool alreadyOut = store.Checkouts.Any(c => c.BookId == bookId && c.ReturnedAt == null);
        if (alreadyOut)
            throw new InvalidOperationException($"Book '{book.Title}' is already checked out.");

        var checkout = new Checkout
        {
            Id = store.NextCheckoutId(),
            BookId = bookId,
            MemberId = memberId,
            CheckedOutAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(14)
        };

        store.Checkouts.Add(checkout);
        return ToDetail(checkout, book, member);
    }

    public CheckoutDetail ReturnBook(int checkoutId)
    {
        var checkout = store.Checkouts.FirstOrDefault(c => c.Id == checkoutId)
            ?? throw new KeyNotFoundException($"Checkout {checkoutId} not found.");

        if (checkout.ReturnedAt != null)
            throw new InvalidOperationException("This book has already been returned.");

        checkout.ReturnedAt = DateTime.UtcNow;

        var book = store.Books.First(b => b.Id == checkout.BookId);
        var member = store.Members.First(m => m.Id == checkout.MemberId);
        return ToDetail(checkout, book, member);
    }

    public IEnumerable<CheckoutDetail> GetOverdueCheckouts()
    {
        var now = DateTime.UtcNow;
        return store.Checkouts
            .Where(c => c.ReturnedAt == null && c.DueDate < now)
            .Select(c => ToDetail(c,
                store.Books.First(b => b.Id == c.BookId),
                store.Members.First(m => m.Id == c.MemberId)));
    }

    public DashboardStats GetDashboard()
    {
        var checkedOutIds = store.Checkouts
            .Where(c => c.ReturnedAt == null)
            .Select(c => c.BookId)
            .ToHashSet();

        var now = DateTime.UtcNow;
        int overdue = store.Checkouts.Count(c => c.ReturnedAt == null && c.DueDate < now);

        return new DashboardStats(
            TotalBooks: store.Books.Count,
            AvailableBooks: store.Books.Count - checkedOutIds.Count,
            CheckedOutBooks: checkedOutIds.Count,
            OverdueCheckouts: overdue,
            TotalMembers: store.Members.Count);
    }

    private static CheckoutDetail ToDetail(Checkout c, Book b, Member m) =>
        new(c.Id, c.CreateDate, b.Id, b.Title, m.Id, m.Name, c.CheckedOutAt, c.DueDate, c.ReturnedAt);
}

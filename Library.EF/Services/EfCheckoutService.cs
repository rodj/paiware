using Library.Core.Models;
using Library.Core.Services;
using Library.EF.Data;
using Microsoft.EntityFrameworkCore;

namespace Library.EF.Services;

public class EfCheckoutService(LibraryContext context) : ICheckoutService
{
    public IEnumerable<BookSummary> GetAllBooks()
    {
        return context.Books.AsNoTracking()
            .Select(b => new BookSummary(
                b.Id, b.Title, b.Author, b.Isbn, b.CreateDate,
                !context.Checkouts.Any(c => c.BookId == b.Id && c.ReturnedAt == null)))
            .ToList();
    }

    public BookSummary AddBook(string title, string author, string isbn)
    {
        var book = new Book { Title = title, Author = author, Isbn = isbn };
        context.Books.Add(book);
        context.SaveChanges();
        return new BookSummary(book.Id, book.Title, book.Author, book.Isbn, book.CreateDate, true);
    }

    public CheckoutDetail CheckOutBook(int bookId, int memberId)
    {
        var book = context.Books.FirstOrDefault(b => b.Id == bookId)
            ?? throw new KeyNotFoundException($"Book {bookId} not found.");

        var member = context.Members.FirstOrDefault(m => m.Id == memberId)
            ?? throw new KeyNotFoundException($"Member {memberId} not found.");

        bool alreadyOut = context.Checkouts.Any(c => c.BookId == bookId && c.ReturnedAt == null);
        if (alreadyOut)
            throw new InvalidOperationException($"Book '{book.Title}' is already checked out.");

        var checkout = new Checkout
        {
            BookId = bookId,
            MemberId = memberId,
            CheckedOutAt = DateTime.UtcNow,
            DueDate = DateTime.UtcNow.AddDays(14)
        };

        context.Checkouts.Add(checkout);
        context.SaveChanges();
        return ToDetail(checkout, book, member);
    }

    public CheckoutDetail ReturnBook(int checkoutId)
    {
        var checkout = context.Checkouts.FirstOrDefault(c => c.Id == checkoutId)
            ?? throw new KeyNotFoundException($"Checkout {checkoutId} not found.");

        if (checkout.ReturnedAt != null)
            throw new InvalidOperationException("This book has already been returned.");

        checkout.ReturnedAt = DateTime.UtcNow;
        context.SaveChanges();

        var book = context.Books.First(b => b.Id == checkout.BookId);
        var member = context.Members.First(m => m.Id == checkout.MemberId);
        return ToDetail(checkout, book, member);
    }

    public IEnumerable<CheckoutDetail> GetOverdueCheckouts()
    {
        var now = DateTime.UtcNow;
        return context.Checkouts
            .Where(c => c.ReturnedAt == null && c.DueDate < now)
            .AsNoTracking()
            .ToList()
            .Select(c => ToDetail(c,
                context.Books.First(b => b.Id == c.BookId),
                context.Members.First(m => m.Id == c.MemberId)));
    }

    public DashboardStats GetDashboard()
    {
        var checkedOutIds = context.Checkouts
            .Where(c => c.ReturnedAt == null)
            .Select(c => c.BookId)
            .ToHashSet();

        var now = DateTime.UtcNow;
        int overdue = context.Checkouts.Count(c => c.ReturnedAt == null && c.DueDate < now);

        return new DashboardStats(
            TotalBooks: context.Books.Count(),
            AvailableBooks: context.Books.Count() - checkedOutIds.Count,
            CheckedOutBooks: checkedOutIds.Count,
            OverdueCheckouts: overdue,
            TotalMembers: context.Members.Count());
    }

    private static CheckoutDetail ToDetail(Checkout c, Book b, Member m) =>
        new(c.Id, c.CreateDate, b.Id, b.Title, m.Id, m.Name, c.CheckedOutAt, c.DueDate, c.ReturnedAt);
}

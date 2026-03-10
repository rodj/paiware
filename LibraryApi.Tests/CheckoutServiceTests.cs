using Library.Core.Services;
using LibraryApi.Data;
using LibraryApi.Services;

namespace LibraryApi.Tests;

public class CheckoutServiceTests
{
    private static (CheckoutService service, InMemoryDataStore store) BuildService()
    {
        var store = new InMemoryDataStore();
        var service = new CheckoutService(store);
        return (service, store);
    }

    // GetAllBooks

    [Fact]
    public void GetAllBooks_ReturnsAllFiveBooks()
    {
        var (service, _) = BuildService();
        Assert.Equal(5, service.GetAllBooks().Count());
    }

    [Fact]
    public void GetAllBooks_SeededCheckedOutBooksShowUnavailable()
    {
        var (service, _) = BuildService();
        var books = service.GetAllBooks().ToList();
        Assert.False(books.Single(b => b.Id == 2).IsAvailable);
        Assert.False(books.Single(b => b.Id == 4).IsAvailable);
    }

    [Fact]
    public void GetAllBooks_SeededAvailableBooksShowAvailable()
    {
        var (service, _) = BuildService();
        var books = service.GetAllBooks().ToList();
        Assert.True(books.Single(b => b.Id == 1).IsAvailable);
        Assert.True(books.Single(b => b.Id == 3).IsAvailable);
        Assert.True(books.Single(b => b.Id == 5).IsAvailable);
    }

    // CheckOutBook

    [Fact]
    public void CheckOutBook_SucceedsForAvailableBook()
    {
        var (service, _) = BuildService();
        var result = service.CheckOutBook(bookId: 1, memberId: 1);
        Assert.Equal(1, result.BookId);
        Assert.Equal(1, result.MemberId);
    }

    [Fact]
    public void CheckOutBook_SetsDueDateTo14DaysFromNow()
    {
        var (service, _) = BuildService();
        var result = service.CheckOutBook(bookId: 1, memberId: 1);
        var expectedDue = DateTime.UtcNow.AddDays(14);
        Assert.True(Math.Abs((result.DueDate - expectedDue).TotalSeconds) < 5);
    }

    [Fact]
    public void CheckOutBook_MakesBookUnavailable()
    {
        var (service, _) = BuildService();
        service.CheckOutBook(bookId: 1, memberId: 1);
        Assert.False(service.GetAllBooks().Single(b => b.Id == 1).IsAvailable);
    }

    [Fact]
    public void CheckOutBook_ThrowsWhenBookAlreadyCheckedOut()
    {
        var (service, _) = BuildService();
        var ex = Assert.Throws<InvalidOperationException>(
            () => service.CheckOutBook(bookId: 2, memberId: 1));
        Assert.Contains("already checked out", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void CheckOutBook_ThrowsWhenBookNotFound()
    {
        var (service, _) = BuildService();
        Assert.Throws<KeyNotFoundException>(() => service.CheckOutBook(bookId: 99, memberId: 1));
    }

    [Fact]
    public void CheckOutBook_ThrowsWhenMemberNotFound()
    {
        var (service, _) = BuildService();
        Assert.Throws<KeyNotFoundException>(() => service.CheckOutBook(bookId: 1, memberId: 99));
    }

    // ReturnBook

    [Fact]
    public void ReturnBook_SucceedsForActiveCheckout()
    {
        var (service, _) = BuildService();
        var result = service.ReturnBook(checkoutId: 1);
        Assert.NotNull(result.ReturnedAt);
    }

    [Fact]
    public void ReturnBook_MakesBookAvailableAgain()
    {
        var (service, _) = BuildService();
        service.ReturnBook(checkoutId: 1);
        Assert.True(service.GetAllBooks().Single(b => b.Id == 2).IsAvailable);
    }

    [Fact]
    public void ReturnBook_ThrowsWhenAlreadyReturned()
    {
        var (service, _) = BuildService();
        service.ReturnBook(checkoutId: 1);
        var ex = Assert.Throws<InvalidOperationException>(
            () => service.ReturnBook(checkoutId: 1));
        Assert.Contains("already been returned", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ReturnBook_ThrowsWhenCheckoutNotFound()
    {
        var (service, _) = BuildService();
        Assert.Throws<KeyNotFoundException>(() => service.ReturnBook(checkoutId: 99));
    }

    // GetOverdueCheckouts

    [Fact]
    public void GetOverdueCheckouts_IncludesSeedOverdueCheckout()
    {
        var (service, _) = BuildService();
        Assert.Contains(service.GetOverdueCheckouts(), c => c.Id == 1);
    }

    [Fact]
    public void GetOverdueCheckouts_ExcludesActiveCheckout()
    {
        var (service, _) = BuildService();
        Assert.DoesNotContain(service.GetOverdueCheckouts(), c => c.Id == 2);
    }

    [Fact]
    public void GetOverdueCheckouts_ExcludesReturnedCheckouts()
    {
        var (service, _) = BuildService();
        service.ReturnBook(checkoutId: 1);
        Assert.DoesNotContain(service.GetOverdueCheckouts(), c => c.Id == 1);
    }

    // GetDashboard

    [Fact]
    public void GetDashboard_ReflectsInitialSeedState()
    {
        var (service, _) = BuildService();
        var stats = service.GetDashboard();
        Assert.Equal(5, stats.TotalBooks);
        Assert.Equal(3, stats.AvailableBooks);
        Assert.Equal(2, stats.CheckedOutBooks);
        Assert.Equal(1, stats.OverdueCheckouts);
        Assert.Equal(3, stats.TotalMembers);
    }

    [Fact]
    public void GetDashboard_UpdatesAfterCheckout()
    {
        var (service, _) = BuildService();
        service.CheckOutBook(bookId: 1, memberId: 1);
        var stats = service.GetDashboard();
        Assert.Equal(2, stats.AvailableBooks);
        Assert.Equal(3, stats.CheckedOutBooks);
    }
}

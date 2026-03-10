using Library.Core.Models;

namespace LibraryApi.Data;

public class InMemoryDataStore
{
    public List<Book> Books { get; } = [];
    public List<Member> Members { get; } = [];
    public List<Checkout> Checkouts { get; } = [];
    private int _nextCheckoutId = 10;

    public int NextCheckoutId() => _nextCheckoutId++;

    public InMemoryDataStore()
    {
        Seed();
    }

    private void Seed()
    {
        Books.AddRange([
            new Book { Id = 1, Title = "The Pragmatic Programmer", Author = "Andrew Hunt",      Isbn = "978-0135957059" },
            new Book { Id = 2, Title = "Clean Code",               Author = "Robert C. Martin", Isbn = "978-0132350884" },
            new Book { Id = 3, Title = "Design Patterns",          Author = "Gang of Four",     Isbn = "978-0201633610" },
            new Book { Id = 4, Title = "Refactoring",              Author = "Martin Fowler",    Isbn = "978-0134757599" },
            new Book { Id = 5, Title = "The Mythical Man-Month",   Author = "Fred Brooks",      Isbn = "978-0201835953" },
        ]);

        Members.AddRange([
            new Member { Id = 1, Name = "Alice Hinson",  Email = "alice@example.com"  },
            new Member { Id = 2, Name = "Robert Martin", Email = "robert@example.com" },
            new Member { Id = 3, Name = "Rodj O'Matic",  Email = "rodj@example.com"   },
        ]);

        var today = DateTime.UtcNow.Date;

        Checkouts.Add(new Checkout
        {
            Id = 1,
            BookId = 2,
            MemberId = 1,
            CheckedOutAt = today.AddDays(-20),
            DueDate = today.AddDays(-6),
            ReturnedAt = null
        });

        Checkouts.Add(new Checkout
        {
            Id = 2,
            BookId = 4,
            MemberId = 3,
            CheckedOutAt = today.AddDays(-5),
            DueDate = today.AddDays(9),
            ReturnedAt = null
        });
    }
}

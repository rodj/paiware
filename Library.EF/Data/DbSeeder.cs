using Library.Core.Models;
using Microsoft.Extensions.Logging;

namespace Library.EF.Data;

public static class DbSeeder
{
    public static void Seed(LibraryContext context, ILogger logger)
    {
        if (context.Books.Any())
        {
            logger.LogInformation("Database already seeded — skipping.");
            return;
        }

        logger.LogInformation("Seeding database...");

        context.Books.AddRange([
            new Book { Title = "The Pragmatic Programmer", Author = "Andrew Hunt",      Isbn = "978-0135957059" },
            new Book { Title = "Clean Code",               Author = "Robert C. Martin", Isbn = "978-0132350884" },
            new Book { Title = "Design Patterns",          Author = "Gang of Four",     Isbn = "978-0201633610" },
            new Book { Title = "Refactoring",              Author = "Martin Fowler",    Isbn = "978-0134757599" },
            new Book { Title = "The Mythical Man-Month",   Author = "Fred Brooks",      Isbn = "978-0201835953" },
        ]);

        context.Members.AddRange([
            new Member { Name = "Alice Hinson",   Email = "alice@example.com"  },
            new Member { Name = "Robert Martin",  Email = "robert@example.com" },
            new Member { Name = "Rodj O'Matic",   Email = "rodj@example.com"   },
        ]);

        context.SaveChanges();
        logger.LogInformation("Seeded {BookCount} books and {MemberCount} members.", 5, 3);

        // Query back generated IDs — don't assume SQL Server starts at 1
        var cleanCode   = context.Books.First(b => b.Title == "Clean Code");
        var refactoring = context.Books.First(b => b.Title == "Refactoring");
        var alice       = context.Members.First(m => m.Name == "Alice Hinson");
        var carol       = context.Members.First(m => m.Name == "Rodj O'Matic");

        var today = DateTime.UtcNow.Date;

        context.Checkouts.AddRange([
            // Overdue: checked out 20 days ago, due 6 days ago
            new Checkout
            {
                BookId      = cleanCode.Id,
                MemberId    = alice.Id,
                CheckedOutAt = today.AddDays(-20),
                DueDate     = today.AddDays(-6),
                ReturnedAt  = null
            },
            // Active: checked out 5 days ago, due in 9 days
            new Checkout
            {
                BookId      = refactoring.Id,
                MemberId    = carol.Id,
                CheckedOutAt = today.AddDays(-5),
                DueDate     = today.AddDays(9),
                ReturnedAt  = null
            },
        ]);

        context.SaveChanges();
        logger.LogInformation("Seeded 2 checkouts (1 overdue, 1 active). Seeding complete.");
    }
}

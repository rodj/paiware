using Library.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.EF.Data;

public class LibraryContext(DbContextOptions<LibraryContext> options) : DbContext(options)
{
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Checkout> Checkouts => Set<Checkout>();
}

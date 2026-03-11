using Library.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.EF.Data;

public class LibraryContext(DbContextOptions<LibraryContext> options) : DbContext(options)
{
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Checkout> Checkouts => Set<Checkout>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>().Property(b => b.CreateDate).HasColumnType("datetime2(0)").HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<Member>().Property(m => m.CreateDate).HasColumnType("datetime2(0)").HasDefaultValueSql("GETUTCDATE()");
        modelBuilder.Entity<Checkout>().Property(c => c.CreateDate).HasColumnType("datetime2(0)").HasDefaultValueSql("GETUTCDATE()");
    }
}

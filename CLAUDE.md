# Library Book Checkout — CLAUDE.md

## Project Overview

Take-home assignment for pAIwares. Progressive enhancement across git-tagged milestones.

**Spec:** `doc/PaiwaresProgrammingTest API.pdf`

---

## Milestone Plan

| Tag | Contents |
|-----|----------|
| `v1-mvp` | .NET API, mutable in-memory data store, all endpoints, Swagger |
| `v2-tests` | xUnit project, service-layer unit tests |
| `v3-database` | EF Core + SQL Server, migrations, real persistence |
| `v4-react` | React frontend hosted at rodj.me |
| `v5-mcp` | MCP server — AI agent can drive the API as a tool |

---

## V1 MVP — Architecture

### Project layout
```
LibraryApi/
  LibraryApi.csproj
  Program.cs
  Models/
    Book.cs
    Member.cs
    Checkout.cs
  Data/
    InMemoryDataStore.cs     ← singleton, seeded, mutable
  Services/
    ICheckoutService.cs
    CheckoutService.cs
  Controllers/
    BooksController.cs       ← GET /api/books
    CheckoutsController.cs   ← POST /api/checkouts
                                POST /api/checkouts/{id}/return
                                GET  /api/checkouts/overdue
                                GET  /api/dashboard  (bonus)
  DTOs/
    CheckoutRequest.cs       ← { BookId, MemberId }
```

### Key design decisions
- `InMemoryDataStore` is a plain C# class registered as a **singleton**. It holds
  `List<Book>`, `List<Member>`, `List<Checkout>` and seeds them in the constructor.
- `ICheckoutService` / `CheckoutService` holds all business logic. Controllers are thin.
- No EF, no NuGet beyond what ships with `dotnet new webapi`. Zero setup friction.
- Swagger/OpenAPI on by default — serves as the UI for v1.

### Seed data
- **5 books**: 3 available, 2 currently checked out
- **3 members**
- **2 checkouts**: 1 already overdue (checkout ~20 days ago), 1 active (within 14 days)

### Endpoints
| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/books` | All books with `isAvailable` flag |
| POST | `/api/checkouts` | Check out a book `{ bookId, memberId }` |
| POST | `/api/checkouts/{id}/return` | Return a checked-out book |
| GET | `/api/checkouts/overdue` | All overdue, unreturned checkouts |
| GET | `/api/dashboard` | Stats summary (bonus) |

### Business rules
- Checkout rejected (409) if book is already checked out
- Due date = checkout date + 14 days
- Overdue = due date < today AND not yet returned
- Return rejected (404/409) if checkout not found or already returned

---

## V2 Tests — xUnit

**Separate project:** `LibraryApi.Tests/LibraryApi.Tests.csproj`

Tests target `CheckoutService` directly, injecting a controlled `InMemoryDataStore`
pre-seeded with known fixture data. No mocking framework needed — just a helper
that creates a fresh store per test.

### Test cases
1. Checkout succeeds for available book
2. Checkout fails (409) for already-checked-out book
3. Checkout sets due date to today + 14 days
4. Return succeeds for active checkout
5. Return fails for already-returned checkout
6. Overdue list excludes returned checkouts
7. Overdue list excludes checkouts not yet past due date
8. Dashboard stats reflect current state

---

## V3 Database — EF Core + SQL Server

- Add `Microsoft.EntityFrameworkCore.SqlServer` and `Tools` packages
- `LibraryContext : DbContext` replaces `InMemoryDataStore`
- `ICheckoutService` interface unchanged — swap implementation only
- Seed via `modelBuilder.HasData()` or a startup seeder
- `appsettings.json` connection string (LocalDB for portability)
- EF migrations checked in

---

## V4 React Frontend

- React app (Vite + TypeScript)
- Pages: Book list, Checkout form, Overdue list, Dashboard
- Deployed at rodj.me alongside the API
- API base URL configurable via env var

---

## Conventions

- Controllers return `IActionResult` with appropriate HTTP status codes
- All dates are UTC internally; display formatting is a frontend concern
- Keep controllers under ~30 lines each — logic lives in the service

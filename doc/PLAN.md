# Library Book Checkout — Implementation Plan

## The Big Idea

Rather than building everything at once, we implement in progressive milestones,
each committed and tagged in git. This lets us tell a story during the review:
"Here is the minimum viable solution, here is when I added tests, here is when I
added a real database, here is the full-stack version." Each tag is a conversation
piece that demonstrates judgment about tradeoffs and priorities.

---

## Milestones

### v1-mvp — Functional API, No Database

**Goal:** Satisfy every functional requirement as quickly as possible.
**Story:** "All requirements met. Hardcoded seed data. No database, no frontend — just
the API working correctly, demonstrating I know what matters first."

**Stack:**
- .NET Core Web API
- Plain C# in-memory data store (singleton, mutable, seeded on startup)
- Swagger UI as the interactive interface
- No external NuGet packages beyond the default webapi template

**Project layout:**
```
LibraryApi/
  LibraryApi.csproj
  Program.cs
  Models/
    Book.cs
    Member.cs
    Checkout.cs
  Data/
    InMemoryDataStore.cs
  Services/
    ICheckoutService.cs
    CheckoutService.cs
  Controllers/
    BooksController.cs
    CheckoutsController.cs
  DTOs/
    CheckoutRequest.cs
```

**Endpoints:**

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/books` | All books, each with an `isAvailable` flag |
| POST | `/api/checkouts` | Check out a book `{ "bookId": 1, "memberId": 2 }` |
| POST | `/api/checkouts/{id}/return` | Return a checked-out book |
| GET | `/api/checkouts/overdue` | All overdue, unreturned checkouts |
| GET | `/api/dashboard` | Summary stats (bonus) |

**Business rules:**
- Checkout returns `409 Conflict` if the book is already checked out
- Due date = checkout date + 14 days
- Overdue = due date is in the past AND book has not been returned
- Return returns `404` if checkout not found, `409` if already returned

**Seed data:**
- 5 books (3 available, 2 checked out)
- 3 members
- 2 checkouts: one already overdue (~20 days ago), one active (within 14 days)

**Key design decision:** `InMemoryDataStore` is injected into `CheckoutService` via
constructor DI and registered as a singleton in `Program.cs`. This means:
- State persists across requests within a session (checkout a book → it shows unavailable)
- Tests can inject a fresh, known-state store without any mocking framework

---

### v2-tests — xUnit Unit Tests

**Goal:** Add a test suite covering all core business logic.
**Story:** "With the interface already in place, adding tests required no structural
refactoring — just a test project and fixtures."

**Project:** `LibraryApi.Tests/LibraryApi.Tests.csproj`

**Approach:** Tests instantiate `CheckoutService` directly, passing a fresh
`InMemoryDataStore` pre-seeded with controlled fixture data. No mocking framework
needed — the constructor injection pattern from v1 makes this natural.

**Test cases:**
1. Checkout succeeds for an available book
2. Checkout fails (409) if book is already checked out
3. Checkout sets due date to today + 14 days
4. Return succeeds for an active checkout
5. Return fails if checkout is already returned
6. Overdue list excludes returned checkouts
7. Overdue list excludes checkouts not yet past due date
8. Dashboard stats reflect current data store state

---

### v3-database — EF Core + SQL Server

**Goal:** Replace the in-memory store with real persistence.
**Story:** "Because the service layer was behind an interface, swapping in a real
database only touched the data layer. Controllers and tests were unchanged."

**Changes:**
- Add `Microsoft.EntityFrameworkCore.SqlServer` package
- Add `LibraryContext : DbContext`
- New implementation: `EfCheckoutService` implementing `ICheckoutService`
- EF migrations checked in
- Connection string in `appsettings.json` (LocalDB for portability)
- Seed data via EF `HasData()` or a startup seeder

---

### v4-react — React Frontend

**Goal:** Full-stack React frontend deployed at rodj.me, talking directly to the live API.
**Story:** "The API was already complete and tested. The frontend is a clean layer on top,
and the whole thing is live at rodj.me for the reviewer to see."

**Frontend stack:**
- **Vite** — build tool (fast dev server, optimized production builds)
- **React + TypeScript** — component framework
- **Tailwind CSS** — utility-first styling, no custom CSS files needed
- **TanStack Query (React Query)** — server state management: handles loading/error
  states, caching, and refetching automatically
- **React Router v6** — client-side navigation between views
- **Native fetch** — HTTP calls (TanStack Query wraps these cleanly)

**Views:**
- Book list with availability badges
- Check out a book (dropdowns for book + member selection)
- Return a book
- Overdue checkouts list
- Dashboard stats summary

**Project location:** `LibraryReact/` at repo root (sibling to `LibraryApi/`)

**API base URL:** Configured via Vite env variable (`VITE_API_BASE_URL`).
In production: `https://rodj.me/LibraryApi/api`

---

#### Hosting — rodj.me (Winhost)

Hosting is Winhost shared IIS hosting. See reference projects for full credentials:
- `C:\Dropbox\ARj\rjmono\RjUtil\RjCoreWebAPI\CLAUDE.md` — existing .NET API deployment example
- `C:\Dropbox\ARj\rjmono\Web\RodjWeb\Rodj.me\CLAUDE.md` — FTP credentials, SSL details

**Key hosting facts:**
- FTP: `ftp.rodj.me`, user `rodjme00`, password in reference CLAUDE.md
- SQL Server: `s11.winhost.com`, DB `DB_6218_Rodj` (credentials in reference CLAUDE.md)
- SSL active at `https://rodj.me` (Let's Encrypt, expires 2026-05-23)

**⚠️ .NET version concern:** Winhost confirmed .NET 9.0 support as of Feb 2026.
Our project targets .NET 10. Before deploying, verify Winhost .NET 10 support, or
retarget to `net9.0` for the deployment branch.

**API deployment:**
1. `dotnet publish -c Release`
2. FTP upload to `/LibraryApi/` on rodj.me
3. Configure `/LibraryApi/` as an IIS Application in Winhost control panel
4. Live at `https://rodj.me/LibraryApi/api/`

**React deployment:**
1. `vite build` → outputs to `LibraryReact/dist/`
2. FTP upload `dist/` contents to `/library/` on rodj.me
3. Live at `https://rodj.me/library/`

**CORS:** API and React app are on the same domain (`rodj.me`) — no CORS configuration
needed. If testing locally (React dev server on localhost:5173 calling the live API),
CORS will need to be enabled for localhost in development only.

**`appsettings.Production.json`** (not committed — contains live DB credentials):
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=s11.winhost.com;Database=DB_6218_Rodj;User Id=...;Password=..."
  }
}
```

---

### v5-mcp — MCP Server

**Goal:** Expose the Library API as an MCP (Model Context Protocol) server so an AI
agent can drive it directly as a tool.

**Story:** "Scalar's 'Generate MCP' button hinted at this. MCP is Anthropic's open
standard for connecting AI assistants to external tools. With a small amount of
additional work, an AI agent — like Claude — can check out books, list overdue items,
and query the dashboard without any human clicking."

**What MCP is:**
Model Context Protocol is an open standard created by Anthropic. It defines a
structured way for AI models to discover and invoke external tools. An MCP server
wraps an existing API and advertises its capabilities in a format AI agents understand.

**Approach:**
- Use Scalar's "Generate MCP" output as a starting point
- Configure and host an MCP server that wraps the Library API endpoints
- Demonstrate an AI agent (Claude) successfully calling the API as a tool

**Why this is relevant to pAIwares:**
The company name and focus are AI-native. Demonstrating that the API is not just
human-usable but also AI-agent-usable is a natural and impressive final step.

---

## What Stays Stable Across All Milestones

- `ICheckoutService` interface — defined in v1, never changed
- All endpoint routes and HTTP semantics
- Business rules (14-day due date, 409 on double-checkout, overdue definition)
- The test suite (v2 tests still pass in v3 and v4)

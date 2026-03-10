using Library.Core.Services;
using LibraryApi.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController(ICheckoutService checkoutService) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll() => Ok(checkoutService.GetAllBooks());

    /// <summary>
    /// Adds a new book to the library. Note: a real application would also include
    /// PUT (update) and DELETE endpoints, but those were outside the original spec.
    /// </summary>
    [HttpPost]
    public IActionResult AddBook([FromBody] AddBookRequest request)
    {
        var result = checkoutService.AddBook(request.Title, request.Author, request.Isbn);
        return CreatedAtAction(nameof(GetAll), result);
    }
}

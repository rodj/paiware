using LibraryApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController(ICheckoutService checkoutService) : ControllerBase
{
    [HttpGet]
    public IActionResult GetAll() => Ok(checkoutService.GetAllBooks());
}

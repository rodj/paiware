using LibraryApi.DTOs;
using LibraryApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace LibraryApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CheckoutsController(ICheckoutService checkoutService) : ControllerBase
{
    [HttpPost]
    public IActionResult CheckOut([FromBody] CheckoutRequest request)
    {
        try
        {
            var result = checkoutService.CheckOutBook(request.BookId, request.MemberId);
            return CreatedAtAction(nameof(CheckOut), result);
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        catch (InvalidOperationException ex) { return Conflict(ex.Message); }
    }

    [HttpPost("{id}/return")]
    public IActionResult Return(int id)
    {
        try
        {
            var result = checkoutService.ReturnBook(id);
            return Ok(result);
        }
        catch (KeyNotFoundException ex) { return NotFound(ex.Message); }
        catch (InvalidOperationException ex) { return Conflict(ex.Message); }
    }

    [HttpGet("overdue")]
    public IActionResult GetOverdue() => Ok(checkoutService.GetOverdueCheckouts());

    [HttpGet("dashboard")]
    public IActionResult GetDashboard() => Ok(checkoutService.GetDashboard());
}

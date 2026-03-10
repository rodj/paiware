namespace LibraryApi.DTOs;

public class CheckoutRequest
{
    public int BookId { get; set; }
    public int MemberId { get; set; }
}

namespace LibraryApi.DTOs;

public class AddBookRequest
{
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public string Isbn { get; set; } = "";
}

namespace LibraryManagement.DTOs
{
    public class BookDto
    {
        public int BookId { get; set; }

        public string BookName { get; set; } = null!;

        public string BookAuthor { get; set; } = null!;

        public int NoOfBook { get; set; }

        public decimal Price { get; set; }

        public IFormFile BookImage { get; set; }
    }
}

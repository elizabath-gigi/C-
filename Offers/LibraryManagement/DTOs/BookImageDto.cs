namespace LibraryManagement.DTOs
{
    public class BookImageDto
    {
        public int BookId { get; set; }

        public string BookName { get; set; } = null!;

        public string BookAuthor { get; set; } = null!;

        public int NoOfBook { get; set; }

        public decimal Price { get; set; }

        public string BookImage { get; set; }
        public string Description { get; set; }
        public decimal? OfferPrice { get; set; }
        public int? SaleId { get; set; }

    }
}

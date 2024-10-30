using System;
using System.Collections.Generic;

namespace LibraryManagement.Models;

public partial class Book
{
    public int BookId { get; set; }

    public string BookName { get; set; } = null!;

    public string BookAuthor { get; set; } = null!;

    public int NoOfBook { get; set; }

    public decimal Price { get; set; }

    public byte[]? BookImage { get; set; }

    public string? Description { get; set; }

    public int? SaleId { get; set; }

    public decimal? OfferPrice { get; set; }

    public virtual Sale? Sale { get; set; }
}

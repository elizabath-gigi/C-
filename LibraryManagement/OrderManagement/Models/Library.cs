using System;
using System.Collections.Generic;

namespace OrderManagement.Models;

public partial class Library
{
    public int BookId { get; set; }

    public string? BookAuthor { get; set; }

    public string? BookName { get; set; }

    public int NoOfBook { get; set; }
}

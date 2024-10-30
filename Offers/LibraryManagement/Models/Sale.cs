using System;
using System.Collections.Generic;

namespace LibraryManagement.Models;

public partial class Sale
{
    public int SaleId { get; set; }

    public string SaleName { get; set; } = null!;

    public string SaleDescription { get; set; } = null!;

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}

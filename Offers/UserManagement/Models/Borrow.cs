using System;
using System.Collections.Generic;

namespace UserManagement.Models;

public partial class Borrow
{
    public int BorrowId { get; set; }

    public int UserId { get; set; }

    public string BookName { get; set; } = null!;

    public DateOnly BorrowDate { get; set; }

    public DateOnly? ReturnDate { get; set; }

    public int? IsReturned { get; set; }

    public virtual User User { get; set; } = null!;
}

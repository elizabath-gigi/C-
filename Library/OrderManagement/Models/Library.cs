using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Models;

public partial class Library
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int BookId { get; set; }

    public string? BookAuthor { get; set; }

    public string? BookName { get; set; }

    public int NoOfBook { get; set; }
}

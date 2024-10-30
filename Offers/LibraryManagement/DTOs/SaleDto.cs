namespace LibraryManagement.DTOs
{
    public class SaleDto
    { 
    public int SaleId { get; set; }

    public string SaleName { get; set; } = null!;

    public string SaleDescription { get; set; } = null!;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

     }
}

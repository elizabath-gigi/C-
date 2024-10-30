namespace UserManagement.DTOs
{
    public class BorrowUserDto
    {
        public int BorrowId { get; set; }
        public string UserName { get; set; }
        public string BookName { get; set; } = null!;

        public DateOnly BorrowDate { get; set; }

        public DateOnly? ReturnDate { get; set; }

        public int? IsReturned { get; set; }
    }
}

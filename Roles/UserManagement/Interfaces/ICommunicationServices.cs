using UserManagement.DTOs;

namespace LibraryManagement.Interfaces
{
    public interface ICommunicationServices
    {
        public Task<BookDto> GetBook(string BookName);
        public Task<string> UpdateNoOfBook(BookDto book, bool increase);
        public  Task<int?> CheckNoOfBook(string BookName);
    }
}

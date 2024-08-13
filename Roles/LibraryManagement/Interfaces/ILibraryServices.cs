using LibraryManagement.Models;

namespace LibraryManagement.Interfaces
{
    public interface ILibraryServices
    {

        public Task<List<Book>> GetBooksInPages(int page, int pageSize);
        public Task<List<Book>> GetBooks();
        public Task<Book> GetBook(int id);
        public Task<Book> AddBook(Book request);
        public Task<Book> UpdateBook(Book request);
        public Task<Book> DeleteBook(int id);
        public Task<string> BulkUploadDynamic(IFormFile file);

    }
}

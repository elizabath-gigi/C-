using LibraryManagement.DTOs;
using LibraryManagement.Models;

namespace LibraryManagement.Interfaces
{
    public interface ILibraryServices
    {

        public Task<List<Book>> GetBooksInPages(int page, int pageSize);
        public Task<List<BookSaleDto>> GetBooksAdmin();
        public Task<BookImageDto> GetBook(int id);
        public Task<BookImageDto> GetByBookName(string BookName);
        public Task<Book> AddBook(BookDto request);
        public Task<BookDto> UpdateBook(BookDto request);
        public Task<Book> DeleteBook(int id);
        public Task<string> BulkUploadDynamic(IFormFile file);
        public Task<Book> UpdateNoOfBook(string BookName, int NoOfBook);
        public Task<List<BookImageDto>> Search(string searchKey);
        public Task<List<BookImageDto>> GetBooksUser();


    }
}

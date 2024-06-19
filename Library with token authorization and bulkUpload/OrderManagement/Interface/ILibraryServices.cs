using OrderManagement.Models;

namespace OrderManagement.Interface
{
    public interface ILibraryServices
    {
        public Task<string> addBooksFromFile(string fileName);
        public  Task<List<Library>> GetBooks();
        public  Task<Library> GetBook(int id);
        public Task<Library> AddBook(Library request);
        public Task<Library> UpdateBook(Library request);
        public Task<Library> DeleteBook(int id);
       

    }
}

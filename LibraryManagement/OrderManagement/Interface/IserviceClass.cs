using OrderManagement.Models;

namespace OrderManagement.Interface
{
    public interface IserviceClass
    {
        public  Task<List<Library>> GetBooks();
        public  Task<Library> GetBook(int id);
        //public Task<List<Library>> GetBooks();
        public Task<List<Library>> AddBook(Library request);
        public Task<List<Library>> UpdateBook(Library request);
        public Task<Library> DeleteBook(int id);
        //public Task<Library> GetBook(int id);
        //public Task<List<Library>> AddBook(Library request);
        //public Task<List<Library>> UpdateBook(Library request);
        //public Task<List<Library>> DeleteBook(int id);

    }
}

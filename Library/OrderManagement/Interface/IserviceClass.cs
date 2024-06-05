using OrderManagement.Models;

namespace OrderManagement.Interface
{
    public interface IserviceClass
    {
        public  Task<List<Library>> GetBooks();
        public  Task<Library> GetBook(int id);
        //public Task<List<Library>> GetBooks();
        public Task<Library> AddBook(Library request);
        public Task<Library> UpdateBook(Library request);
        public Task<Library> DeleteBook(int id);
       

    }
}

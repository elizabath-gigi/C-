using OrderManagement.Model;

namespace OrderManagement.Services
{
    public interface IserviceClass
    {
        public Task<List<Books>> GetBooks();
        public Task<Books> GetBook(int id);
        public Task<List<Books>> AddBook(Books request);
        public Task<List<Books>> UpdateBook(Books request);
        public Task<List<Books>> DeleteBook(int id);

    }
}

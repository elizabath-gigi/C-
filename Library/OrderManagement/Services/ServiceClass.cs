using Microsoft.EntityFrameworkCore;
using OrderManagement.Interface;
using OrderManagement.Models;
//using OrderManagement.Controllers;


namespace OrderManagement.Services
{
    public class ServiceClass : IserviceClass
    {
        private readonly LibraryContext _libraryContext;

        /*public static List<Library> books = new List<Library>()
        {
                new Library()
                {
                    BookId = 1,
                    BookName="Alice in Wonderland",
                    BookAuthor="Lewis Caroll",
                    NoOfBook=10
                },
                 new Library()
                {
                    BookId = 2,
                    BookName="Pride nad Prejudice",
                    BookAuthor="Jane Austin",
                    NoOfBook=15
                 }
        };*/
        private static List<Library> books = new List<Library>();
        public ServiceClass(LibraryContext libraryContext)
        {
            _libraryContext = libraryContext;
        }
        public  async Task<List<Library>> GetBooks()
        {
            return await   _libraryContext.Libraries.ToListAsync();
        }
        public async Task<Library> GetBook(int id)
        {
            var book = _libraryContext.Libraries.FirstOrDefault(x => x.BookId == id);
            //return await Task.FromResult(book);
            return  book;
        }

        public async Task<Library> AddBook(Library request)
        {
            /* var existing = _libraryContext.Libraries.FirstOrDefault(x => x.BookId == request.BookId);
             if (existing != null)
             {
                 return null;
             }*/
            

            _libraryContext.Add(request);
            _libraryContext.SaveChanges();
            
            //_libraryContext.Add(request);
            //_libraryContext.SaveChanges();
           // books.Add(request);
            return request;
        }

        public async Task<Library> UpdateBook(Library request)
        {
            var existing = _libraryContext.Libraries.FirstOrDefault(x => x.BookId == request.BookId);
            if (existing == null)
            {
                return null;
            }
            existing.NoOfBook = request.NoOfBook;
            existing.BookName = request.BookName;
            existing.BookAuthor = request.BookAuthor;
            _libraryContext.SaveChanges();
            return request;
        }

        public async Task<Library> DeleteBook(int id)
        {
            var book = _libraryContext.Libraries.FirstOrDefault(x => x.BookId == id);
            //return await Task.FromResult(book);
            if (book == null)
            {
                return null;
            }
            _libraryContext.Libraries.Remove(book);
            _libraryContext.SaveChanges();
            return book;
        }
    
    }
}


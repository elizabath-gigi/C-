using Microsoft.EntityFrameworkCore;
using OrderManagement.Exceptions;
using OrderManagement.Interface;
using OrderManagement.Models;



namespace OrderManagement.Services
{
    public class LibraryServices : ILibraryServices
    {
        private readonly LibraryContext _libraryContext;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LibraryServices));
       

        
        public LibraryServices(LibraryContext libraryContext)
        {
            _libraryContext = libraryContext;
        }
        public  async Task<List<Library>> GetBooks()
        {
            var book=await _libraryContext.Libraries.ToListAsync();
            if(book.Count== 0)
            {
                log.Debug("The library DB is null");
            }
            log.Info("The contents of the Library DB is retrieved");
            return book;
           
        }
        public async Task<Library> GetBook(int id)
        {
            var book = _libraryContext.Libraries.FirstOrDefault(x => x.BookId == id);
            if(book==null)
            {
                log.Debug("The book doesn't exist in the DB, so retrieval failed");
                throw new IdNotFoundException("The book doesn't exist");
            }
            log.Info("The details of the book is retrieved");
            return  book;
           
        }

        public async Task<Library> AddBook(Library request)
        { 
            _libraryContext.Add(request);
            _libraryContext.SaveChanges();
            log.Info("The details of the book is added successfully to DB");
            
            return request;
        }

        public async Task<Library> UpdateBook(Library request)
        {
            var existing = _libraryContext.Libraries.FirstOrDefault(x => x.BookId == request.BookId);
            if (existing == null)
            {
                log.Debug("The book is not found from DB, so update failed");
                throw new IdNotFoundException("The book doesn't exist.");
              
            }
            existing.NoOfBook = request.NoOfBook;
            existing.BookName = request.BookName;
            existing.BookAuthor = request.BookAuthor;
            _libraryContext.SaveChanges();
            log.Info("The details of the book is updated successfully to DB");
            return request;
        }

        public async Task<Library> DeleteBook(int id)
        {
            var book = _libraryContext.Libraries.FirstOrDefault(x => x.BookId == id);
      
            if (book == null)
            {
                log.Debug("The book is not found from DB, delete failed");
                throw new IdNotFoundException("The book doesn't exist.");
            }
            _libraryContext.Libraries.Remove(book);
            _libraryContext.SaveChanges();
            log.Info("The details of the book is deleted successfully from DB");
            return book;
        }
    
    }
}


using Microsoft.EntityFrameworkCore;
using OrderManagement.Exceptions;
using OrderManagement.Interface;
using OrderManagement.Models;
using System.Globalization;
using CsvHelper;



namespace OrderManagement.Services
{
    public class LibraryServices : ILibraryServices
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly LibraryContext _libraryContext;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LibraryServices));
       

        
        public LibraryServices(LibraryContext libraryContext,IWebHostEnvironment webHostEnvironment)
        {
            _libraryContext = libraryContext;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<string> addBooksFromFile(string fileName)
        {
            // Get the current directory path of the web application
            string currentDirectory = _webHostEnvironment.ContentRootPath;

            // Combine the current directory path with the filename
            string filePath = Path.Combine(currentDirectory,fileName);

            if (string.IsNullOrEmpty(filePath))
            {
                log.Debug("File name is null or empty, Bulk Upload failed");
                throw new ArgumentException("File name cannot be null or empty.", fileName);
                
            }

            if (!File.Exists(filePath))
            {
                log.Debug("File doesn't exist, Bulk Upload failed");
                throw new ArgumentException("File does not exist at the specified path.", fileName);
            }
            var books = new List<Library>();

            string csvData = File.ReadAllText(filePath);
            foreach (string row in csvData.Split('\n'))
            {
                if (!string.IsNullOrEmpty(row))
                {
                    string[] columns = row.Split(',');
                    Library item = new Library
                    {
                        BookName = columns[0].Trim(),
                        BookAuthor = columns[1].Trim(),
                        NoOfBook = int.Parse(columns[2].Trim())
                    };
                    books.Add(item);

                }
            }
            await _libraryContext.BulkInsertAsync(books);
            _libraryContext.SaveChanges();
            log.Debug("Books added from "+fileName+"and added to DB successfully.");
            return "Added successfully";
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


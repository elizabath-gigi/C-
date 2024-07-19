using Microsoft.Data.SqlClient;
using System.Data;
using System.Formats.Asn1;
using System;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using log4net;
using static System.Reflection.Metadata.BlobBuilder;
using System.Reflection;
using CsvHelper.TypeConversion;
using System.ComponentModel;
using NullableConverter = System.ComponentModel.NullableConverter;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using log4net.Core;
using System.Data;
using static System.Runtime.InteropServices.JavaScript.JSType;
using LibraryManagement.Models;
using LibraryManagement.Exceptions;
using LibraryManagement.Interfaces;

namespace LibraryManagement.Services
{
    public class LibraryServices:ILibraryServices
    {
  
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly LibraryManagementContext _libraryContext;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LibraryServices));
        private readonly IHttpContextAccessor _httpContextAccessor;
        //private  string _userId;

        public LibraryServices(LibraryManagementContext libraryContext, IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _libraryContext = libraryContext;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
            // Set the UserId in log4net's global context
            //_userId = _httpContextAccessor.HttpContext.Session.GetString("UserId");
            //ThreadContext.Properties["UserId"] = _userId;
        }
        /// <summary>
        /// Add books from CSV file to the Database(Bulk Upload).
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns> Task<string> </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="CSVException"></exception>

       
        /// <summary>
        /// Get the details about all the books from the DB.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Book>> GetBooksInPages(int page, int pageSize)
        {
            var books = await _libraryContext.Books.ToListAsync();
            var totalNumberOfBooks = books.Count;
            var totalPages = (int)Math.Ceiling((decimal)totalNumberOfBooks / pageSize);
            if (page > totalPages)
            {
                log.Debug("The page number exceeds the limit, retrieval failed");
                //CallStoredProcedureAsync("DEBUG", "The page number exceeds the limit, retrieval failed");
                throw new ArgumentsException("The page number exceeds the limit");

            }
            var bookPerPage = books
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToList();

            log.Info("The contents of the Library DB from page " + page + " is retrieved");
           // CallStoredProcedureAsync("INFO", "The contents of the Library DB from page " + page + " is retrieved");
            return bookPerPage;


        }
        public async Task<List<Book>> GetBooks()
        {
            var book = await _libraryContext.Books.ToListAsync();
            if (book.Count == 0)
            {
                log.Debug("The library DB is null");
                //CallStoredProcedureAsync("DEBUG", "The library DB is null");
            }
            log.Info("Details of Books retrieved successfully");
            //CallStoredProcedureAsync("INFO","Details of Books retrieved successfully");
            return book;


        }
        public async Task<Book> GetBook(int id)
        {
            var book = _libraryContext.Books.FirstOrDefault(x => x.BookId == id);
            if (book == null)
            {
                log.Debug("The book doesn't exist in the DB, so retrieval failed");
                //CallStoredProcedureAsync("INFO", "The book doesn't exist in the DB, so retrieval failed");
                throw new IdNotFoundException("The book doesn't exist");
            }
            log.Info("The details of the book is retrieved");
            //CallStoredProcedureAsync("INFO", "The details of the book is retrieved");
            return book;

        }
        /// <summary>
        /// Add the details of a book to the DB.
        /// </summary>
        /// <param name="request"></param>
        /// <returns> Task<Library> </returns>
        public async Task<Book> AddBook(Book request)
        {
            _libraryContext.Add(request);
            _libraryContext.SaveChanges();
            log.Info("The details of the book is added successfully to DB");
            //CallStoredProcedureAsync("INFO", "The details of the book is added successfully to DB");
            return request;
        }
        /// <summary>
        /// Update the details of a book in DB.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Task<Library></returns>
        /// <exception cref="IdNotFoundException"></exception>
        public async Task<Book> UpdateBook(Book request)
        {
            var existing = _libraryContext.Books.FirstOrDefault(x => x.BookId == request.BookId);
            if (existing == null)
            {
                log.Debug("The book is not found from DB, so update failed");
                //CallStoredProcedureAsync("DEBUG", "The book is not found from DB, so update failed");
                throw new IdNotFoundException("The book doesn't exist.");

            }
            existing.NoOfBook = request.NoOfBook;
            existing.BookName = request.BookName;
            existing.BookAuthor = request.BookAuthor;
            _libraryContext.SaveChanges();
            log.Info("The details of the book is updated successfully to DB");
            //CallStoredProcedureAsync("INFO", "The details of the book is added successfully to DB");
            return request;
        }
        /// <summary>
        /// Delete the details of the book from the DB.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Task<Library></returns>
        /// <exception cref="IdNotFoundException"></exception>DeletedBo
        public async Task<Book> DeleteBook(int id)
        {
            var book = _libraryContext.Books.FirstOrDefault(x => x.BookId == id);

            if (book == null)
            {
                log.Debug("The book is not found from DB, delete failed");
                //CallStoredProcedureAsync("INFO", "The book is not found from DB, delete failed");
                throw new IdNotFoundException("The book doesn't exist.");
            }
            _libraryContext.Books.Remove(book);
            _libraryContext.SaveChanges();
            log.Info("The details of the book is deleted successfully from DB");
            //CallStoredProcedureAsync("INFO", "The details of the book is deleted successfully from DB");
            return book;
        }
        /*public void CallStoredProcedureAsync(string level, string message)
        {
            try
            {
                //var userIdString = _httpContextAccessor.HttpContext.Session.GetString("UserId");
                //int.TryParse(userIdString, out int userId);
                var user = _httpContextAccessor.HttpContext.Items["User"] as User;
                var userId = user.Id.ToString();
                var logDate = DateTime.UtcNow;
                var thread = Thread.CurrentThread.ManagedThreadId.ToString();
                var logger = nameof(LibraryServices);


                var userIdParameter = new SqlParameter("@UserId", SqlDbType.Int) { Value = userId };
                var dateParameter = new SqlParameter("@LogDate", SqlDbType.DateTime) { Value = logDate };
                var threadParameter = new SqlParameter("@Thread", SqlDbType.NVarChar, 255) { Value = thread };
                var levelParameter = new SqlParameter("@Level", SqlDbType.NVarChar, 50) { Value = level };
                var loggerParameter = new SqlParameter("@Logger", SqlDbType.NVarChar, 255) { Value = logger };
                var messageParameter = new SqlParameter("@Message", SqlDbType.NVarChar, 4000) { Value = message };

                _libraryContext.Database.ExecuteSqlRaw(
                   "EXEC Logging @UserId, @LogDate, @Thread, @Level, @Logger, @Message",
                   userIdParameter, dateParameter, threadParameter, levelParameter, loggerParameter, messageParameter
               );
            }
            catch
            {
                log.Debug("Some errors with stored procedure");
            }

        }*/







    }
}

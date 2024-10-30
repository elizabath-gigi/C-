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
using LibraryManagement.DTOs;


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
                CallStoredProcedureAsync("DEBUG", "The page number exceeds the limit, retrieval failed");
                throw new ArgumentsException("The page number exceeds the limit");

            }
            var bookPerPage = books
                               .Skip((page - 1) * pageSize)
                               .Take(pageSize)
                               .ToList();


            log.Info("The contents of the Library DB from page " + page + " is retrieved");
            CallStoredProcedureAsync("INFO", "The contents of the Library DB from page " + page + " is retrieved");

            return bookPerPage;


        }
        
        public async Task<List<BookImageDto>>  GetBooksUser()
        {
            var books = await _libraryContext.Books.ToListAsync();
            if (books.Count == 0)
            {
                log.Debug("The library DB is null");
                CallStoredProcedureAsync("DEBUG", "The library DB is null");
            }

            log.Info("Details of Books retrieved successfully");
            CallStoredProcedureAsync("INFO", "Details of Books retrieved successfully");
            var today = DateTime.Today;
             
            var activeSale = await _libraryContext.Sales.Where(s => today >= s.StartDate && today <= s.EndDate).FirstOrDefaultAsync();

            var saleId = activeSale?.SaleId;
            var bookImageDtos = books.Select(book => new BookImageDto
            {
                BookId = book.BookId,
                BookName = book.BookName,
                BookAuthor = book.BookAuthor,
                NoOfBook = book.NoOfBook,
                Price = book.Price,
                BookImage = book.BookImage != null ? Convert.ToBase64String(book.BookImage) : null,
                Description = book.Description,
                OfferPrice = (saleId != null && saleId == book.SaleId) ? book.OfferPrice : 0 ,
                SaleId=book.SaleId
            }).ToList();

            return bookImageDtos;
        }
        public async Task<List<BookSaleDto>> GetBooksAdmin()
        {
            var books = await _libraryContext.Books.ToListAsync();
            if (books.Count == 0)
            {
                log.Debug("The library DB is null");
                CallStoredProcedureAsync("DEBUG", "The library DB is null");
            }
          
            log.Info("Details of Books retrieved successfully");
            CallStoredProcedureAsync("INFO","Details of Books retrieved successfully");
            //var today = DateOnly.FromDateTime(DateTime.Today);
            // Get the active sale based on today's date
            //var activeSale = await _libraryContext.Sales.Where(s => today >= s.StartDate && today <= s.EndDate).FirstOrDefaultAsync();

            // Retrieve the SaleId if an active sale exists
            //var saleId = activeSale?.SaleId;  // This will be null if no active sale exists

            var bookImageDtos = (from book in books
                                 join sale in _libraryContext.Sales on book.SaleId equals sale.SaleId
                                 select new BookSaleDto
                                 {
                                     BookId = book.BookId,
                                     BookName = book.BookName,
                                     BookAuthor = book.BookAuthor,
                                     NoOfBook = book.NoOfBook,
                                     Price = book.Price,
                                     BookImage = book.BookImage != null ? Convert.ToBase64String(book.BookImage) : null,
                                     Description = book.Description,
                                     OfferPrice = book.OfferPrice,  
                                     SaleId = book.SaleId,
                                     SaleName=sale.SaleName,
                                     StartDate=sale.StartDate,
                                     EndDate=sale.EndDate,

                                 }).ToList();

            return bookImageDtos;
        }
        public async Task<BookImageDto> GetBook(int id)
        {
            var book = await _libraryContext.Books.FirstOrDefaultAsync(x => x.BookId == id);
            if (book == null)
            {
                log.Debug("The book doesn't exist in the DB, so retrieval failed");
                CallStoredProcedureAsync("INFO", "The book doesn't exist in the DB, so retrieval failed");
                throw new IdNotFoundException("The book doesn't exist");
            }

            // Convert the image byte[] to a base64 string
            string base64Image = null;
            if (book.BookImage != null && book.BookImage.Length > 0)
            {
                base64Image = Convert.ToBase64String(book.BookImage);
            }

            var bookDto = new BookImageDto
            {
                BookId = book.BookId,
                BookName = book.BookName,
                BookAuthor = book.BookAuthor,
                NoOfBook = book.NoOfBook,
                Price = book.Price,
                BookImage = base64Image ,
                Description=book.Description
            };

            log.Info("The details of the book are retrieved");
            CallStoredProcedureAsync("INFO", "The details of the book are retrieved");

            return bookDto;
        }

        public async Task<BookImageDto> GetByBookName(string BookName)
        {
            var book = _libraryContext.Books.FirstOrDefault(x => x.BookName == BookName);
            if (book == null)
            {
                log.Debug("The book doesn't exist in the DB, so retrieval failed");
                CallStoredProcedureAsync("INFO", "The book doesn't exist in the DB, so retrieval failed");
                throw new IdNotFoundException("The book doesn't exist");
            }
            string base64Image = null;
            if (book.BookImage != null && book.BookImage.Length > 0)
            {
                base64Image = Convert.ToBase64String(book.BookImage);
            }

            var bookDto = new BookImageDto
            {
                BookId = book.BookId,
                BookName = book.BookName,
                BookAuthor = book.BookAuthor,
                NoOfBook = book.NoOfBook,
                Price = book.Price,
                BookImage = base64Image,
                Description = book.Description
            };

            log.Info("The details of the book is retrieved");
            CallStoredProcedureAsync("INFO", "The details of the book is retrieved");
            return bookDto;

        }
        
            public async Task<List<BookImageDto>> Search(string searchKey)
        {
            if(searchKey==null)
            {
                throw new ArgumentsException("Please enter a Search key");
            }
            var books=new List<BookImageDto>();
            foreach(var book in _libraryContext.Books)
            {
                if(book.BookName.Contains(searchKey,StringComparison.OrdinalIgnoreCase)||
                         book.BookAuthor.Contains(searchKey, StringComparison.OrdinalIgnoreCase))
                {
                    string base64Image = null;
                    if (book.BookImage != null && book.BookImage.Length > 0)
                    {
                        base64Image = Convert.ToBase64String(book.BookImage);
                    }
                    var bookDto = new BookImageDto
                    {
                        BookId = book.BookId,
                        BookName = book.BookName,
                        BookAuthor = book.BookAuthor,
                        NoOfBook = book.NoOfBook,
                        Price = book.Price,
                        BookImage = base64Image,
                        Description=book.Description
                    };
                    books.Add(bookDto);
                }
                
            }
           if(books.Count==0)
            {
                throw new ArgumentsException("No results found");

            }
            
            return books;
        }
        /// <summary>
        /// Add the details of a book to the DB.
        /// </summary>
        /// <param name="request"></param>
        /// <returns> Task<Library> </returns>
        public async Task<Book> AddBook(BookDto request)
        {
            var existing = _libraryContext.Books.FirstOrDefault(x => x.BookName == request.BookName && x.BookAuthor == request.BookAuthor);

            
            byte[] imageData = null;
            if (request.BookImage != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await request.BookImage.CopyToAsync(memoryStream);
                    imageData = memoryStream.ToArray();
                }
            }

            Book newBook = null; 

            if (existing != null)
            {
                existing.NoOfBook = request.NoOfBook + existing.NoOfBook;
                existing.BookImage = imageData;
            }
            else
            {
                newBook = new Book
                {
                    BookName = request.BookName,
                    BookAuthor = request.BookAuthor,
                    NoOfBook = request.NoOfBook,
                    BookImage = imageData,
                    Price = request.Price,
                    Description=request.Description,
                    SaleId=request.SaleId,
                    OfferPrice=request.OfferPrice,
                                         
                };
                _libraryContext.Books.Add(newBook);
            }

            await _libraryContext.SaveChangesAsync();
            log.Info("The details of the book are added successfully to the DB");
            CallStoredProcedureAsync("INFO", "The details of the book are added successfully to the DB");

            return existing ?? newBook;
        }


        /// <summary>
        /// Update the details of a book in DB.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Task<Library></returns>
        /// <exception cref="IdNotFoundException"></exception>
        public async Task<BookDto> UpdateBook(BookDto request)
        {
            var existing = _libraryContext.Books.FirstOrDefault(x => x.BookId == request.BookId);
            if (existing == null)
            {
                log.Debug("The book is not found in the DB, so the update failed");
                throw new IdNotFoundException("The book doesn't exist.");
            }

            // Update the existing book's properties
            foreach (PropertyInfo property in typeof(Book).GetProperties().Where(p => p.Name != "BookId"))
            {
                var requestProperty = typeof(BookDto).GetProperty(property.Name);
                if (requestProperty != null)
                {
                    if (property.Name == "BookImage")
                    {
                        var imageFile = requestProperty.GetValue(request) as IFormFile;
                        if (imageFile != null)
                        {
                            byte[] imageData;
                            using (var memoryStream = new MemoryStream())
                            {
                                await imageFile.CopyToAsync(memoryStream);
                                imageData = memoryStream.ToArray();
                            }
                            property.SetValue(existing, imageData);
                        }
                    }
                    else
                    {
                        var propertyValue = requestProperty.GetValue(request);
                        // Ensure the value being set is of the correct type
                        if (propertyValue != null && property.PropertyType.IsAssignableFrom(propertyValue.GetType()))
                        {
                            property.SetValue(existing, propertyValue);
                        }
                    }
                }
            }

            await _libraryContext.SaveChangesAsync();
            log.Info("The details of the book are updated successfully in the DB");
            return request;
        }



        public async Task<Book> UpdateNoOfBook(string BookName,int NoOfBook)
        {
            var existing = _libraryContext.Books.FirstOrDefault(x => x.BookName == BookName);
            if (existing == null)
            {
                log.Debug("The book is not found from DB, so update failed");
                throw new IdNotFoundException("The book doesn't exist.");

            }
            
            existing.NoOfBook = NoOfBook;            
            _libraryContext.SaveChanges();
            log.Info("The details of the book is updated successfully to DB");
            return existing;
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
                CallStoredProcedureAsync("INFO", "The book is not found from DB, delete failed");
                throw new IdNotFoundException("The book doesn't exist.");
            }
            _libraryContext.Books.Remove(book);
            _libraryContext.SaveChanges();
            log.Info("The details of the book is deleted successfully from DB");
            CallStoredProcedureAsync("INFO", "The details of the book is deleted successfully from DB");
            return book;
        }
        public async Task<string> BulkUploadDynamic(IFormFile file)
        {

            if (file == null || file.Length == 0)
            {
                log.Debug("No file uploaded or file is empty.");
                throw new CSVException("File is empty.");
            }

            Type type = typeof(Book);
            PropertyInfo[] properties = type.GetProperties();

            var books = new List<Book>();
            string emptyCell = "";
            //using (var reader = new StreamReader(filePath))
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0; // Reset the stream position to the beginning
                using (var reader = new StreamReader(stream))
                using (var csv = new CsvReader(reader, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)))
                {
                    csv.Read();
                    csv.ReadHeader();
                    var headerRow = csv.HeaderRecord;
                    //Mapping the properties to header columns
                    var propertyMap = new Dictionary<string, PropertyInfo>();

                    foreach (var header in headerRow)
                    {
                        var property = properties.FirstOrDefault(p => p.Name.Equals(header, StringComparison.OrdinalIgnoreCase));
                        if (property != null)
                        {
                            propertyMap[header] = property;
                        }
                        else
                        {
                            log.Debug($"CSV header '{header}' does not match any property in the Item class.");
                            CallStoredProcedureAsync("DEBUG", $"CSV header '{header}' does not match any property in the Item class.");
                            throw new CSVException($"CSV header not match property in Book class.");
                        }
                    }

                    int i = 0;
                    bool flag = false;
                    //int headerLength = headerRow.Count();

                    // var allFields = new List<string>();
                    while (csv.Read())
                    {
                        i++;
                        var book = new Book();
                        foreach (var header in headerRow)
                        {
                            var value = csv.GetField(header);

                            if (string.IsNullOrEmpty(value))
                            {
                                log.Debug($"CSV contains empty or null fields at row {i}");
                                CallStoredProcedureAsync("DEBUG", $"CSV contains empty or null fields at row {i}");
                                emptyCell = ", but some cells are empty ";
                                flag = true;
                                break;
                            }
                            var property = propertyMap[header];
                            if (header == "BookImage")
                            {
                                var imagePath = csv.GetField(header);
                                if (File.Exists(imagePath))
                                {
                                    byte[] imageBytes = File.ReadAllBytes(imagePath);
                                    var prop = propertyMap["BookImage"];
                                    object convertedImage;

                                    prop.SetValue(book, imageBytes);
                                    continue;
                                }
                                else
                                {
                                    throw new ArgumentsException($"Image file not found: {imagePath}");
                                }
                            }
                            object convertedValue;
                            //Type conversions
                            if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                            {
                                var nullableConverter = new NullableConverter(property.PropertyType);
                                convertedValue = nullableConverter.ConvertFrom(value);

                            }
                            else
                            {
                                convertedValue = Convert.ChangeType(value, property.PropertyType);
                            }
                            property.SetValue(book, convertedValue);

                        }
                        if (flag)
                        {
                            flag = false;
                            continue;
                        }
                        var existingItem = _libraryContext.Books.FirstOrDefault(b => b.BookName == book.BookName && b.BookAuthor == book.BookAuthor);
                        if (existingItem != null)
                        {
                            // Update the existing book's NoOfBook
                            foreach (PropertyInfo property in existingItem.GetType().GetProperties()
                                                  .Where(p => p.Name != "BookName" && p.Name != "BookAuthor" && p.Name != "BookId"))
                            {
                                //var propertyValue = property.GetValue(book);
                                //property.SetValue(existingItem, propertyValue);
                                var propertyValue = property.GetValue(book);

                                if (property.Name == "NoOfBook")
                                {
                                    var existingQuantity = (int)property.GetValue(existingItem);
                                    var newQuantity = (int)propertyValue;
                                    property.SetValue(existingItem, existingQuantity + newQuantity);
                                }
                                if (property.Name == "Price")
                                {
                                    property.SetValue(existingItem, propertyValue);
                                }     

                            }
                        }
                        else
                        {

                            books.Add(book);
                        }
                    }
                }

            }
            foreach (var book in books)
            {
                _libraryContext.Books.Add(book);
            }
            //await _libraryContext.BulkInsertAsync(books);
            await _libraryContext.SaveChangesAsync();
            log.Debug("Items added from " + file + " and added to DB successfully" + emptyCell);
            CallStoredProcedureAsync("DEBUG", "Items added from " + file + " and added to DB successfully" + emptyCell);
            return "Items added successfully" + emptyCell;


        }


        public void CallStoredProcedureAsync(string level, string message)
        {
            try
            {
                //var userIdString = _httpContextAccessor.HttpContext.Session.GetString("UserId");
                //int.TryParse(userIdString, out int userId);
                //var user = _httpContextAccessor.HttpContext.Items["User"] as User;
                var userId = _httpContextAccessor.HttpContext.Items["UserId"];
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

        }

    }
}

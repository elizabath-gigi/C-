using Microsoft.EntityFrameworkCore;
using OrderManagement.Exceptions;
using OrderManagement.Interface;
using OrderManagement.Models;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using log4net;
using static System.Reflection.Metadata.BlobBuilder;
using System.Reflection;
using CsvHelper.TypeConversion;
using System.ComponentModel;
using NullableConverter = System.ComponentModel.NullableConverter;
using NetTopologySuite.Index.HPRtree;




namespace OrderManagement.Services
{
    public class DynamicLibraryServices : IDynamicLibraryServices
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly LibraryContext _libraryContext;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LibraryServices));



        public DynamicLibraryServices(LibraryContext libraryContext, IWebHostEnvironment webHostEnvironment)
        {
            _libraryContext = libraryContext;
            _webHostEnvironment = webHostEnvironment;
        }
        /// <summary>
        /// Add books from CSV file to the Database(Bulk Upload).
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns> Task<string> </returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="CSVException"></exception>
        public async Task<string> BulkUploadDynamic(string fileName)
        {
            //get the file path
            string currentDirectory = _webHostEnvironment.ContentRootPath;
            string filePath = Path.Combine(currentDirectory, fileName);
            //Check if file exists in the path
            if (!File.Exists(filePath))
            {
                log.Debug("File doesn't exist, Bulk Upload failed");
                throw new ArgumentsException("File does not exist at the specified path.");
            }

            Type type = typeof(Library);
            PropertyInfo[] properties = type.GetProperties();

            var books = new List<Library>();
            string emptyCell = "";
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)))
            {
                csv.Read();
                csv.ReadHeader();
                var headerRow = csv.HeaderRecord;
                // Dynamically check and map headers to properties
                // var properties = typeof(Item).GetProperties();
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
                        throw new CSVException($"CSV header '{header}' does not match any property in the Item class.");
                    }
                }

                int i = 0;
                bool flag = false;
                //int headerLength = headerRow.Count();

                // var allFields = new List<string>();
                while (csv.Read())
                {
                    i++;
                    var book = new Library();
                    foreach (var header in headerRow)
                    {
                        var value = csv.GetField(header);

                        if (string.IsNullOrEmpty(value))
                        {
                            log.Debug($"CSV contains empty or null fields at row {i}");
                            emptyCell = ", but some cells are empty or null";
                            flag = true;
                            break;
                        }
                        var property = propertyMap[header];
                        object convertedValue;

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
                    var existingItem = _libraryContext.Libraries.FirstOrDefault(b => b.BookName == book.BookName && b.BookAuthor == book.BookAuthor);
                    if (existingItem != null)
                    {
                        // Update the existing book's NoOfBook
                        foreach (PropertyInfo property in existingItem.GetType().GetProperties().Where(p => p.Name != "BookName" && p.Name != "BookAuthor" && p.Name != "BookId"))
                        {
                            var propertyValue = property.GetValue(book);
                            property.SetValue(existingItem, propertyValue);
                        }
                    }
                    else
                    {
                        books.Add(book);
                    }
                }
            }
            foreach(var book in books)
            {
                _libraryContext.Libraries.Add(book);
            }
            //await _libraryContext.BulkInsertAsync(books);
            await _libraryContext.SaveChangesAsync();
            log.Debug("Items added from " + fileName + " and added to DB successfully" + emptyCell);
            return "Items added successfully" + emptyCell;


        }
        
       
        /// <summary>
        /// Update the details of a book in DB.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Task<Library></returns>
        /// <exception cref="IdNotFoundException"></exception>
        public async Task<Library> UpdateBook(Library request)
        {
            var existing = _libraryContext.Libraries.FirstOrDefault(x => x.BookId == request.BookId);
            if (existing == null)
            {
                log.Debug("The book is not found from DB, so update failed");
                throw new IdNotFoundException("The book doesn't exist.");

            }
            // Update the existing book's NoOfBook
            foreach (PropertyInfo property in existing.GetType().GetProperties().Where(p => p.Name != "BookId"))
            {
                var propertyValue = property.GetValue(request);
                property.SetValue(existing, propertyValue);
            }
            _libraryContext.SaveChanges();
            log.Info("The details of the book is updated successfully to DB");
            return request;
        }

    }
}



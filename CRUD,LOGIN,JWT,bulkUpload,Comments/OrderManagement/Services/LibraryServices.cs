﻿using Microsoft.EntityFrameworkCore;
using OrderManagement.Exceptions;
using OrderManagement.Interface;
using OrderManagement.Models;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using log4net;
using static System.Reflection.Metadata.BlobBuilder;



namespace OrderManagement.Services
{
    public class LibraryServices : ILibraryServices
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly LibraryContext _libraryContext;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LibraryServices));



        public LibraryServices(LibraryContext libraryContext, IWebHostEnvironment webHostEnvironment)
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
        public async Task<string> addBooksFromFile(string fileName)
        {
            //get the file path
            string currentDirectory = _webHostEnvironment.ContentRootPath;
            string filePath = Path.Combine(currentDirectory, fileName);

            //Check if file path is empty
            if (string.IsNullOrEmpty(filePath))
            {
                log.Debug("File name is null or empty, Bulk Upload failed");
                throw new ArgumentException("File name cannot be null or empty.", fileName);
            }

            //Check if file exists in the path
            if (!File.Exists(filePath))
            {
                log.Debug("File doesn't exist, Bulk Upload failed");
                throw new ArgumentException("File does not exist at the specified path.", fileName);
            }

            var books = new List<Library>();

            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(System.Globalization.CultureInfo.InvariantCulture)))
            {
                csv.Read();
                csv.ReadHeader();
                var headerRow = csv.HeaderRecord;

                string[] expectedHeaders = { "BookName", "BookAuthor", "NoOfBook" };
                //Checks if Columns are match
                if (!headerRow.SequenceEqual(expectedHeaders, StringComparer.OrdinalIgnoreCase))
                {
                    log.Debug("CSV header does not match the expected format, Bulk Upload failed");
                    throw new CSVException("CSV header does not match the expected format.");
                }

                while (csv.Read())
                {
                    // Retrieve the value of each field
                    string bookName = csv.GetField<string>("BookName");
                    string bookAuthor = csv.GetField<string>("BookAuthor");
                    int? noOfBook = csv.GetField<int?>("NoOfBook");

                    // Check if any field is null or empty
                    if (string.IsNullOrEmpty(bookName) || string.IsNullOrEmpty(bookAuthor)|| !noOfBook.HasValue)
                    {
                        log.Debug("CSV contains empty or null fields, Bulk Upload failed");
                        throw new CSVException("CSV contains empty or null fields.");
                    }

                    // Check if the book already exists in the database
                    var existingBook = _libraryContext.Libraries.FirstOrDefault(b => b.BookName == bookName && b.BookAuthor == bookAuthor);

                    if (existingBook != null)
                    {
                        // Update the existing book's NoOfBook
                        existingBook.NoOfBook += noOfBook;
                    }
                    else
                    {
                        //if new book, add it
                        var book = new Library
                        {
                            BookName = bookName,
                            BookAuthor = bookAuthor,
                            NoOfBook = noOfBook
                        };
                        books.Add(book);
                    }
                }
            }

            await _libraryContext.BulkInsertAsync(books);
            _libraryContext.SaveChanges();

            log.Debug("Books added from " + fileName + " and added to DB successfully.");
            return "Added successfully";
        }
        /// <summary>
        /// Get the details about all the books from the DB.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Library>> GetBooks()
            {
                var book = await _libraryContext.Libraries.ToListAsync();
                if (book.Count == 0)
                {
                    log.Debug("The library DB is null");
                }
                log.Info("The contents of the Library DB is retrieved");
                return book;

            }
            public async Task<Library> GetBook(int id)
            {
                var book = _libraryContext.Libraries.FirstOrDefault(x => x.BookId == id);
                if (book == null)
                {
                    log.Debug("The book doesn't exist in the DB, so retrieval failed");
                    throw new IdNotFoundException("The book doesn't exist");
                }
                log.Info("The details of the book is retrieved");
                return book;

            }
        /// <summary>
        /// Add the details of a book to the DB.
        /// </summary>
        /// <param name="request"></param>
        /// <returns> Task<Library> </returns>
            public async Task<Library> AddBook(Library request)
            {
                _libraryContext.Add(request);
                _libraryContext.SaveChanges();
                log.Info("The details of the book is added successfully to DB");

                return request;
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
                existing.NoOfBook = request.NoOfBook;
                existing.BookName = request.BookName;
                existing.BookAuthor = request.BookAuthor;
                _libraryContext.SaveChanges();
                log.Info("The details of the book is updated successfully to DB");
                return request;
            }
        /// <summary>
        /// Delete the details of the book from the DB.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Task<Library></returns>
        /// <exception cref="IdNotFoundException"></exception>
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



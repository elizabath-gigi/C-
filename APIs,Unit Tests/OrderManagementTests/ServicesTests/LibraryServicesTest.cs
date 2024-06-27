using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OrderManagement.Controllers;
using OrderManagement.Exceptions;
using OrderManagement.Interface;
using OrderManagement.Models;
using OrderManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementTests.ServicesTests
{
    public class LibraryServicesTest
    {
      
        private  LibraryServices _libraryService;
        private  Mock<IWebHostEnvironment> _mockWebHostEnvironment;
        private List<Library> sampleBooks = new List<Library>
        {
             new Library { BookId = 1, BookName = "Book 1", BookAuthor = "Author 1", NoOfBook = 5, Price = 100 },
             new Library { BookId = 2, BookName = "Book 2", BookAuthor = "Author 2", NoOfBook = 15, Price = 150 },
             new Library { BookId = 3, BookName = "Book 3", BookAuthor = "Author 3", NoOfBook = 20, Price = 200 }
        };
        public LibraryServicesTest()
        {
            
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
        }
        private async Task<LibraryContext> GetMemoryDatabaseContext(int count)
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var optionsBuilder = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(databaseName: "Library")
                .UseInternalServiceProvider(serviceProvider);
            var _context = new LibraryContext(optionsBuilder.Options);
            // context.Database.EnsureCreated();
            if (await _context.Libraries.CountAsync() <= 0)
            {
                for (int i = 0; i < count; i++)
                {
                    _context.Libraries.Add(sampleBooks[i]);
                    await _context.SaveChangesAsync();
                }
            }

            return _context;
        }
      
        [Fact]
        public async void GetBooksInPages_ShouldReturnListOfBooksInThatPage()
        {
            var context = await GetMemoryDatabaseContext(3);
            _libraryService = new LibraryServices(context, _mockWebHostEnvironment.Object);

            var result = await _libraryService.GetBooksInPages(2,2);

            Assert.NotNull(result);
            Assert.Equal(1, result.Count);
            Assert.IsType<List<Library>>(result);
            Assert.Equal("Book 3", result[0].BookName);
            Assert.Equal("Author 3", result[0].BookAuthor);
            Assert.Equal(20, result[0].NoOfBook);
            Assert.Equal(200, result[0].Price);


        }
        [Fact]
        public async void GetBooksInPages_ShouldReturnException_PageNumberExceedsLimit()
        {
            var context = await GetMemoryDatabaseContext(2);
            _libraryService = new LibraryServices(context, _mockWebHostEnvironment.Object);

            await Assert.ThrowsAsync<ArgumentsException>(() => _libraryService.GetBooksInPages(2,2));


        }

        [Fact]
        public async void GetBooks_ShouldReturnListOfBooks()
        {
            var context= await GetMemoryDatabaseContext(2);
            _libraryService = new LibraryServices(context, _mockWebHostEnvironment.Object);

            var result = await _libraryService.GetBooks();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.IsType<List<Library>>(result);

        }
        [Fact]
        public async void GetBooks_ShouldReturnEmptyList()
        {
            var context = await GetMemoryDatabaseContext(0);
            _libraryService = new LibraryServices(context, _mockWebHostEnvironment.Object);

            var result = await _libraryService.GetBooks();

            Assert.Empty(result);
            Assert.IsType<List<Library>>(result);

        }
        [Fact]
        public async void GetBook_ShouldReturnBook()
        {
            var context = await GetMemoryDatabaseContext(3);
            _libraryService = new LibraryServices(context, _mockWebHostEnvironment.Object);

            var result = await _libraryService.GetBook(1);

            Assert.IsType<Library>(result);
            Assert.Equal("Book 1", result.BookName);
            Assert.Equal("Author 1", result.BookAuthor);
            Assert.Equal(5, result.NoOfBook);
            Assert.Equal(100, result.Price);
        }
        [Fact]
        public async void GetBook_ShouldReturnException()
        {
            var context = await GetMemoryDatabaseContext(3);
            _libraryService = new LibraryServices(context, _mockWebHostEnvironment.Object);
            
           await  Assert.ThrowsAsync<IdNotFoundException>(() => _libraryService.GetBook(9));
        }
       [Fact]
        public async void AddBook_ShouldReturnBook()
        {
            var context = await GetMemoryDatabaseContext(2);
            _libraryService = new LibraryServices(context, _mockWebHostEnvironment.Object);
            var expectedBook = new Library { BookId = 4, BookName = "Book 4", BookAuthor = "Author 4", NoOfBook = 4, Price = 150 };

            var result = await _libraryService.AddBook(expectedBook);

            Assert.IsType<Library>(result);
            Assert.Equal(expectedBook.BookId, result.BookId);
            Assert.Equal(expectedBook.BookName, result.BookName);
            Assert.Equal(expectedBook.BookAuthor, result.BookAuthor);
            Assert.Equal(expectedBook.NoOfBook, result.NoOfBook);
            Assert.Equal(expectedBook.Price, result.Price);
        }
        [Fact]
        public async void UpdateBook_ShouldReturnBook()
        {
            var context = await GetMemoryDatabaseContext(2);
            _libraryService = new LibraryServices(context, _mockWebHostEnvironment.Object);
            var expectedBook = new Library { BookId = 2, BookName = "Book 4", BookAuthor = "Author 4", NoOfBook = 4, Price = 150 };

            var result = await _libraryService.UpdateBook(expectedBook);

            Assert.IsType<Library>(result);
            Assert.Equal(expectedBook.BookId, result.BookId);
            Assert.Equal(expectedBook.BookName, result.BookName);
            Assert.Equal(expectedBook.BookAuthor, result.BookAuthor);
            Assert.Equal(expectedBook.NoOfBook, result.NoOfBook);
            Assert.Equal(expectedBook.Price, result.Price);
        }

        [Fact]
        public async void UpdateBook_ShouldReturException()
        {
            var context = await GetMemoryDatabaseContext(3);
            _libraryService = new LibraryServices(context, _mockWebHostEnvironment.Object);
            var expectedBook = new Library { BookId = 4, BookName = "Book 4", BookAuthor = "Author 4", NoOfBook = 4, Price = 150 };


            await Assert.ThrowsAsync<IdNotFoundException>(() => _libraryService.UpdateBook(expectedBook));
        }
        [Fact]
        public async void DeleteBook_ShouldReturnBook()
        {
            var context = await GetMemoryDatabaseContext(3);
            _libraryService = new LibraryServices(context, _mockWebHostEnvironment.Object);
          

            var result = await _libraryService.DeleteBook(1);

            Assert.IsType<Library>(result);
            Assert.Equal("Book 1", result.BookName);
            Assert.Equal("Author 1", result.BookAuthor);
            Assert.Equal(5, result.NoOfBook);
            Assert.Equal(100, result.Price);
        }

        [Fact]
        public async void DeleteBook_ShouldReturException()
        {
            var context = await GetMemoryDatabaseContext(3);
            _libraryService = new LibraryServices(context, _mockWebHostEnvironment.Object);
            
            await Assert.ThrowsAsync<IdNotFoundException>(() => _libraryService.DeleteBook(8));
        }
    }

}

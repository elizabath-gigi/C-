using Azure;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderManagement.Controllers;
using OrderManagement.Interface;
using OrderManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementTests.ControllerTests
{
    public class LibraryControllerTests
    {
        private readonly Mock<ILibraryServices> _mockILibraryService;
        private readonly LibraryController _libraryController;

        public LibraryControllerTests()
        {
            _mockILibraryService = new Mock<ILibraryServices>();
            _libraryController = new LibraryController(_mockILibraryService.Object);
        }
        [Fact]
        public async Task AddBooksFromFile_ShouldReturnOkWithMessage()
        {
            var fileName = "file.csv";
            var response = "Added Successfully";

            _mockILibraryService.Setup(service => service.AddBooksFromFile(fileName)).ReturnsAsync(response);

            var result = await _libraryController.AddBooksFromFile(fileName);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<string>(okResult.Value);
            Assert.Equal(response, returnValue);
        }
        [Fact]
        public async Task GetBooksInPages_ShouldReturnOkWithBooks()
        {
            var page = 1;
            var pageSize = 2;
            var books = new List<Library>
            {
                new Library { BookId = 1, BookName = "Book 1", BookAuthor = "Author1", NoOfBook = 1, Price = 100},
                new Library { BookId = 2, BookName = "Book 2", BookAuthor = "Author2", NoOfBook = 2, Price = 200}
            };

            _mockILibraryService.Setup(service=>service.GetBooksInPages(page,pageSize)).ReturnsAsync(books);

            var result = await _libraryController.GetBooksInPages(1,2);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<Library>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);

        }

        [Fact]
        public async Task GetBooksInPages_ShouldReturnNotFoundWithMessage()
        {
            var page = 1;
            var pageSize = 2;
            var books = new List<Library>();
            
            _mockILibraryService.Setup(service => service.GetBooksInPages(page, pageSize)).ReturnsAsync(books);

            var result = await _libraryController.GetBooksInPages(1, 2);

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("No books found.", notFoundResult.Value);

        }
        [Fact]
        public async Task GetBooks_ShouldReturnOkWithBooks()
        {
            
            var books = new List<Library>
            {
                new Library { BookId = 1, BookName = "Book 1", BookAuthor = "Author1", NoOfBook = 1, Price = 100},
                new Library { BookId = 2, BookName = "Book 2", BookAuthor = "Author2", NoOfBook = 2, Price = 200}
            };

            _mockILibraryService.Setup(service => service.GetBooks()).ReturnsAsync(books);

            
            var result = await _libraryController.GetBooks();

            
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<Library>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetBooks_ShouldReturnNotFoundWithMessage()
        {
            var books = new List<Library>();
            _mockILibraryService.Setup(service => service.GetBooks()).ReturnsAsync(books);

            var result = await _libraryController.GetBooks();

            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("No books found.", notFoundResult.Value);
        }
        [Fact]
        public async Task GetBook_ShouldReturnOkWithBook()
        {
           
            var expectedBook = new Library { BookId = 1, BookName = "Book 1", BookAuthor = "Author1", NoOfBook = 1, Price = 100 };
            _mockILibraryService.Setup(service=>service.GetBook(expectedBook.BookId)).ReturnsAsync(expectedBook);

            var result = await _libraryController.GetBook(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<Library>(okResult.Value);

            Assert.Equal(expectedBook.BookId, returnValue.BookId);
            Assert.Equal(expectedBook.BookName, returnValue.BookName); 
            Assert.Equal(expectedBook.BookAuthor, returnValue.BookAuthor);
            Assert.Equal(expectedBook.NoOfBook, returnValue.NoOfBook);
            Assert.Equal(expectedBook.Price, returnValue.Price);
        }

        [Fact]
        public async Task AddBook_ShouldReturnOkWithBook()
        {
            var expectedBook = new Library { BookId = 1, BookName = "Book 1", BookAuthor = "Author1", NoOfBook = 1, Price = 100 };
            _mockILibraryService.Setup(service=>service.AddBook(expectedBook)).ReturnsAsync(expectedBook);

            var result = await _libraryController.AddBook(expectedBook);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<Library>(okResult.Value);

            Assert.Equal(expectedBook.BookId, returnValue.BookId);
            Assert.Equal(expectedBook.BookName, returnValue.BookName);
            Assert.Equal(expectedBook.BookAuthor, returnValue.BookAuthor);
            Assert.Equal(expectedBook.NoOfBook, returnValue.NoOfBook);
            Assert.Equal(expectedBook.Price, returnValue.Price);
        }
        [Fact]       
        public async Task UpdateBook_ShouldReturnOkWithBook()
        {
            var expectedBook = new Library { BookId = 1, BookName = "Book 1", BookAuthor = "Author1", NoOfBook = 1, Price = 100 };
            _mockILibraryService.Setup(service => service.UpdateBook(expectedBook)).ReturnsAsync(expectedBook);

            var result = await _libraryController.UpdateBook(expectedBook);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<Library>(okResult.Value);

            Assert.Equal(expectedBook.BookId, returnValue.BookId);
            Assert.Equal(expectedBook.BookName, returnValue.BookName);
            Assert.Equal(expectedBook.BookAuthor, returnValue.BookAuthor);
            Assert.Equal(expectedBook.NoOfBook, returnValue.NoOfBook);
            Assert.Equal(expectedBook.Price, returnValue.Price);
        }
        [Fact]
        public async Task DeleteBook_ShouldReturnOkWithBook()
        {
            var expectedBook = new Library { BookId = 1, BookName = "Book 1", BookAuthor = "Author1", NoOfBook = 1, Price = 100 };
            _mockILibraryService.Setup(service=>service.DeleteBook(expectedBook.BookId)).ReturnsAsync(expectedBook);

            var result = await _libraryController.DeleteBook(1);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<Library>(okResult.Value);

            Assert.Equal(expectedBook.BookId, returnValue.BookId);
            Assert.Equal(expectedBook.BookName, returnValue.BookName);
            Assert.Equal(expectedBook.BookAuthor, returnValue.BookAuthor);
            Assert.Equal(expectedBook.NoOfBook, returnValue.NoOfBook);
            Assert.Equal(expectedBook.Price, returnValue.Price);


        }
    }
}

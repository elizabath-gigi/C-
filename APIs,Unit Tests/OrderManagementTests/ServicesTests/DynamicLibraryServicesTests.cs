using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
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
    public class DynamicLibraryServicesTests
    {
        private DynamicLibraryServices _dynamicLibraryService;
        private Mock<IWebHostEnvironment> _mockWebHostEnvironment;
        private List<Library> sampleBooks = new List<Library>
        {
             new Library { BookId = 1, BookName = "Book 1", BookAuthor = "Author 1", NoOfBook = 5, Price = 100 },
             new Library { BookId = 2, BookName = "Book 2", BookAuthor = "Author 2", NoOfBook = 15, Price = 150 },
             new Library { BookId = 3, BookName = "Book 3", BookAuthor = "Author 3", NoOfBook = 20, Price = 200 }
        };
        public DynamicLibraryServicesTests()
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
        public async void BulkUploadDynamic_ShouldReturnArgumentException_FileDoesNotExist()
        {
            
            string fileName = "invalidfile.csv";
            _mockWebHostEnvironment.Setup(env => env.ContentRootPath).Returns(Directory.GetCurrentDirectory());
            var context = await GetMemoryDatabaseContext(0);
            _dynamicLibraryService = new DynamicLibraryServices(context, _mockWebHostEnvironment.Object);

            
            await Assert.ThrowsAsync<ArgumentsException>(() => _dynamicLibraryService.BulkUploadDynamic(fileName));
        }
        [Fact]
        public async Task AddBooksFromFile_ShouldReturnCSVException_HeaderDoesNotMatch()
        {
            
            string fileName = "invalidHeader.csv";
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            _mockWebHostEnvironment.Setup(env => env.ContentRootPath).Returns(Directory.GetCurrentDirectory());
            await File.WriteAllTextAsync(filePath, "BookName,Book,NoOfBook,Price\n");

            var context = await GetMemoryDatabaseContext(0);
            _dynamicLibraryService = new DynamicLibraryServices(context, _mockWebHostEnvironment.Object);

            
             await Assert.ThrowsAsync<CSVException>(() => _dynamicLibraryService.BulkUploadDynamic(fileName));
            
        }
        [Fact]
        public async Task AddBooksFromFile_ShouldReturnCSVException_FileContainsEmptyOrNullFields()
        {
            string expectedSubstring = ", but some cells are empty or null";
            string fileName = "nullFields.csv";
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            _mockWebHostEnvironment.Setup(env => env.ContentRootPath).Returns(Directory.GetCurrentDirectory());
            await File.WriteAllTextAsync(filePath, "BookName,BookAuthor,NoOfBook,Price\nBook1,Author1,,100\nBook2,Author2,10,100\n");

            var context = await GetMemoryDatabaseContext(0);
            _dynamicLibraryService = new DynamicLibraryServices(context, _mockWebHostEnvironment.Object);

           
            var result = await _dynamicLibraryService.BulkUploadDynamic(fileName);

            
            Assert.Contains(expectedSubstring,result);

           
        }

        [Fact]
        public async Task AddBooksFromFile_ShouldAddBooksSuccessfully()
        {
            
            string fileName = "validBooks.csv";
            string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
            _mockWebHostEnvironment.Setup(env => env.ContentRootPath).Returns(Directory.GetCurrentDirectory());
            await File.WriteAllTextAsync(filePath, "BookName,BookAuthor,NoOfBook\nBook4,Author4,5,100\nBook5,Author5,10,100\n");

            var context = await GetMemoryDatabaseContext(3);
            _dynamicLibraryService = new DynamicLibraryServices(context, _mockWebHostEnvironment.Object);

           
            var result = await _dynamicLibraryService.BulkUploadDynamic(fileName);

            
            Assert.Equal("Items added successfully", result);
            Assert.Equal(5, await context.Libraries.CountAsync());

            
        }
        [Fact]
        public async void UpdateBook_ShouldReturnBook()
        {
            var context = await GetMemoryDatabaseContext(2);
            _dynamicLibraryService = new DynamicLibraryServices(context, _mockWebHostEnvironment.Object);
            var expectedBook = new Library { BookId = 2, BookName = "Book 4", BookAuthor = "Author 4", NoOfBook = 4, Price = 150 };

            var result = await _dynamicLibraryService.UpdateBook(expectedBook);

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
            _dynamicLibraryService = new DynamicLibraryServices(context, _mockWebHostEnvironment.Object);
            var expectedBook = new Library { BookId = 4, BookName = "Book 4", BookAuthor = "Author 4", NoOfBook = 4, Price = 150 };


            await Assert.ThrowsAsync<IdNotFoundException>(() => _dynamicLibraryService.UpdateBook(expectedBook));
        }

    }
}

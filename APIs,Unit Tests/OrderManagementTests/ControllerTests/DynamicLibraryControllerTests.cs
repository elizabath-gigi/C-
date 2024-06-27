using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderManagement.Controllers;
using OrderManagement.Interface;
using OrderManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementTests.ControllerTests
{
    public  class DynamicLibraryControllerTests
    {
        private readonly Mock<IDynamicLibraryServices> _mockIDynamicLibraryServices;
        private readonly DynamicLibraryController _dynamicLibraryController;
        public DynamicLibraryControllerTests()
        {
            _mockIDynamicLibraryServices = new Mock<IDynamicLibraryServices>();
            _dynamicLibraryController = new DynamicLibraryController(_mockIDynamicLibraryServices.Object);
        }
        [Fact]
        public async Task BulkUploadDynamic_ShouldReturnOkWithMessage()
        {
            var fileName = "file.csv";
            var response = "Added Successfully";

            _mockIDynamicLibraryServices.Setup(service => service.BulkUploadDynamic(fileName)).ReturnsAsync(response);

            var result = await _dynamicLibraryController.BulkUploadDynamic(fileName);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<string>(okResult.Value);
            Assert.Equal(response, returnValue);
        }
        [Fact]
        public async Task UpdateBook_ShouldReturnOkWithBook()
        {
            var expectedBook = new Library { BookId = 1, BookName = "Book 1", BookAuthor = "Author1", NoOfBook = 1, Price = 100 };
            _mockIDynamicLibraryServices.Setup(service => service.UpdateBook(expectedBook)).ReturnsAsync(expectedBook);

            var result = await _dynamicLibraryController.UpdateBook(expectedBook);

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

using LibraryManagement.Exceptions;
using LibraryManagement.Interfaces;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagement.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class LibraryController : ControllerBase
    {
        private readonly ILibraryServices iservice;


        public LibraryController(ILibraryServices iservice)
        {
            this.iservice = iservice;
        }





        [Authorize(Roles = "Admin,User")]
        [HttpGet]// ASP.NET Core Web API to specify that a particular action method should respond to HTTP GET requests. 
        [Route("getBooksInPages")]
        public async Task<ActionResult<Book>> GetBooksInPages(int page, int pageSize)
        {
            try
            {
                var books = await iservice.GetBooksInPages(page, pageSize);
                if (books.Count == 0)
                {
                    return NotFound("No books found.");
                }
                return Ok(books);
            }
            catch (ArgumentsException ex)
            {
                return BadRequest(ex.Message);
            }

        }



        [Authorize(Roles = "Admin,User")]
        [HttpGet]// ASP.NET Core Web API to specify that a particular action method should respond to HTTP GET requests. 
        [Route("getBooks")]
        public async Task<ActionResult<Book>> GetBooks()
        {
            var books = await iservice.GetBooks();
            if (books.Count == 0)
            {
                return NotFound("No books found.");
            }

            return Ok(books);

        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        [Route("getBook")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            try
            {
                var book = await iservice.GetBook(id);
                return Ok(book);
            }
            catch (IdNotFoundException ex)
            {
                return NotFound(ex.Message);


            }
        }
        [Authorize(Roles = "Admin,User")]
        [HttpGet]
        [Route("getByBookName")]
        public async Task<ActionResult<Book>> GetByBookName(string BookName)
        {
            try
            {
                var book = await iservice.GetByBookName(BookName);
                return Ok(book);
            }
            catch (IdNotFoundException ex)
            {
                return NotFound(ex.Message);


            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("addBook")]
        public async Task<ActionResult<Book>> AddBook(Book request)
        {
            var books = await iservice.AddBook(request);
            return Ok(books);


        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("updateBook")]
        public async Task<ActionResult<Book>> UpdateBook(Book request)
        {
            try
            {
                var books = await iservice.UpdateBook(request);
                return Ok(books);
            }
            catch (IdNotFoundException ex)
            {
                return NotFound(ex.Message);
            }


        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        [Route("updateNoOfBooks")]
        public async Task<ActionResult<Book>> UpdateNoOfBook(string BookName,int NoOfBook)
        {
            try
            {
                var books = await iservice.UpdateNoOfBook(BookName,NoOfBook);
                return Ok(books);
            }
            catch (IdNotFoundException ex)
            {
                return NotFound(ex.Message);
            }


        }
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [Route("deleteBook")]
        public async Task<ActionResult<Book>> DeleteBook(int id)
        {
            try
            {
                var books = await iservice.DeleteBook(id);
                return Ok(books);
            }
            catch (IdNotFoundException ex)
            {
                return NotFound(ex.Message);
            }

        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("bulkUploadDynamic")]
        public async Task<ActionResult<string>> BulkUploadDynamic(FormFile fileName)
        {
            try
            {
                var response = await iservice.BulkUploadDynamic(fileName);
                return Ok(response);
            }
            catch (CSVException ex)
            {
                return BadRequest(ex.Message);
            }

        }

    }
}

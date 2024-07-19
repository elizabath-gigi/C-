using LibraryManagement.Interfaces;
using LibraryManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LibraryManagement.Controllers
{
    public class LibraryController:ControllerBase
    {
        private readonly ILibraryServices iservice;


        public LibraryController(ILibraryServices iservice)
        {
            this.iservice = iservice;
        }






        [HttpGet]// ASP.NET Core Web API to specify that a particular action method should respond to HTTP GET requests. 
        [Route("getBooksInPages")]
        public async Task<ActionResult<Book>> GetBooksInPages(int page, int pageSize)
        {
            var books = await iservice.GetBooksInPages(page, pageSize);
            if (books.Count == 0)
            {
                return NotFound("No books found.");
            }
            return Ok(books);
        }



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

        [HttpGet]
        [Route("getBook")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await iservice.GetBook(id);
            return Ok(book);


        }
        [HttpPost]
        [Route("addBook")]
        public async Task<ActionResult<Book>> AddBook(Book request)
        {
            var books = await iservice.AddBook(request);
            return Ok(books);


        }
        [HttpPut]
        [Route("updateBook")]
        public async Task<ActionResult<Book>> UpdateBook(Book request)
        {
            var books = await iservice.UpdateBook(request);
            return Ok(books);


        }
        [HttpDelete]
        [Route("deleteBook")]
        public async Task<ActionResult<Book>> DeleteBook(int id)
        {
            var books = await iservice.DeleteBook(id);
            return Ok(books);


        }
    }
}

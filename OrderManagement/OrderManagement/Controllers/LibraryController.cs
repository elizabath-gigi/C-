//using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Model;

using OrderManagement.Services;

//using static System.Reflection.Metadata.BlobBuilder;

namespace OrderManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibraryController : ControllerBase
    {
        //private readonly ServiceClass serviceClass;
        private readonly IserviceClass iservice;
        public LibraryController(IserviceClass iservice) 
        {
            this.iservice=iservice;
        }
    
        [HttpGet]
        [Route("getBooks")]
        public async Task<ActionResult<Books>> GetBooks()
        {
            var books = await iservice.GetBooks();
            return Ok(books);

        }

        [HttpGet]
        [Route("getBook")]
        public async Task<ActionResult<Books>> GetBook(int id)
        {
            var book = await iservice.GetBook(id);

            if (book == null)
            {
                return BadRequest("No records found");
            }

            return Ok(book);


        }
        [HttpPost]
        [Route("addBook")]
        public async Task<ActionResult<Books>> AddBook(Books request)
        {
            var books = await iservice.AddBook(request);
            return Ok(books);


        }
        [HttpPut]
        [Route("updateBook")]
        public async Task<ActionResult<Books>> UpdateBook(Books request)
        {
            var books = await iservice.UpdateBook(request);

            if (books == null)
            {
                return BadRequest("No records found");
            }

            return Ok(books);


        }
        [HttpDelete]
        [Route("deleteBook")]
        public async Task<ActionResult<Books>> DeleteBook(int id)
        {
            var books = await iservice.DeleteBook(id);

            if (books == null)
            {
                return BadRequest("No records found");
            }

            return Ok(books);


        }
    }
}

//using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderManagement.Interface;
using OrderManagement.Models;
using OrderManagement.Services;
using OrderManagement.Exceptions;
using Microsoft.AspNetCore.Authorization;

//using static System.Reflection.Metadata.BlobBuilder;


namespace OrderManagement.Controllers
{
    [Route("api/[controller]")]//This attribute specifies the base URI for all HTTP requests handled by this controller
    [ApiController]//indicates that the controller is an API controller.
    [Authorize]
    public class LibraryController : ControllerBase//This signifies that the LibraryController class inherits from ControllerBase
    {
        //private readonly ServiceClass serviceClass;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LibraryController));
        private readonly ILibraryServices iservice;
        public LibraryController(ILibraryServices iservice) 
        {
            this.iservice=iservice;
        }
    
        [HttpGet]// ASP.NET Core Web API to specify that a particular action method should respond to HTTP GET requests. 
        [Route("getBooks")]
        public async Task<ActionResult<Library>> GetBooks()
        {
            var books = await iservice.GetBooks();
            log.Info("The contents of the Library DB is retrieved");

            return Ok(books);

        }

        [HttpGet]
        [Route("getBook")]
        public async Task<ActionResult<Library>> GetBook(int id)
        {
            var book = await iservice.GetBook(id);

            if (book == null)
            {
                log.Debug("The book doesn't exist in the DB, so retrieval failed");
                throw new IdNotFoundException("The book doesn't exist in the DB, so retrieval failed");
                //return BadRequest("No records found");//methods from controllerBase
            }
            log.Info("The contents of the book is retrieved");
            return Ok(book);


        }
        [HttpPost]
        [Route("addBook")]
        public async Task<ActionResult<Library>> AddBook(Library request)
        {
          
            var books = await iservice.AddBook(request);
            /*if (books == null)
            {
                log.Debug("The book already exists in the DB, so add failed");
                throw new IdNotFoundException("The book with the given id is not found from DB");
                //return BadRequest("Item Exists");
            }*/
            log.Info("The details of the book is added successfully to DB");
            return Ok(books);


        }
        [HttpPut]
        [Route("updateBook")]
        public async Task<ActionResult<Library>> UpdateBook(Library request)
        {
            var books = await iservice.UpdateBook(request);

            if (books == null)
            {
                log.Debug("The book is not found from DB, so update failed");
                throw new IdNotFoundException("The book with the given id is not found from DB");
                //return BadRequest("No records found");
            }
            log.Info("The details of the book is updated successfully to DB");
            return Ok(books);


        }
        [HttpDelete]
        [Route("deleteBook")]
        public async Task<ActionResult<Library>> DeleteBook(int id)
        {
            var books = await iservice.DeleteBook(id);

            if (books == null)
            {
                log.Debug("The book is not found from DB, delete failed");
                throw new IdNotFoundException("The book with the given id is not found from DB");
                //return BadRequest("No records found");
            }
            log.Info("The details of the book is deleted successfully from DB");
            return Ok(books);


        }
    }
}

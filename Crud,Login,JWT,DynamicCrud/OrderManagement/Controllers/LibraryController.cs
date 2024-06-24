﻿using Microsoft.AspNetCore.Mvc;
using OrderManagement.Interface;
using OrderManagement.Models;
using OrderManagement.Services;
using OrderManagement.Exceptions;
using Microsoft.AspNetCore.Authorization;
using CsvHelper;




namespace OrderManagement.Controllers
{
    [Route("api/[controller]")]//This attribute specifies the base URI for all HTTP requests handled by this controller
    [ApiController]//indicates that the controller is an API controller.
    //[Authorize]
    public class LibraryController : ControllerBase//This signifies that the LibraryController class inherits from ControllerBase
    {
        
        private readonly ILibraryServices iservice;
        

        public LibraryController(ILibraryServices iservice) 
        {
            this.iservice=iservice;
        }

        [HttpPost]
        [Route("bulkUploadDynamic")]
        public async Task<ActionResult<string>> bulkUploadDynamic(string fileName)
        {
            var response = await iservice.bulkUploadDynamic(fileName);
            return Ok(response);

        }



        [HttpPost]
        [Route("addBooksFromFile")]
        public async Task<ActionResult<string>> addBooksFromFile(string fileName)
        {
            var response = await iservice.addBooksFromFile(fileName);
            return Ok(response);

        }
        [HttpGet]// ASP.NET Core Web API to specify that a particular action method should respond to HTTP GET requests. 
        [Route("getBooksInPages")]
        public async Task<ActionResult<Library>> GetBooksInPages(int page = 1, int pageSize = 10)
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
        public async Task<ActionResult<Library>> GetBooks()
        {
            var books = await iservice.GetBooks();
            if(books.Count== 0)
            {
                return NotFound("No books found.");
            }
           
          

            return Ok(books);

        }

        [HttpGet]
        [Route("getBook")]
        public async Task<ActionResult<Library>> GetBook(int id)
        {
            var book = await iservice.GetBook(id);
            return Ok(book);


        }
        [HttpPost]
        [Route("addBook")]
        public async Task<ActionResult<Library>> AddBook(Library request)
        {

            var books = await iservice.AddBook(request);
            return Ok(books);


        }
        [HttpPut]
        [Route("updateBook")]
        public async Task<ActionResult<Library>> UpdateBook(Library request)
        {
            var books = await iservice.UpdateBook(request);
            return Ok(books);


        }
        [HttpDelete]
        [Route("deleteBook")]
        public async Task<ActionResult<Library>> DeleteBook(int id)
        {
            var books = await iservice.DeleteBook(id);
            return Ok(books);


        }
    }
}

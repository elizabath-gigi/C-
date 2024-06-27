using Microsoft.AspNetCore.Mvc;
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
    public class DynamicLibraryController : ControllerBase//This signifies that the LibraryController class inherits from ControllerBase
    {

        private readonly IDynamicLibraryServices iservice;


        public DynamicLibraryController(IDynamicLibraryServices iservice)
        {
            this.iservice = iservice;
        }

        [HttpPost]
        [Route("bulkUploadDynamic")]
        public async Task<ActionResult<string>> BulkUploadDynamic(string fileName)
        {
            var response = await iservice.BulkUploadDynamic(fileName);
            return Ok(response);

        }
       
        
        [HttpPut]
        [Route("updateBook")]
        public async Task<ActionResult<Library>> UpdateBook(Library request)
        {
            var books = await iservice.UpdateBook(request);
            return Ok(books);


        }
       
    }
}

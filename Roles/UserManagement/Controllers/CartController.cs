using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Exceptions;
namespace UserManagement.Controllers
{
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly Interfaces.ICartServices _cartServices;
        public CartController(Interfaces.ICartServices cartServices)
        {
            _cartServices = cartServices;
        }
        [HttpGet("ViewCartItems")]
        public async Task<IActionResult> ViewCartItems()
        {

            var response = await _cartServices.ViewCartItems();
            if (response.Count == 0)
            {
                return NotFound("No books found in the cart");
            }

            return Ok(response);
        }
        [HttpPost("AddCartItem")]
        public async Task<ActionResult> AddCartItem(string BookName)
        {
            try
            {
                var books = await _cartServices.AddCartItem(BookName);
                return Ok(books);
            }
            catch (ArgumentsException ex) 
            {
                return BadRequest(ex.Message);
            }
            catch(IdNotFoundException ex) 
            { 
                return BadRequest(ex.Message);
            }   
        }
        [HttpDelete("RemoveCartItem")]
        public async Task<ActionResult> RemoveCartItem(string BookName)
        {
            try
            {
                var books = await _cartServices.RemoveCartItem(BookName);
                return Ok(books);
            }
            catch(ArgumentsException ex) 
            { 
                return BadRequest(ex.Message);
            }
        }
        [HttpDelete("BorrowCartItem")]
        public async Task<ActionResult> BorrowCartItem()
        {
            try
            {
                var books = await _cartServices.BorrowCartItem();
                return Ok(books);
            }
            catch (ArgumentsException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("getAllBorrows")]
        public async Task<IActionResult> GetAllBorrows()
        {

            try
            {
                var books = await _cartServices.GetAllBorrows();
                return Ok(books);
            }
            catch (ArgumentsException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("getUserBorrows")]
        public async Task<IActionResult> GetUserBorrows()
        {
            try
            {
                var books = await _cartServices.GetUserBorrows();
                return Ok(books);
            }
            catch (ArgumentsException ex)
            {
                return BadRequest(ex.Message);
            }

        }
    }
}

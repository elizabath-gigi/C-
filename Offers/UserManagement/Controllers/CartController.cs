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
        public async Task<ActionResult> ViewCartItems()
        {

            var response = await _cartServices.ViewCartItems();
            if (response.Count == 0)
            {
                return NotFound("No books found in the cart");
            }

            return Ok(response);
        }
        [HttpGet("GetCartValue")]
        public async Task<ActionResult> GetCartValue()
        {
            try
            {
                var cartValue = await _cartServices.GetCartValue();
                if (cartValue == 0)
                {
                    return NotFound("CartValue is 0 as cart is empty");
                }
                return Ok(cartValue);

            }
            catch(ArgumentsException ex) 
            {
                return BadRequest(ex.Message);
            }
            
        }
        [HttpPost("AddCartItem")]
        public async Task<ActionResult> AddCartItem(string BookName,decimal Price)
        {
            try
            {
                var books = await _cartServices.AddCartItem(BookName,Price);
                return Ok(new {books }); 
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
        public async Task<ActionResult> RemoveCartItem(string BookName, decimal Price)
        {
            try
            {
                var books = await _cartServices.RemoveCartItem(BookName, Price);
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
        [HttpDelete("ReturnBook")]
        public async Task<ActionResult> ReturnBook(string BookName)
        {
            try
            {
                var books = await _cartServices.ReturnBook(BookName);
                return Ok(books);
            }
            catch (ArgumentsException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("getAllBorrows")]
        public async Task<ActionResult> GetAllBorrows()
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
        public async Task<ActionResult> GetUserBorrows()
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

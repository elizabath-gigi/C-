using LibraryManagement.Interfaces;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Services;
using UserManagement.DTOs;
using UserManagement.Exceptions;
using UserManagement.Interfaces;
using UserManagement.Models;

namespace UserManagement.Services
{

    public class CartServices : ICartServices
    {
        private readonly LibraryManagementContext _context;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(UserServices));
        private IConfiguration _config;
        private IHttpContextAccessor _httpContextAccessor;
        private int userId;
        private readonly ICommunicationServices _communicationServices;

        public CartServices(LibraryManagementContext context, IConfiguration config, IHttpContextAccessor httpContextAccessor, ICommunicationServices communicationServices)
        {
            _communicationServices = communicationServices;
            _context = context;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            var userIdString = _httpContextAccessor.HttpContext.User.Claims
           .FirstOrDefault(c => c.Type == "id")?.Value;
            userId = int.Parse(userIdString);

        }
        public async Task<List<CartDto>> RemoveCartItem(string BookName, decimal Price)
        {
            var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId);
            if (cart == null)
            {
                throw new ArgumentsException("Cart is empty");
            }
            var cartItem = cart.CartItems.FirstOrDefault(c => c.BookName == BookName);
            if (cartItem == null)
            {
                throw new ArgumentsException("Book not found in cart");
            }

            // Update CartValue by subtracting the price of the removed item
            if (Price > 0)
            {
                cart.Total= cart.Total-Price;
            }

            // Remove the item from the cart
            _context.CartItems.Remove(cartItem);
            var book = await _communicationServices.GetBook(BookName);
            _communicationServices.UpdateNoOfBook(book, true);

            // Save changes to the database
            await _context.SaveChangesAsync();
            var cartItems = await ViewCartItems();
            return cartItems;

        }
        public async Task<List<CartDto>> BorrowCartItem()
        {
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart == null || !cart.CartItems.Any())
            {
                throw new ArgumentException("No books found in cart");
            }

            // Initialize a list to store the borrowed items
            var borrowedItems = new List<Borrow>();

            foreach (var cartItem in cart.CartItems)
            {
                // Create a new Borrow record for each cart item
                var borrow = new Borrow
                {
                    UserId = cart.UserId,
                    BookName = cartItem.BookName,
                    BorrowDate = DateOnly.FromDateTime(DateTime.Now),
                    ReturnDate = DateOnly.FromDateTime(DateTime.Now).AddDays(15),
                    IsReturned = 0 // Assuming 0 means not returned
                };


                borrowedItems.Add(borrow);
            }
            if (cart.Total > 0)
            {
                cart.Total = 0;
            }

           _context.Borrows.AddRange(borrowedItems);
           _context.CartItems.RemoveRange(cart.CartItems);

            await _context.SaveChangesAsync();
            var cartItems = await ViewCartItems();
            return cartItems;
        }

        public async Task<string> AddCartItem(string BookName,decimal Price)
        {
            var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId);
            var NoOfBook = await _communicationServices.CheckNoOfBook(BookName);
            if (NoOfBook <= 0)
            {
                throw new IdNotFoundException("No books availabe");
            }

            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = userId,
                    CartItems = new List<CartItem>(),
                    Total = 0
                };
                _context.Carts.Add(cart);
            }
            var cartItem = cart.CartItems.FirstOrDefault(c => c.BookName == BookName);
            var borrows = await _context.Borrows
                  .Where(b => b.UserId == userId && b.IsReturned==0)
                  .ToListAsync();
            if (cartItem != null)
            {
                throw new ArgumentsException("Book already found in cart");
            }
            else if (borrows.Any(b => b.BookName == BookName))
            {
                throw new ArgumentsException("Book already found borrowed and not yet returned");
            }

            else 
            {
                var book = await _communicationServices.GetBook(BookName);
                _communicationServices.UpdateNoOfBook(book, false);
    

                // If the item does not exist, add it to the cart
                cartItem = new CartItem
                {
                    BookName = BookName,
                    Price = Price
                };
                cart.CartItems.Add(cartItem);
            }

            // Update the total value of the cart
            cart.Total += cartItem.Price;

            // Save changes to the database
            await _context.SaveChangesAsync();

            return $"Book '{BookName}' successfully added to the cart.";
        }
        public async Task<List<CartDto>> ViewCartItems()
        {
            // Find the cart for the current user
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            // If the cart doesn't exist, return an empty list
            if (cart == null)
            {
                return new List<CartDto>();
            }

            // Map the cart items to CartDto
            var cartItemsDto = cart.CartItems.Select(item => new CartDto
            {
                BookName = item.BookName,
                Price = item.Price
            }).ToList();

            return cartItemsDto;
        }
        public async Task<decimal> GetCartValue()
        {
            // Find the cart for the current user
            var cart = await _context.Carts
                .FirstOrDefaultAsync(c => c.UserId == userId);

            // If the cart doesn't exist, return 0
            if (cart == null)
            {
                throw new ArgumentsException("Cart is empty");
            }

            // Return the total value of the cart
            return cart.Total;
        }

        public async Task<List<BorrowUserDto>> GetAllBorrows()
        {
            var borrows = await (from borrow in _context.Borrows
                                 join user in _context.Users
                                 on borrow.UserId equals user.UserId
                                 select new BorrowUserDto
                                 {
                                     BorrowId = borrow.BorrowId,
                                     UserName = user.UserName, 
                                     BookName = borrow.BookName,
                                     BorrowDate = borrow.BorrowDate,
                                     ReturnDate = borrow.ReturnDate,
                                     IsReturned = borrow.IsReturned
                                 }).ToListAsync();

            if (borrows.Count == 0)
            {
                throw new ArgumentsException("Borrow List is empty");
            }

            return borrows;
        }
        

        public async Task<List<Borrow>> GetUserBorrows()
        {
           
            var borrows = await _context.Borrows
                   .Where(b => b.UserId == userId && b.IsReturned==0)
                   .ToListAsync();

           
            if (borrows == null)
            {
                throw new ArgumentsException("Borrow List is empty");
            }

            
            return borrows;
        }
        public async Task<List<Borrow>> ReturnBook(string bookName)
        {
          
            var borrow = await _context.Borrows
                .FirstOrDefaultAsync(b => b.BookName == bookName && b.UserId == userId && b.IsReturned==0);

          
            if (borrow == null)
            {
                throw new ArgumentsException("Borrow record not found for the specified book.");
            }
            if (borrow.IsReturned == 1)
            {
                throw new ArgumentsException("This book has already been returned.");
            }
            
            borrow.IsReturned = 1;
            var book = await _communicationServices.GetBook(bookName);
            await _communicationServices.UpdateNoOfBook(book, true);

            
            await _context.SaveChangesAsync();
            var borrows = await GetUserBorrows();
            return borrows;
        }
    }

}


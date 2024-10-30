using Microsoft.AspNetCore.Mvc;
using UserManagement.DTOs;
using UserManagement.Models;

namespace UserManagement.Interfaces
{
    public interface ICartServices
    {
        public Task<List<CartDto>> ViewCartItems();
        public Task<decimal> GetCartValue();
        public Task<string> AddCartItem(string BookName,decimal Price);
        public Task<List<CartDto>> RemoveCartItem(string BookName, decimal Price);
        public Task<List<CartDto>> BorrowCartItem();
        public Task<List<BorrowUserDto>> GetAllBorrows();
        public Task<List<Borrow>> GetUserBorrows();
        public Task<List<Borrow>> ReturnBook(string BookName);
    }
}



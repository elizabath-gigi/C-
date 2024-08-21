using Microsoft.AspNetCore.Mvc;
using UserManagement.DTOs;
using UserManagement.Models;

namespace UserManagement.Interfaces
{
    public interface ICartServices
    {
        public Task<List<CartDto>> ViewCartItems();
        public Task<string> AddCartItem(string BookName);
        public Task<string> RemoveCartItem(string BookName);
        public Task<string> BorrowCartItem();
        public Task<List<Borrow>> GetAllBorrows();
        public Task<List<Borrow>> GetUserBorrows();
    }
}



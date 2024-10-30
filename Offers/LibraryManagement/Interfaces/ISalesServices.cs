using LibraryManagement.DTOs;
using LibraryManagement.Models;
namespace LibraryManagement.Interfaces
{
    public interface ISalesServices
    { 
    
        public Task<List<Sale>> GetAllSales();
        public Task<Sale> AddSales(SaleDto request);
        public Task<Sale> UpdateSales(SaleDto request);
        public Task<Sale> DeleteSales(int id);
    }
}

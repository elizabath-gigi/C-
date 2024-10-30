using LibraryManagement.Models;
using Microsoft.AspNetCore.Hosting;
using LibraryManagement.Models;
using LibraryManagement.Exceptions;
using LibraryManagement.Interfaces;
using LibraryManagement.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using System.Data;

namespace LibraryManagement.Services
{
    public class SaleServices:ISalesServices
    {
        private readonly LibraryManagementContext _libraryContext;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LibraryServices));
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SaleServices(LibraryManagementContext libraryContext, IHttpContextAccessor httpContextAccessor)
        {
            _libraryContext = libraryContext;
            _httpContextAccessor = httpContextAccessor;

            // Set the UserId in log4net's global context
            //_userId = _httpContextAccessor.HttpContext.Session.GetString("UserId");
            //ThreadContext.Properties["UserId"] = _userId;
        }
        public async Task<List<Sale>> GetAllSales()
        {
            var sale = await _libraryContext.Sales.ToListAsync();
            if (sale.Count == 0)
            {
                log.Debug("The library DB is null");
                CallStoredProcedureAsync("DEBUG", "The library DB is null");
            }
            
            return sale;
        }
        public async Task<Sale> AddSales(SaleDto request)
        {
            var existing=await _libraryContext.Sales.FirstOrDefaultAsync(s=>s.SaleName==request.SaleName);
            if (existing != null)
            {
                throw new ArgumentsException("The sale already exists");
            }
            var sale = new Sale();
           
            sale.SaleName= request.SaleName;
            sale.SaleDescription=request.SaleDescription;
            sale.StartDate= request.StartDate;
            sale.EndDate= request.EndDate;
            sale.Books = null;
            _libraryContext.Sales.Add(sale);
            _libraryContext.SaveChanges();
            return sale;
        }
        public async Task<Sale> UpdateSales(SaleDto request)
        {
            var existing = await _libraryContext.Sales.FirstOrDefaultAsync(s => s.SaleName == request.SaleName);
            if (existing == null)
            {
                throw new ArgumentsException("The sale doesn't exists");
            }
            existing.SaleName= request.SaleName;
            existing.SaleDescription= request.SaleDescription;
            existing.StartDate= request.StartDate;
            existing.EndDate= request.EndDate;
            existing.Books=existing.Books;
            _libraryContext.SaveChanges();
            return existing;
        }
        public async Task<Sale> DeleteSales(int id)
        {
            var existing = await _libraryContext.Sales.FirstOrDefaultAsync();
            if (existing == null)
            {
                throw new ArgumentsException("The sale doesn't exists");
            }
            _libraryContext.Sales.Remove(existing);
            _libraryContext.SaveChanges();
            return existing;
        }


        public void CallStoredProcedureAsync(string level, string message)
        {
            try
            {
                //var userIdString = _httpContextAccessor.HttpContext.Session.GetString("UserId");
                //int.TryParse(userIdString, out int userId);
                //var user = _httpContextAccessor.HttpContext.Items["User"] as User;
                var userId = _httpContextAccessor.HttpContext.Items["UserId"];
                var logDate = DateTime.UtcNow;
                var thread = Thread.CurrentThread.ManagedThreadId.ToString();
                var logger = nameof(LibraryServices);


                var userIdParameter = new SqlParameter("@UserId", SqlDbType.Int) { Value = userId };
                var dateParameter = new SqlParameter("@LogDate", SqlDbType.DateTime) { Value = logDate };
                var threadParameter = new SqlParameter("@Thread", SqlDbType.NVarChar, 255) { Value = thread };
                var levelParameter = new SqlParameter("@Level", SqlDbType.NVarChar, 50) { Value = level };
                var loggerParameter = new SqlParameter("@Logger", SqlDbType.NVarChar, 255) { Value = logger };
                var messageParameter = new SqlParameter("@Message", SqlDbType.NVarChar, 4000) { Value = message };

                _libraryContext.Database.ExecuteSqlRaw(
                   "EXEC Logging @UserId, @LogDate, @Thread, @Level, @Logger, @Message",
                   userIdParameter, dateParameter, threadParameter, levelParameter, loggerParameter, messageParameter
               );
            }
            catch
            {
                log.Debug("Some errors with stored procedure");
            }

        }
    }
}

using LibraryManagement.Interfaces;
using OrderManagement.Services;
using System.Diagnostics.Eventing.Reader;
using System.Net.Http;
using UserManagement.DTOs;
using UserManagement.Models;

namespace LibraryManagement.Services
{
    public class CommunicationServices : ICommunicationServices
    {
        private readonly LibraryManagementContext _context;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(UserServices));
        private IConfiguration _config;
        private readonly HttpClient _httpClient;
        private IHttpContextAccessor _httpContextAccessor;
       

        public CommunicationServices(HttpClient httpClient,LibraryManagementContext context, IConfiguration config, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _config = config;
            _httpContextAccessor = httpContextAccessor;
            _httpClient = httpClient;
        }
        public async Task<BookDto> GetBook(string BookName)
        {
           try
            {
                var apiUrl = $"https://localhost:7254/Library/getByBookName?BookName={BookName}";
                var response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                var book = await response.Content.ReadFromJsonAsync<BookDto>();

                return book;
            }
            catch (HttpRequestException ex)
            {

                throw new Exception("Error fetching book details.", ex);
            }

        }
        public async Task<int?> CheckNoOfBook(string BookName)
        {
            try
            {
                var apiUrl = $"https://localhost:7254/Library/getByBookName?BookName={BookName}";
                var response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();
                var book = await response.Content.ReadFromJsonAsync<BookDto>();

       
                if (book == null)
                {
                    throw new Exception("Book not found.");
                }

               
                return book.NoOfBook;
            }
            catch (HttpRequestException ex)
            {
                
                throw new Exception("Error fetching book details.", ex);
            }
          
        }

        public async Task<string> UpdateNoOfBook(BookDto book, bool increase)
        {
            
            int change = increase ? 1 : -1;
            book.NoOfBook=book.NoOfBook+change;

           
            var apiUrl = $"https://localhost:7254/Library/updateNoOfBooks?BookName={book.BookName}&NoOfBook={book.NoOfBook}";

           
           
                try
                {
                   
                    var response = await _httpClient.PostAsync(apiUrl, null);

                    
                    if (response.IsSuccessStatusCode)
                    {
                        
                        var updatedBookCount = await response.Content.ReadFromJsonAsync<int>();

                        
                        return increase
                            ? $"The number of books for '{book.BookName}' has been increased by 1. Updated count: {updatedBookCount}."
                            : $"The number of books for '{book.BookName}' has been decreased by 1. Updated count: {updatedBookCount}.";
                    }
                    else
                    {
                        
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        return $"Failed to update the number of books for '{book.BookName}'. Error: {errorMessage}";
                    }
                }
                catch (HttpRequestException ex)
                {
                    
                    return $"An error occurred while updating the number of books for '{book.BookName}'. Exception: {ex.Message}";
                }
            }
        


    }
}

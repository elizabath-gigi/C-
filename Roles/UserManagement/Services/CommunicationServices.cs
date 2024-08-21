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
            // Construct the API endpoint URL
            var apiUrl = $"https://localhost:7254/Library/getByBookName?BookName={BookName}";

            // Send the GET request
            var response = await _httpClient.GetAsync(apiUrl);

            // Ensure the response indicates success
            response.EnsureSuccessStatusCode();

            // Parse the response body as BookDto
            var book = await response.Content.ReadFromJsonAsync<BookDto>();

            return book;
        }
        public async Task<int?> CheckNoOfBook(string BookName)
        {
            try
            {
                // Call the GetBookByName API
                var apiUrl = $"https://localhost:7254/Library/getByBookName?BookName={BookName}";

                // Send the GET request to retrieve the book details
                var response = await _httpClient.GetAsync(apiUrl);

                // Ensure the response indicates success
                response.EnsureSuccessStatusCode();

                // Parse the response body as a BookDto object
                var book = await response.Content.ReadFromJsonAsync<BookDto>();

                // Check if the book is null
                if (book == null)
                {
                    throw new Exception("Book not found.");
                }

                // Return the number of books available
                return book.NoOfBook;
            }
            catch (HttpRequestException ex)
            {
                // Log or handle the HTTP request exception
                throw new Exception("Error fetching book details.", ex);
            }
            catch (Exception ex)
            {
                // Log or handle other exceptions
                throw new Exception("An error occurred while checking book availability.", ex);
            }
        }

        public async Task<string> UpdateNoOfBook(BookDto book, bool increase)
        {
            // Determine the change value based on the increase parameter
            int change = increase ? 1 : -1;
            book.NoOfBook=book.NoOfBook+change;

            // Construct the API URL
            var apiUrl = $"https://localhost:7254/Library/updateNoOfBooks?BookName={book.BookName}&NoOfBook={book.NoOfBook}";

            // Initialize HttpClient to call the API
            using (var httpClient = new HttpClient())
            {
                try
                {
                    // Send the API request
                    var response = await httpClient.PostAsync(apiUrl, null);

                    // Check if the response is successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Get the updated number of books from the response
                        var updatedBookCount = await response.Content.ReadFromJsonAsync<int>();

                        // Return a success message based on the operation performed
                        return increase
                            ? $"The number of books for '{book.BookName}' has been increased by 1. Updated count: {updatedBookCount}."
                            : $"The number of books for '{book.BookName}' has been decreased by 1. Updated count: {updatedBookCount}.";
                    }
                    else
                    {
                        // Handle cases where the API request fails
                        var errorMessage = await response.Content.ReadAsStringAsync();
                        return $"Failed to update the number of books for '{book.BookName}'. Error: {errorMessage}";
                    }
                }
                catch (HttpRequestException ex)
                {
                    // Handle exceptions such as network errors
                    return $"An error occurred while updating the number of books for '{book.BookName}'. Exception: {ex.Message}";
                }
            }
        }


    }
}

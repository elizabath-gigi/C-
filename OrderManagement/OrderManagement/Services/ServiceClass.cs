using OrderManagement.Model;


namespace OrderManagement.Services
{
    public class ServiceClass:IserviceClass
    {
        public static List<Books> books = new List<Books>()
        {
                new Books()
                {
                    bookId = 1,
                    bookName="Alice in Wonderland",
                    bookAuthor="Lewis Caroll",
                    noOfBook=10
                },
                 new Books()
                {
                    bookId = 2,
                    bookName="Pride nad Prejudice",
                    bookAuthor="Jane Austin",
                    noOfBook=15
                }
        };
        public async Task<List<Books>> GetBooks()
        {
            return await Task.FromResult(books);
        }
        public async Task<Books> GetBook(int id)
        {
            var book = books.Find(x => x.bookId == id);
            return await Task.FromResult(book);
        }

        public async Task<List<Books>> AddBook(Books request)
        {
            books.Add(request);
            return await Task.FromResult(books);
        }

        public async Task<List<Books>> UpdateBook(Books request)
        {
            var book = books.Find(x => x.bookId == request.bookId);

            if (book == null)
            {
                // Handle not found
                return null;
            }

            book.noOfBook = request.noOfBook;
            book.bookName = request.bookName;
            book.bookAuthor = request.bookAuthor;

            return await Task.FromResult(books);
        }

        public async Task<List<Books>> DeleteBook(int id)
        {
            var book = books.Find(x => x.bookId == id);

            if (book == null)
            {
                // Handle not found
                return null;
            }

            books.Remove(book);

            return await Task.FromResult(books);
        }
    
    }
}


using OrderManagement.Models;

namespace OrderManagement.Interface
{
    public interface IDynamicLibraryServices
    {
        public Task<string> BulkUploadDynamic(string fileName);
       
        public Task<Library> UpdateBook(Library request);
       


    }
}

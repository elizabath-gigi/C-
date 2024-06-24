using OrderManagement.Models;

namespace OrderManagement.Interface
{
    public interface IDynamicLibraryServices
    {
        public Task<string> bulkUploadDynamic(string fileName);
       
        public Task<Library> UpdateBook(Library request);
       


    }
}

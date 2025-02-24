using QuickForm.Common.Domain;
using Microsoft.AspNetCore.Http;
namespace QuickForm.Common.Application;
public interface IAzureBlobStorageService
{
    Task DeleteFileBlobAsync(string blobContainerName, string blobName);
    Task<ResultT<(Stream, string)>> GetFileBlobAsync(string blobContainerName, string blobName, bool useStreaming = true);
    Task<BlobInformation> UploadFileBlobAsync(string blobContainerName, IFormFile file, string? folderName = null);
    Result BlobContainerExists(string blobContainerName);
}

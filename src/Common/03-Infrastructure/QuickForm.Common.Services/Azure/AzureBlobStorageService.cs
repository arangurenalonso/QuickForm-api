using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;

namespace QuickForm.Common.Infrastructure;

public class AzureBlobStorageService : IAzureBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;

    public AzureBlobStorageService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }


    public async Task<ResultT<(Stream, string)>> GetFileBlobAsync(string blobContainerName, string blobName, bool useStreaming = true)
    {
        var containerClient = await GetContainerClient(blobContainerName);
        var blobClient = containerClient.GetBlobClient(blobName);

        if (!await blobClient.ExistsAsync())
        {
            var error = ResultError.NullValue("Blob", $"The Blob '{blobName}' not found.");
            return ResultT<(Stream, string)>.Failure(ResultType.NotFound, error);
        }

        Stream contentStream;
        string contentType;

        if (useStreaming)
        {
            var downloadResponse = await blobClient.DownloadStreamingAsync();
            contentStream = downloadResponse.Value.Content;
            contentType = downloadResponse.Value.Details.ContentType;
        }
        else
        {
            var blobDownloadInfo = await blobClient.DownloadAsync();
            contentStream = blobDownloadInfo.Value.Content;
            contentType = blobDownloadInfo.Value.Details.ContentType;
        }
        return (contentStream, contentType);
    }
    public async Task<BlobInformation> UploadFileBlobAsync(string blobContainerName, IFormFile file, string? folderName = null)
    {
        var containerClient = await GetContainerClient(blobContainerName);
        var filename = GetFileNameWithGUUID(file);
        string blobName = folderName != null ? $"{folderName}/{filename}" : filename;

        var blobClient = containerClient.GetBlobClient(blobName);

        using (var stream = file.OpenReadStream())
        {
            await blobClient.UploadAsync(stream, new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = file.ContentType } });
        }
        var blobInfo = new BlobInformation
        {
            BlobUri = blobClient.Uri,
            BlobContainerName = blobContainerName,
            BlobName = blobName,
            ContentType = file.ContentType,
            Size = file.Length,
            FileExtension = Path.GetExtension(file.FileName)
        };
        return blobInfo;
    }

    private string GetFileNameWithGUUID(IFormFile file)
    {
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.FileName);
        string fileExtension = Path.GetExtension(file.FileName);

        var normalizedString = fileNameWithoutExtension.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();
        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }
        string fileNameWithoutTildes = stringBuilder.ToString().Normalize(NormalizationForm.FormC);

        fileNameWithoutExtension = Regex.Replace(fileNameWithoutTildes, @"[^a-zA-Z0-9\-_]", "");

        string filename = $"{fileNameWithoutExtension}-{Guid.NewGuid()}{fileExtension}";
        filename = filename.Replace(" ", "_");
        return filename.ToLower(CultureInfo.InvariantCulture);
    }
    public async Task DeleteFileBlobAsync(string blobContainerName, string blobName)
    {
        var containerClient = await GetContainerClient(blobContainerName);
        var blobClient = containerClient.GetBlobClient(blobName);
        await blobClient.DeleteIfExistsAsync();
    }

    private async Task<BlobContainerClient> GetContainerClient(string blobContainerName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);
        await containerClient.CreateIfNotExistsAsync();
        return containerClient;
    }
    public Result BlobContainerExists(string blobContainerName)
    {
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(blobContainerName);
        var containerExist = containerClient.Exists();
        if (!containerExist)
        {
            var error = ResultError.NullValue("BlobContainer", $"The BlobContainer '{blobContainerName}' not found.");
            return ResultT<(Stream, string)>.Failure(ResultType.NotFound, error);

        }
        return Result.Success();
    }
}

namespace QuickForm.Common.Application;
public class BlobInformation
{
    public Uri BlobUri { get; set; }
    public string BlobContainerName { get; set; }
    public string BlobName { get; set; }
    public string ContentType { get; set; }
    public string FileExtension { get; set; }
    public long Size { get; set; }
}

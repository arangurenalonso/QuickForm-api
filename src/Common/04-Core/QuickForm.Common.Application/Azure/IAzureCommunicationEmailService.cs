using Microsoft.AspNetCore.Http;

namespace QuickForm.Common.Application;
public interface IAzureCommunicationEmailService
{ 
    Task SendEmailAsync(string to, string subject, string body);
    Task SendEmailWithAttachmentAsync(string to, string subject, string body, IFormFile? attachmentFile = null);
    Task SendEmailWithAttachmentAsync(List<DatosDestinatario> destinatarios, string subject, string body, IFormFile? attachmentFile = null);
    Task SendEmailWithMultipleAttachmentAsync(string to, string subject, string body, List<(Stream stream, string fileName)> listStreams);
}

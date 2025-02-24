using Azure;
using Azure.Communication.Email;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using QuickForm.Common.Application;

namespace QuickForm.Common.Infrastructure;
public class AzureCommunicationEmailService : IAzureCommunicationEmailService
{
    private readonly AzureCommunicationEmailOptions _options;
    private readonly EmailClient _emailClient;

    public AzureCommunicationEmailService(IOptions<AzureCommunicationEmailOptions> options)
    {
        _options = options.Value;
        _emailClient = new EmailClient(_options.ConnectionString);
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        await SendEmailWithAttachmentAsync(to, subject, body, null);
    }

    public async Task SendEmailWithAttachmentAsync(
        string to, 
        string subject, 
        string body, 
        IFormFile? attachmentFile = null)
    {
        var emailMessage = await BuildEmailMessage(to, subject, body, attachmentFile);
        await SendMessage(emailMessage);
    }

    public Task SendEmailWithAttachmentAsync(
        List<DatosDestinatario> destinatarios, 
        string subject, 
        string body, 
        IFormFile? attachmentFile = null)
    {
        throw new NotImplementedException();
    }

    public async Task SendEmailWithMultipleAttachmentAsync(
        string to, string subject, string body, List<(Stream stream, string fileName)> listStreams)
    {
        var emailMessage =await BuildEmailMessage(to, subject, body, null, listStreams);
        await SendMessage(emailMessage);
    }

    private async Task<EmailMessage> BuildEmailMessage(
        string to, 
        string subject, 
        string body, 
        IFormFile? attachmentFile, 
        List<(Stream stream, string fileName)>? multipleAttachments = null)
    {
        // Crear la lista de destinatarios
        var toRecipients = new List<EmailAddress> { new EmailAddress(to) };
        var recipients = new EmailRecipients(toRecipients);

        // Crear el contenido del correo
        var content = new EmailContent(subject)
        {
            Html = body // Aquí estableces el contenido HTML
                        // También puedes agregar PlainText si lo deseas
                        // PlainText = "Texto plano opcional"
        };


        // Adjuntos
        var attachments = new List<EmailAttachment>();

        //// Un solo adjunto (attachmentFile)
        if (attachmentFile != null && attachmentFile.Length > 0)
        {
            await using var ms = new MemoryStream();
            await attachmentFile.CopyToAsync(ms);
            ms.Position = 0;
            var contentBytes = ms.ToArray();
            var binaryData = new BinaryData(contentBytes);

            // No convertir a Base64, pasar el contenido directamente
            attachments.Add(new EmailAttachment(
                name: attachmentFile.FileName,
                contentType: attachmentFile.ContentType ?? "application/octet-stream",
                content: binaryData
            ));
        }

        //// Múltiples adjuntos
        if (multipleAttachments != null)
        {
            foreach (var (stream, fileName) in multipleAttachments)
            {
                await using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                ms.Position = 0;
                var contentBytes = ms.ToArray();
                var binaryData = new BinaryData(contentBytes);
                attachments.Add(new EmailAttachment(
                    name: fileName,
                    contentType: "application/octet-stream",
                    content: binaryData
                ));
            }
        }
        // Crear el mensaje con el remitente, contenido y destinatarios "To"
        var emailMessage = new EmailMessage(
             senderAddress: _options.FromEmail,
             content: content,
             recipients: recipients
         );
        if (attachments.Count > 0)
        {
            foreach (var item in emailMessage.Attachments)
            {
                emailMessage.Attachments.Add(item);
            }
        }
        return emailMessage;
    }

    private async Task SendMessage(EmailMessage emailMessage)
    {
        try
        {
            // Enviar el correo
            EmailSendOperation sendOperation = await _emailClient.SendAsync(WaitUntil.Completed, emailMessage);
            EmailSendResult sendResult = sendOperation.Value;

            if (sendResult.Status != EmailSendStatus.Succeeded)
            {
                throw new ApplicationException($"Error al enviar el correo a través de Azure Communication Services: {sendResult.Status}");
            }
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Error al enviar el correo electrónico por Azure Communication Service: " + ex.Message);
        }
    }

}

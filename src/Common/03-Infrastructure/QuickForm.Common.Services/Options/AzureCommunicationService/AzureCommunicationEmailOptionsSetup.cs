using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace QuickForm.Common.Infrastructure;
public class AzureCommunicationEmailOptionsSetup : IConfigureOptions<AzureCommunicationEmailOptions>
{
    private const string SectionName = "Common:AzureCommunicationEmail";
    private readonly IConfiguration _configuration;

    public AzureCommunicationEmailOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(AzureCommunicationEmailOptions options)
    {
        _configuration.GetSection(SectionName).Bind(options);
    }
}

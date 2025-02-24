using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace QuickForm.Modules.Survey.Options;
public class OutboxOptionsSetup : IConfigureOptions<OutboxOptions>
{
    private const string SectionName = "Survey:Outbox";
    private readonly IConfiguration _configuration;

    public OutboxOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(OutboxOptions options)
    {
        _configuration.GetSection(SectionName).Bind(options);
    }
}

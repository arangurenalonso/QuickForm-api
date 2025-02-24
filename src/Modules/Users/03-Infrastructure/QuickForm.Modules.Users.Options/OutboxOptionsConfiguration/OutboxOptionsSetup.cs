using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace QuickForm.Modules.Users.Options;
public class OutboxOptionsSetup : IConfigureOptions<OutboxOptions>
{
    private const string SectionName = "Users:Outbox";
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

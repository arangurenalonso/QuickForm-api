using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace QuickForm.Modules.Users.Options;
public class InboxOptionsSetup : IConfigureOptions<InboxOptions>
{
    private const string SectionName = "Users:Inbox";
    private readonly IConfiguration _configuration;

    public InboxOptionsSetup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(InboxOptions options)
    {
        _configuration.GetSection(SectionName).Bind(options);
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace QuickForm.Common.Infrastructure;

public class RefreshTokenOptionsSetup(IConfiguration _configuration)
    : IConfigureOptions<RefreshTokenOptions>
{
    private const string SectionName = "Common:RefreshToken";

    public void Configure(RefreshTokenOptions options)
    {
        _configuration.GetSection(SectionName).Bind(options);
    }
}

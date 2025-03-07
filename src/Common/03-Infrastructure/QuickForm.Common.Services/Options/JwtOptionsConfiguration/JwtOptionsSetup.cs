using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace QuickForm.Common.Infrastructure;

public class JwtOptionsSetup(IConfiguration _configuration) 
    : IConfigureOptions<JwtOptions>
{
    private const string SectionName = "Common:Jwt";

    public void Configure(JwtOptions options)
    {
        _configuration.GetSection(SectionName).Bind(options);
    }
}

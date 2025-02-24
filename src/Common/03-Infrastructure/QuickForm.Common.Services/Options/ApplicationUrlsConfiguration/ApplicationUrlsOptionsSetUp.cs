using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace QuickForm.Common.Infrastructure;
public class ApplicationUrlsOptionsSetUp : IConfigureOptions<ApplicationUrlsOptions>
{
    private const string SectionName = "Common:ApplicationUrls";
    private readonly IConfiguration _configuration;

    public ApplicationUrlsOptionsSetUp(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void Configure(ApplicationUrlsOptions options)
    {
        _configuration.GetSection(SectionName).Bind(options);
    }
}

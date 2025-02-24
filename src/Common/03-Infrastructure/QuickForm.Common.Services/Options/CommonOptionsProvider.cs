using Microsoft.Extensions.Options;
using QuickForm.Common.Application;

namespace QuickForm.Common.Infrastructure;

public class CommonOptionsProvider : ICommonOptionsProvider
{
    private readonly ApplicationUrlsOptions _applicationUrlsOptions;

    public CommonOptionsProvider(
        IOptions<ApplicationUrlsOptions> applicationUrlsOptions
        )
    {
        _applicationUrlsOptions = applicationUrlsOptions.Value;
    }

    public Uri GetCurrentApplicationUrl()
    {
        return new Uri(_applicationUrlsOptions.CurrentApplicationURL);
    }
    public Uri GetFrontEndApplicationUrl()
    {
        return new Uri(_applicationUrlsOptions.WebUrl);
    }
}

using Microsoft.AspNetCore.Routing;

namespace QuickForm.Common.Presentation;
public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}

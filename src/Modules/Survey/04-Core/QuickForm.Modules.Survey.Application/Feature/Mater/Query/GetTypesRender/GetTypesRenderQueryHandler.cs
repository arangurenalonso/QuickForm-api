using QuickForm.Common.Application;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Application;
internal sealed class GetTypesRenderQueryHandler(
        IMasterRepository _masterRepository
    )
    : IQueryHandler<GetTypesRenderQuery, List<TypeRenderFormViewModel>>
{
    public async Task<ResultT<List<TypeRenderFormViewModel>>> Handle(GetTypesRenderQuery request, CancellationToken cancellationToken)
    {
        var result = await _masterRepository.GetTypesRenderQuery(cancellationToken);
        return result;
    }

}

using QuickForm.Common.Application;
using QuickForm.Common.Domain;
using QuickForm.Modules.Survey.Domain;

namespace QuickForm.Modules.Survey.Application;
internal sealed class GetMyFormsQueryHandler(
    )
    : IQueryHandler<GetMyFormsQuery, List<FormStructureSectionReponse>>
{
    public async Task<ResultT<List<FormStructureSectionReponse>>> Handle(GetMyFormsQuery request, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        throw new NotImplementedException();
    }

}

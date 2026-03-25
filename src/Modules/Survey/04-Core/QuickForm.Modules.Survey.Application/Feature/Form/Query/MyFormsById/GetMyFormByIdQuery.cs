
using QuickForm.Common.Application;

namespace QuickForm.Modules.Survey.Application;
public sealed record GetMyFormByIdQuery(Guid IdForm)
    : IQuery<FormViewModel>, IRequireFormOwnership
{
    public Guid FormId => IdForm;
}

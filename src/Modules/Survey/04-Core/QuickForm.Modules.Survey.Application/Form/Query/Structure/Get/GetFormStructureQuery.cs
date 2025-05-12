
using QuickForm.Common.Application;

namespace QuickForm.Modules.Survey.Application;
public sealed record GetFormStructureQuery(
        Guid IdForm
    )
    : IQuery<object>;


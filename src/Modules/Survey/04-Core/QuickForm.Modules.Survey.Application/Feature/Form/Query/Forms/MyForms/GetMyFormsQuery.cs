
using QuickForm.Common.Application;

namespace QuickForm.Modules.Survey.Application;
public sealed record GetMyFormsQuery(
    )
    : IQuery<List<FormViewModel>>;



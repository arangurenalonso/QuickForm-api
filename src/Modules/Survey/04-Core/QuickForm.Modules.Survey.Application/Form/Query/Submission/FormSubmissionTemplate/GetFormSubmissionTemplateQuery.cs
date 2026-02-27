
using QuickForm.Common.Application;

namespace QuickForm.Modules.Survey.Application;
public sealed record GetFormSubmissionTemplateQuery(
        Guid IdForm
    )
    : IQuery<GetFormSubmissionTemplateViewModel>;



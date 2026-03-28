
using QuickForm.Common.Application;

namespace QuickForm.Modules.Survey.Application;
public sealed record GetTypesRenderQuery(
    )
    : IQuery<List<TypeRenderFormViewModel>>;



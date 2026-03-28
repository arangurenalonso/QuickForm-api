namespace QuickForm.Modules.Survey.Application;

public interface IMasterRepository
{
    Task<List<TypeRenderFormViewModel>> GetTypesRenderQuery(
        CancellationToken ct = default
    );
}

using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public class FormRenderDomain : BaseMasterEntity
{

    public StatusColorVO? Color { get; private set; } = StatusColorVO.Default;
    public StatusIconVO? Icon { get; private set; }


    #region Many-to-One Relationship
    public ICollection<FormConfigDomain> FormConfig { get; private set; } = [];

    #endregion
    private FormRenderDomain() { }
    private FormRenderDomain(MasterId id) : base(id) { }


    public static ResultT<FormRenderDomain> Create(
            string keyName,
            string? description = null,
            string? color = null,
            string? icon = null
        )
    {
        var newDomain = new FormRenderDomain();
        var result = newDomain.Update(keyName, description,color,icon);

        if (result.IsFailure)
        {
            return result.Errors;
        }
        return newDomain;
    }
    public static ResultT<FormRenderDomain> CreateWithId(
            MasterId id,
            string keyName,
            string? description = null,
            string? color = null,
            string? icon = null
        )
    {
        var newDomain = new FormRenderDomain(id);
        var result = newDomain.Update(keyName, description,color,icon);
        if (result.IsFailure)
        {
            return result.Errors;
        }
        return newDomain;
    }

    public Result Update(
            string keyName,
            string? description = null,
            string? color = null,
            string? icon = null)
    {
        var masterUpdateBase = new MasterUpdateBase(keyName, description);
        var result = SetBaseProperties(masterUpdateBase);

        if (result.IsFailure)
        {
            return result.Errors;
        }

        if (!string.IsNullOrEmpty(color))
        {
            var colorResult = StatusColorVO.Create(color);
            if (colorResult.IsFailure)
            {
                return colorResult.Errors;
            }
            Color = colorResult.Value;
        }
        if (!string.IsNullOrEmpty(icon))
        {
            var iconResult = StatusIconVO.Create(icon);
            if (iconResult.IsFailure)
            {
                return iconResult.Errors;
            }
            Icon = iconResult.Value;
        }
        return Result.Success();
    }


}

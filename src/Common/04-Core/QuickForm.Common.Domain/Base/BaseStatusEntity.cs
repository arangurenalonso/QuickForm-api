namespace QuickForm.Common.Domain;
public abstract class BaseStatusEntity : BaseMasterEntity
{
    public StatusColorVO Color { get; private set; } = StatusColorVO.Default;
    public StatusIconVO? Icon { get; private set; }

    protected BaseStatusEntity() : base()
    {
    }

    protected BaseStatusEntity(
        MasterId id
        ) : base(id)
    {
    }

    protected Result SetStatusProperties(StatusPropertiesDto dto)
    {
        var baseResult = SetBaseProperties(dto);
        if (baseResult.IsFailure)
        {
            return baseResult;
        }

        var colorResult = StatusColorVO.Create(dto.Color);
        if (colorResult.IsFailure)
        {
            return colorResult.Errors; 
        }

        var iconResult = StatusIconVO.Create(dto.Icon);
        if (iconResult.IsFailure)
        {
            return iconResult.Errors;
        }

        Color = colorResult.Value;
        Icon = iconResult.Value;

        return Result.Success();
    }
    public virtual Result Update(
            string keyName,
            string color,
            string? icon = null,
            string? description = null
        )
    {
        var baseResult = base.Update(keyName, description);
        if (baseResult.IsFailure)
        {
            return baseResult;
        }
        var colorResult = StatusColorVO.Create(color);
        if (colorResult.IsFailure)
        {
            return colorResult.Errors;
        }
        if (icon is not null)
        {
            var iconResult = StatusIconVO.Create(icon);
            if (iconResult.IsFailure)
            {
                return iconResult.Errors;
            }
            Icon = iconResult.Value;
        }

        Color = colorResult.Value;

        return Result.Success();
    }
}

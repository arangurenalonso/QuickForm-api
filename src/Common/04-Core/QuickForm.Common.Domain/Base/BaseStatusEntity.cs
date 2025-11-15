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
}

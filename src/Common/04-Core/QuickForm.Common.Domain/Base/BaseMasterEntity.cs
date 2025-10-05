namespace QuickForm.Common.Domain;
public abstract class BaseMasterEntity: BaseDomainEntity<MasterId>
{
    public KeyNameVO KeyName { get; private set; }
    public DescriptionVO? Description { get; private set; }
    public int SortOrder { get; private set; }

    protected BaseMasterEntity()
     : base(MasterId.Create())
    {

    }

    protected BaseMasterEntity(MasterId id)
        : base(id)
    {
    }
    protected virtual Result SetBaseProperties(MasterUpdateBase dto)
    {
        if (dto.Description != null)
        {
            var descriptionResult = DescriptionVO.Create(dto.Description);

            if (descriptionResult.IsFailure)
            {
                return descriptionResult.Errors;
            }
            Description = descriptionResult.Value;
        }

        var KeyNameResult = KeyNameVO.Create(dto.KeyName);
        if (KeyNameResult.IsFailure)
        {
            return KeyNameResult.Errors;
        }
        KeyName = KeyNameResult.Value;
        SortOrder = dto.SortOrder ?? 0;
        return Result.Success();
    }

}

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

        var keyNameResult = KeyNameVO.Create(dto.KeyName);
        if (keyNameResult.IsFailure)
        {
            return keyNameResult.Errors;
        }
        KeyName = keyNameResult.Value;
        SortOrder = dto.SortOrder ?? 0;
        return Result.Success();
    }
    public virtual Result Update(
        string keyName,
        string? description = null
    )
    {
        var masterUpdateBase = new MasterUpdateBase(keyName, description);
        return SetBaseProperties(masterUpdateBase);
    }


}

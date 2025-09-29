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
        var descriptionResult = DescriptionVO.Create(dto.Description);
        var KeyNameResult = KeyNameVO.Create(dto.KeyName);

        if (descriptionResult.IsFailure)
        {
            var errorList = new ResultErrorList(
                new List<ResultErrorList>() { descriptionResult.Errors, KeyNameResult.Errors }
                );
            return errorList;
        }
        Description = descriptionResult.Value;
        KeyName = KeyNameResult.Value;
        SortOrder = dto.SortOrder ?? 0;
        return Result.Success();
    }

}

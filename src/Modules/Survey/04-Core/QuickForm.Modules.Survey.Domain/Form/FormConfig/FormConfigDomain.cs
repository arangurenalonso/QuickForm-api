using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public class FormConfigDomain : BaseDomainEntity<FormConfigId>
{
    public FormId FormId { get; private set; }
    public MasterId FormRenderId { get; private set; }

    #region One-to-One Relationship
    public FormDomain Form { get; private set; }
    #endregion

    #region One-to-Many Relationship
    public FormRenderDomain FormRender { get; private set; }
    #endregion
    private FormConfigDomain() { }
    private FormConfigDomain(FormConfigId id, FormId formId) : base(id) 
    { 
        FormId = formId;
        FormRenderId = new MasterId(FormRenderType.Default.GetId());
    }
    public static ResultT<FormConfigDomain> Create(
            FormId formId
        )
    {
        var formConfigId = FormConfigId.Create();
        var newDomain = new FormConfigDomain(formConfigId, formId);

        return newDomain;
    }
    public Result UpdateFormRender(MasterId formRenderId)
    {
        FormRenderId = formRenderId;
        return Result.Success();
    }



}

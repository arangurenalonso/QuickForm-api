using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public sealed class FormStatusPermissionDomain : BaseDomainEntity<FormStatusPermissionIdVO>
{
    public MasterId IdFormAction { get; private set; }
    public MasterId IdFormStatus { get; private set; }

    public FormStatusDomain FormStatus { get; private set; }
    public FormActionDomain FormAction { get; private set; }
    public FormStatusPermissionDomain() { }
    private FormStatusPermissionDomain(FormStatusPermissionIdVO id) : base(id) { }
    private FormStatusPermissionDomain(
        FormStatusPermissionIdVO id,
        MasterId idFormAction,
        MasterId idFormStatus
    ) : base(id)
    {
        IdFormAction = idFormAction;
        IdFormStatus = idFormStatus;
    }

    public static ResultT<FormStatusPermissionDomain> Create(
            FormStatusPermissionIdVO id,
            MasterId idFormAction,
            MasterId idFormStatus
        )
    {
        var newDomain = new FormStatusPermissionDomain(
            id,
            idFormAction,
            idFormStatus
        );
        return newDomain;
    }


}

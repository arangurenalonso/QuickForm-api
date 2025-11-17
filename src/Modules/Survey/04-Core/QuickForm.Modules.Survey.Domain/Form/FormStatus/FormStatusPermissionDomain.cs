using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public sealed class FormStatusPermissionDomain : BaseDomainEntity<FormStatusPermissionId>
{
    public MasterId IdFormAction { get; private set; }
    public MasterId IdFormStatus { get; private set; }

    public FormStatusDomain FormStatus { get; private set; }
    public FormActionDomain FormAction { get; private set; }
    public FormStatusPermissionDomain() { }
    private FormStatusPermissionDomain(FormStatusPermissionId id) : base(id) { }
    private FormStatusPermissionDomain(
        FormStatusPermissionId id,
        MasterId idFormAction,
        MasterId idFormStatus
    ) : base(id)
    {
        IdFormAction = idFormAction;
        IdFormStatus = idFormStatus;
    }

    public static ResultT<FormStatusPermissionDomain> Create(
            FormStatusPermissionId id,
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

    public void Update(
        MasterId idFormAction,
        MasterId idFormStatus
    )
    {
        IdFormAction = idFormAction;
        IdFormStatus = idFormStatus;
    }

}

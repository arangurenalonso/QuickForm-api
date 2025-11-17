using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public sealed class FormStatusHistoryDomain : BaseDomainEntity<FormStatusHistoryId>
{
    public FormId IdForm { get; private set; }
    public MasterId IdFormStatus { get; private set; }

    public FormDomain Form { get; private set; }
    public FormStatusDomain FormStatus { get; private set; }
    public FormStatusHistoryDomain() { }
    private FormStatusHistoryDomain(FormStatusHistoryId id) : base(id) { }
    private FormStatusHistoryDomain(
        FormStatusHistoryId id,
        FormId idForm,
        MasterId idFormStatus
    ) : base(id)
    {
        IdForm = idForm;
        IdFormStatus = idFormStatus;
    }

    public static ResultT<FormStatusHistoryDomain> Create(
            FormId idForm,
            MasterId idFormStatus
        )
    {
        var id = FormStatusHistoryId.Create();
        var newDomain = new FormStatusHistoryDomain(
            id,
            idForm,
            idFormStatus
        );
        return newDomain;
    }

}

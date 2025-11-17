using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public enum FormActionType
{
    [Id("36EE108D-369D-46F2-90C3-FAD3F4633EB3")]
    [Name("FormEdit")]
    FormEdit = 1,

    [Id("0F19EDF8-F0B5-45AC-B972-A5A92DFD7404")]
    [Name("FormPublish")]
    FormPublish = 2,

    [Id("BD9122A1-CC35-41B2-A6D3-1B722D6C5CA3")]
    [Name("FormPause")]
    FormPause = 3,

    [Id("C60C8D1B-F3E8-4D0A-9C66-FD6A5023CABD")]
    [Name("FormClose")]
    FormClose = 4,

    [Id("DCDEA642-18D3-461E-B5E1-4C9F77877FBC")]
    [Name("FormResume")]
    FormResume = 5,

    [Id("40D5DD5A-EBCB-48F3-A36E-3822459249C8")]
    [Name("ViewResponses")]
    ViewResponses = 6,
}

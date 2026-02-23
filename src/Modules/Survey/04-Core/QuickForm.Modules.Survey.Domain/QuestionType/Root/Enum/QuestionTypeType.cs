using QuickForm.Common.Domain;
using System.ComponentModel;

namespace QuickForm.Modules.Survey.Domain;

public enum QuestionTypeType
{
    [Name("InputTypeText")]
    [Id("5272D526-C6FD-4F84-AA7E-A954DF645F4B")]
    InputTypeText = 1,

    [Name("InputTypeInteger")]
    [Id("0B8461E8-DC83-45A1-9280-0503FDFBC5EB")]
    InputTypeInteger = 2,

    [Name("InputTypeDecimal")]
    [Id("C1B8F3B7-9E5B-4C9A-BDCD-1F2E5B8F6A9D")]
    InputTypeDecimal = 3,

}

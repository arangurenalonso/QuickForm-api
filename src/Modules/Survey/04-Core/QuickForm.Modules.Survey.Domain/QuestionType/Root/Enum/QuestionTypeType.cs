using QuickForm.Common.Domain;
using System.ComponentModel;

namespace QuickForm.Modules.Survey.Domain;

public enum QuestionTypeType
{
    [Detail("InputTypeText")]
    [Description("5272D526-C6FD-4F84-AA7E-A954DF645F4B")]
    InputTypeText = 1,

    [Detail("InputTypeNumber")]
    [Description("0B8461E8-DC83-45A1-9280-0503FDFBC5EB")]
    InputTypeNumber = 2,

}

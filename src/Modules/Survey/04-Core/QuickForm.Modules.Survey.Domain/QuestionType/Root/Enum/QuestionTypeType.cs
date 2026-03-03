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

    [Name("InputTypeDate")]
    [Id("A3F5E2C4-7B9D-4E6F-8C1A-2D3B4E5F6A7B")]
    InputTypeDate = 4,

    [Name("InputTypeDatetime")]
    [Id("E5F6A7B8-9C0D-4E1F-8G2A-4C5D6E7F8G9H")]
    InputTypeDatetime = 5,

    [Name("InputTypeTime")]
    [Id("F6A7B8C9-0D1E-4F2A-8H3B-5D6E7F8G9H0I")]
    InputTypeTime = 6,

    [Name("InputTypeBoolean")]
    [Id("D4E5F6A7-8B9C-4D1E-8F2A-3B4C5D6E7F8G")]
    InputTypeBoolean = 7,

}

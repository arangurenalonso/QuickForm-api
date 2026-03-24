using QuickForm.Common.Domain;

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

    [Name("InputTypeBoolean")]
    [Id("DE69DD93-979A-4D45-9928-3DB5B39E81B2")]
    InputTypeBoolean = 4,

    [Name("InputTypeDate")]
    [Id("439AB327-A519-4823-8086-2A20335A3260")]
    InputTypeDate = 5,

    [Name("InputTypeDatetime")]
    [Id("59923D41-CDBF-4AB8-B509-CE15639E2055")]
    InputTypeDatetime = 6,

    [Name("InputTypeTime")]
    [Id("EDC7F74F-15BF-464D-880B-A177C65560C4")]
    InputTypeTime = 7,

    [Name("InputTypeSelect")]
    [Id("CC35C6D9-A1E7-4DAB-8EB1-07CBBE288F55")]
    InputTypeSelect = 8,


    [Name("Status")]
    [Id("D856CD38-3788-48EE-8199-9135C204D62B")]
    Status = 98,

    [Name("Action")]
    [Id("E43B13DB-CEA5-48C6-B4ED-CF935113A858")]
    Action = 99,




}

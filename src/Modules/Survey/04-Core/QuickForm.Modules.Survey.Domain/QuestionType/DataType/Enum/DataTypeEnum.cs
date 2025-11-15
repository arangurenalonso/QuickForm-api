using QuickForm.Common.Domain;
using System.ComponentModel;

namespace QuickForm.Modules.Survey.Domain;

public enum DataTypeType
{
    [Name("string")]
    [Id("82478251-A1F8-4647-B852-9956E208D7FC")]
    StringType=1,

    [Name("int")]
    [Id("037422C2-9096-47A1-903A-6B9784F238EE")]
    IntType=2,
    
    [Name("decimal")]
    [Id("AC960635-4FE6-4575-A2CD-F791AE5283B0")]
    DecimalType=3,
    
    [Name("bool")]
    [Id("ACFE7FFD-C780-46BD-AAA9-A86B79B38F1F")]
    BooleanType=4,
    
    [Name("datetime")]
    [Id("84DD58EF-0E4C-473E-B6C3-5617DA52CB4B")]
    DatetimeType=5
}

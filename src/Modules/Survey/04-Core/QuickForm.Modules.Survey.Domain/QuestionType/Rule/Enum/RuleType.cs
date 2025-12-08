using QuickForm.Common.Domain;
using System.ComponentModel;

namespace QuickForm.Modules.Survey.Domain;

public enum RuleType
{
    [Name("Required")]
    [Id("BAF7F7C6-16C4-4935-A11E-DAAB6332E361")]
    Required = 1,

    [Name("Min")]
    [Id("D5F9D516-BF4B-4E3B-90A5-F85E2078E07B")]
    Min = 2,

    [Name("Max")]
    [Id("0F7A4FD2-835E-4A33-AE24-183F76691DF8")]
    Max = 3,

    [Name("MinLength")]
    [Id("80BED42D-07EA-48AB-A339-1D48920E9C19")]
    MinLength = 4,

    [Name("MaxLength")]
    [Id("617A1D41-80A6-4FBC-9147-56C4544A8088")]
    MaxLength = 5,

}

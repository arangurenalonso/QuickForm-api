

using System.ComponentModel;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;

public enum UiControlTypeType
{
    [Name("none")]
    [Description("None")]
    [Id("C2B2D3E4-F5A6-4789-92B3-C4D5E6F7A801")]
    None = 1,

    [Name("text")]
    [Description("Text")]
    [Id("C2B2D3E4-F5A6-4789-92B3-C4D5E6F7A802")]
    Text = 2,

    [Name("number")]
    [Description("Number")]
    [Id("C2B2D3E4-F5A6-4789-92B3-C4D5E6F7A803")]
    Number = 3,

    [Name("date")]
    [Description("Date")]
    [Id("C2B2D3E4-F5A6-4789-92B3-C4D5E6F7A804")]
    Date = 4,

    [Name("datetime")]
    [Description("Datetime")]
    [Id("C2B2D3E4-F5A6-4789-92B3-C4D5E6F7A805")]
    Datetime = 5,

    [Name("time")]
    [Description("Time")]
    [Id("C2B2D3E4-F5A6-4789-92B3-C4D5E6F7A806")]
    Time = 6,

    [Name("boolean")]
    [Description("Boolean")]
    [Id("C2B2D3E4-F5A6-4789-92B3-C4D5E6F7A807")]
    Boolean = 7,

    [Name("range-number")]
    [Description("Range Number")]
    [Id("C2B2D3E4-F5A6-4789-92B3-C4D5E6F7A808")]
    RangeNumber = 8,

    [Name("range-date")]
    [Description("Range Date")]
    [Id("C2B2D3E4-F5A6-4789-92B3-C4D5E6F7A809")]
    RangeDate = 9,

    [Name("range-datetime")]
    [Description("Range Datetime")]
    [Id("C2B2D3E4-F5A6-4789-92B3-C4D5E6F7A810")]
    RangeDatetime = 10,

    [Name("range-time")]
    [Description("Range Time")]
    [Id("C2B2D3E4-F5A6-4789-92B3-C4D5E6F7A811")]
    RangeTime = 11
}

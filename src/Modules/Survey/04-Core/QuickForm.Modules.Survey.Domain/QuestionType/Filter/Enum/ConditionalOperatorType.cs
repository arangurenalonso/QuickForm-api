using System.ComponentModel;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;

public enum ConditionalOperatorType
{
    [Name("contains")]
    [Description("Contains")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F701")]
    Contains = 1,

    [Name("notContains")]
    [Description("Not Contains")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F702")]
    NotContains = 2,

    [Name("equals")]
    [Description("Equals")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F703")]
    Equals = 3,

    [Name("notEquals")]
    [Description("Not Equals")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F704")]
    NotEquals = 4,

    [Name("startsWith")]
    [Description("Starts With")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F705")]
    StartsWith = 5,

    [Name("endsWith")]
    [Description("Ends With")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F706")]
    EndsWith = 6,

    [Name("greaterThan")]
    [Description("Greater Than")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F707")]
    GreaterThan = 7,

    [Name("greaterThanOrEqual")]
    [Description("Greater Than Or Equal")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F708")]
    GreaterThanOrEqual = 8,

    [Name("lessThan")]
    [Description("Less Than")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F709")]
    LessThan = 9,

    [Name("lessThanOrEqual")]
    [Description("Less Than Or Equal")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F710")]
    LessThanOrEqual = 10,

    [Name("between")]
    [Description("Between")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F711")]
    Between = 11,

    [Name("before")]
    [Description("Before")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F712")]
    Before = 12,

    [Name("after")]
    [Description("After")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F713")]
    After = 13,

    [Name("on")]
    [Description("On")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F714")]
    On = 14,

    [Name("onOrBefore")]
    [Description("On Or Before")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F715")]
    OnOrBefore = 15,

    [Name("onOrAfter")]
    [Description("On Or After")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F716")]
    OnOrAfter = 16,

    [Name("isTrue")]
    [Description("Is True")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F717")]
    IsTrue = 17,

    [Name("isFalse")]
    [Description("Is False")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F718")]
    IsFalse = 18,

    [Name("isEmpty")]
    [Description("Is Empty")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F719")]
    IsEmpty = 19,

    [Name("isNotEmpty")]
    [Description("Is Not Empty")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F720")]
    IsNotEmpty = 20
}


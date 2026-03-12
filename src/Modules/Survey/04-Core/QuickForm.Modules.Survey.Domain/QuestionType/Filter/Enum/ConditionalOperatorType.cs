using System.ComponentModel;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;

public enum ConditionalOperatorType
{
    [Name("contains")]
    [Description("Contains")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F701")]
    [Order(1)]
    Contains = 1,

    [Name("notContains")]
    [Description("Not Contains")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F702")]
    [Order(2)]
    NotContains = 2,

    [Name("equals")]
    [Description("Equals")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F703")]
    [Order(3)]
    Equals = 3,

    [Name("notEquals")]
    [Description("Not Equals")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F704")]
    [Order(4)]
    NotEquals = 4,

    [Name("startsWith")]
    [Description("Starts With")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F705")]
    [Order(5)]
    StartsWith = 5,

    [Name("endsWith")]
    [Description("Ends With")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F706")]
    [Order(6)]
    EndsWith = 6,

    [Name("greaterThan")]
    [Description("Greater Than")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F707")]
    [Order(7)]
    GreaterThan = 7,

    [Name("greaterThanOrEqual")]
    [Description("Greater Than Or Equal")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F708")]
    [Order(8)]
    GreaterThanOrEqual = 8,

    [Name("lessThan")]
    [Description("Less Than")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F709")]
    [Order(9)]
    LessThan = 9,

    [Name("lessThanOrEqual")]
    [Description("Less Than Or Equal")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F710")]
    [Order(10)]
    LessThanOrEqual = 10,

    [Name("between")]
    [Description("Between")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F711")]
    [Order(11)]
    Between = 11,

    [Name("before")]
    [Description("Before")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F712")]
    [Order(12)]
    Before = 12,

    [Name("after")]
    [Description("After")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F713")]
    [Order(13)]
    After = 13,

    [Name("on")]
    [Description("On")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F714")]
    [Order(14)]
    On = 14,

    [Name("onOrBefore")]
    [Description("On Or Before")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F715")]
    [Order(15)]
    OnOrBefore = 15,

    [Name("onOrAfter")]
    [Description("On Or After")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F716")]
    [Order(16)]
    OnOrAfter = 16,

    [Name("isTrue")]
    [Description("Is True")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F717")]
    [Order(17)]
    IsTrue = 17,

    [Name("isFalse")]
    [Description("Is False")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F718")]
    [Order(18)]
    IsFalse = 18,

    [Name("isEmpty")]
    [Description("Is Empty")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F719")]
    [Order(19)]
    IsEmpty = 19,

    [Name("isNotEmpty")]
    [Description("Is Not Empty")]
    [Id("B1A1C2D3-E4F5-4678-91A2-B3C4D5E6F720")]
    [Order(20)]
    IsNotEmpty = 20
}


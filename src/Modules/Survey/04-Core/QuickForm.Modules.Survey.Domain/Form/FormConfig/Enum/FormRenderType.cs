using System.ComponentModel;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;

public enum FormRenderType
{
    [Id("E115B787-1E69-4A93-A23B-C7C5CD9FF588")]
    [Name("Default")]
    [Color("info")]
    [Icon("LayoutTemplate")]
    [Description("A classic stacked layout with all sections shown naturally.")]
    Default,

    [Id("9BBFA25F-7025-488C-8360-27F097590E08")]
    [Name("Tabs")]
    [Color("primary")]
    [Icon("PanelsTopLeft")]
    [Description("Best when users need to jump between sections quickly.")]
    Tabs,

    [Id("221959BF-B21F-44FF-8990-63DAB8094645")]
    [Name("Accordion")]
    [Color("warning")]
    [Icon("Rows3")]
    [Description("Compact and clean for forms with many sections.")]
    Accordion,

    [Id("180D167D-CEB5-4FBA-B0F5-F929396410ED")]
    [Name("Stepper")]
    [Color("success")]
    [Icon("ListOrdered")]
    [Description("Guided flow for longer forms and better completion.")]
    Stepper,
}

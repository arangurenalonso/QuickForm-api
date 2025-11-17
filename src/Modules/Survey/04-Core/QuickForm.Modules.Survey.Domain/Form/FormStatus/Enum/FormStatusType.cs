using System.ComponentModel;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public enum FormStatusType
{
    [Id("031E6828-BA13-4606-9E78-E3A8217068C3")]
    [Name("Draft")]
    [Color("info")]
    [Icon("FilePenLine")]
    Draft,

    [Id("C8C2030D-9A25-4E33-A2D8-394848891C5D")]
    [Name("Published")]
    [Color("success")]
    [Icon("CheckCircle2")]
    Published,

    [Id("2390A665-E129-4F15-9C5F-4D5AC4345683")]
    [Name("Paused")]
    [Color("warning")]
    [Icon("PauseCircle")]
    Paused,

    [Id("A3F5D1B4-1C2B-4E2D-8F1C-3D6F0B8E2B7E")]
    [Name("Resumen")]
    [Color("primary")]
    [Icon("BarChartLine")]
    Resumen,

    [Id("5C558B1C-0C1F-4CB2-A492-D4DFAC742A4A")]
    [Name("Closed")]
    [Color("error")]
    [Icon("XCircle")]
    Closed,
}

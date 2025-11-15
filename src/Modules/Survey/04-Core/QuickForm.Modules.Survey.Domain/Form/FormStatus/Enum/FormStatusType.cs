using System.ComponentModel;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;
public enum FormStatusType
{
    [Id("031E6828-BA13-4606-9E78-E3A8217068C3")]
    [Name("Draft")]
    [StatusColor("info")]
    [StatusIcon("FilePenLine")]
    Draft,

    [Id("C8C2030D-9A25-4E33-A2D8-394848891C5D")]
    [Name("Published")]
    [StatusColor("success")]
    [StatusIcon("CheckCircle2")]
    Published,

    [Id("2390A665-E129-4F15-9C5F-4D5AC4345683")]
    [Name("Paused")]
    [StatusColor("warning")]
    [StatusIcon("PauseCircle")]
    Paused,

    [Id("5C558B1C-0C1F-4CB2-A492-D4DFAC742A4A")]
    [Name("Closed")]
    [StatusColor("error")]
    [StatusIcon("XCircle")]
    Closed,
}

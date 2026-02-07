
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Application;
public class FormViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public StatusViewModel Status { get; set; }
    public int Visits { get; set; }
    public int Submissions { get; set; } 
}

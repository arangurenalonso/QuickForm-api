using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace QuickForm.Modules.Survey.Domain;
public class AttributeValueOccurrence
{
    public Guid IdQuestionType { get; set; }
    public string QuestionType { get; set; } = string.Empty;
    public string AttributeName { get; set; } = string.Empty;
    public JsonElement Value { get; set; }
}

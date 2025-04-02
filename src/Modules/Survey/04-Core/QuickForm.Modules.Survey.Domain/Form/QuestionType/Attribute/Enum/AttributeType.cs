using QuickForm.Common.Domain;
using System.ComponentModel;

namespace QuickForm.Modules.Survey.Domain;

public enum AttributeType
{
    [Detail("Name")]
    [Description("8B1F9394-03B5-4784-826E-4D04A3377AD7")]
    Name = 1,

    [Detail("Label")]
    [Description("1B87884C-93AB-42E3-A189-5DDF0D713723")]
    Label = 2,

    [Detail("HelperText")]
    [Description("4B35F086-438D-4665-A343-91C92D45B5C5")]
    HelperText = 3,

    [Detail("Placeholder")]
    [Description("FFC2DA22-0025-45E4-905A-64B60D80F845")]
    Placeholder = 4,

    [Detail("InformationText")]
    [Description("1E451ACC-0CE7-4FC2-87BF-FD8BDEE7F8CD")]
    InformationText = 5,


    [Detail("Prefix")]
    [Description("E8FD7A8B-985B-4439-A87F-519AA0F6BC41")]
    Prefix = 6,

    [Detail("Suffix")]
    [Description("F5D0F732-3BD5-4264-9999-67B764607A2A")]
    Suffix = 7,

    [Detail("DecimalScale")]
    [Description("BC20FF56-0291-4DEB-872E-DC8DF39781AC")]
    DecimalScale = 8,

    [Detail("AllowNegative")]
    [Description("64AC9058-D95B-451F-B36F-4124C7F2585C")]
    AllowNegative = 9,
}

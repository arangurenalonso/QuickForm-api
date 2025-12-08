using QuickForm.Common.Domain;

namespace QuickForm.Modules.Survey.Domain;

public enum AttributeType
{
    [Name("Name")]
    [Id("8B1F9394-03B5-4784-826E-4D04A3377AD7")]
    Name = 1,

    [Name("Label")]
    [Id("1B87884C-93AB-42E3-A189-5DDF0D713723")]
    Label = 2,

    [Name("HelperText")]
    [Id("4B35F086-438D-4665-A343-91C92D45B5C5")]
    HelperText = 3,

    [Name("Placeholder")]
    [Id("FFC2DA22-0025-45E4-905A-64B60D80F845")]
    Placeholder = 4,

    [Name("InformationText")]
    [Id("1E451ACC-0CE7-4FC2-87BF-FD8BDEE7F8CD")]
    InformationText = 5,


    [Name("Prefix")]
    [Id("E8FD7A8B-985B-4439-A87F-519AA0F6BC41")]
    Prefix = 6,

    [Name("Suffix")]
    [Id("F5D0F732-3BD5-4264-9999-67B764607A2A")]
    Suffix = 7,

    [Name("DecimalScale")]
    [Id("BC20FF56-0291-4DEB-872E-DC8DF39781AC")]
    DecimalScale = 8,

    [Name("AllowNegative")]
    [Id("64AC9058-D95B-451F-B36F-4124C7F2585C")]
    AllowNegative = 9,
}

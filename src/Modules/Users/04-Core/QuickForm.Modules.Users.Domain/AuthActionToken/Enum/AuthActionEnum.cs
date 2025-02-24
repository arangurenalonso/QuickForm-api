using System.ComponentModel;
using QuickForm.Common.Domain;

namespace QuickForm.Modules.Users.Domain.AuthActionToken.Enum;
public enum AuthActionType
{
    [Detail("RecoveryPassword")]
    [Description("E71B80A3-5C62-4FF4-B2A1-9E523F7A7864")]
    RecoveryPassword,

    [Detail("EmailConfirmation")]
    [Description("C14EFC94-7FDC-420F-96F7-373E3276F0A3")]
    EmailConfirmation,
}

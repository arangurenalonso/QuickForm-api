namespace QuickForm.Modules.Users.Application;
public sealed record AuthSessionResponse(
    string AccessToken,
    string RefreshToken,
    UserResponse User
);

namespace QuickForm.Modules.Users.Application;

public sealed record UserResponse(
        Guid Id, 
        string Email
    );

﻿namespace QuickForm.Modules.Users.Domain;

public interface IAuthActionTokenRepository
{
    Task<AuthActionTokenDomain?> GetAuthActionTokenByAuthActionIdAndTokenAsync(
        AuthActionId authActionId,
        TokenVO userActionToken);
    void Insert(AuthActionTokenDomain authActionTokenDomain);
    void Update(AuthActionTokenDomain authActionTokenDomain);
}

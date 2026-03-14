namespace QuickForm.Common.Application;
public interface ICommonOptionsProvider
{
    Uri GetCurrentApplicationUrl();
    Uri GetFrontEndApplicationUrl();
    string GetSecretKey();
}

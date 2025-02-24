namespace QuickForm.Common.Application;
public interface ICommonOptionsProvider
{
    public Uri GetCurrentApplicationUrl();
    public Uri GetFrontEndApplicationUrl();
}

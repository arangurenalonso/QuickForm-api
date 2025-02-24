using QuickForm.Common.Application;

namespace QuickForm.Common.Infrastructure;
internal sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}

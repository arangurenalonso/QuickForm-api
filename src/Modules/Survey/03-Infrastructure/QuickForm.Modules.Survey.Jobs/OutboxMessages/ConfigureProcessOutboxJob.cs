using Microsoft.Extensions.Options;
using Quartz;
using QuickForm.Modules.Survey.Options;

namespace QuickForm.Modules.Survey.Jobs;
internal sealed class ConfigureProcessOutboxJob : IConfigureOptions<QuartzOptions>
{
    private readonly OutboxOptions _outboxOptions;

    public ConfigureProcessOutboxJob(IOptions<OutboxOptions> outboxOptions)
    {
        _outboxOptions = outboxOptions.Value;
    }
     
    public void Configure(QuartzOptions options)
    {
        string jobName = typeof(ProcessOutboxJob).FullName!;

        options
            .AddJob<ProcessOutboxJob>(configure => configure.WithIdentity(jobName))
            .AddTrigger(configure =>
                configure
                    .ForJob(jobName)
                    .WithSimpleSchedule(schedule =>
                        schedule.WithIntervalInSeconds(_outboxOptions.IntervalInSeconds).RepeatForever()));
    }
}

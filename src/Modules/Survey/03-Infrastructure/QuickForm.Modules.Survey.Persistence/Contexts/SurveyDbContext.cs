using System.Reflection;
using Microsoft.EntityFrameworkCore;
using QuickForm.Common.Infrastructure;
using QuickForm.Modules.Survey.Application;
using QuickForm.Modules.Survey.Domain.Customers;
using QuickForm.Modules.Survey.Domain.Form;

namespace QuickForm.Modules.Survey.Persistence;
public sealed class SurveyDbContext(DbContextOptions<SurveyDbContext> options) : DbContext(options), IUnitOfWork
{
    public required DbSet<Customer> Customers { get; set; }
    public required DbSet<FormDomain> Form { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Survey);

        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}

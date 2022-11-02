using JsonColumns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using static System.Console;

using (var dbContext = new EventContext())
{
    await EventContext.InitializeAsync(dbContext);
}

using (var dbContext = new EventContext())
{
    dbContext.Log = WriteLine;

    dbContext.EventInboxes.Add(
        new EventAInbox
        {
            Event = new EventA
            {
                Name = "Maddy Montaquila",
                ProcessEarliest = DateTimeOffset.Now.AddMinutes(5),
            }
        }
    );

    dbContext.EventInboxes.Add(
        new EventBInbox
        {
            Event = new EventB { Name = "Jeremy Likness", Age = 43, }
        }
    );

    await dbContext.SaveChangesAsync();
}

using (var dbContext = new EventContext())
{
    dbContext.Log = WriteLine;

    var author = await dbContext.EventAs.SingleAsync(x => x.Event.Name == "Maddy Montaquila");

    WriteLine(author);
}

namespace JsonColumns
{
    public class EventA
    {
        public string Name { get; set; } = null!;
        public DateTimeOffset? ProcessEarliest { get; set; }
    }

    public class EventB
    {
        public string Name { get; set; } = null!;
        public int Age { get; set; }
    }

    public class EventContext : DbContext
    {
        public Action<string>? Log { get; set; }
        public LogLevel LogLevel { get; set; } = LogLevel.Information;

        public DbSet<EventInbox> EventInboxes { get; set; }
        public DbSet<EventAInbox> EventAs { get; set; }
        public DbSet<EventBInbox> EventBs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) =>
            optionsBuilder
                .UseSqlServer(
                    @"Server=(localdb)\mssqllocaldb;Database=JsonColumns;Trusted_Connection=True;MultipleActiveResultSets=true"
                )
                .LogTo(s => Log?.Invoke(s), LogLevel)
                .ConfigureWarnings(
                    config =>
                        config.Ignore(RelationalEventId.ForeignKeyPropertiesMappedToUnrelatedTables)
                );

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(EventContext).Assembly);
        }

        public static async Task InitializeAsync(EventContext context)
        {
            await context.Database.EnsureDeletedAsync();

            await context.Database.MigrateAsync();
        }
    }

    public sealed class EventAInbox : EventInbox
    {
        public EventA Event { get; set; } = null!;

        internal sealed class Configuration : IEntityTypeConfiguration<EventAInbox>
        {
            public void Configure(EntityTypeBuilder<EventAInbox> builder)
            {
                builder.OwnsOne(
                    e => e.Event,
                    ownedNavigationBuilder => ownedNavigationBuilder.ToJson()
                );
            }
        }
    }

    public sealed class EventBInbox : EventInbox
    {
        public EventB Event { get; set; } = null!;

        internal sealed class Configuration : IEntityTypeConfiguration<EventBInbox>
        {
            public void Configure(EntityTypeBuilder<EventBInbox> builder)
            {
                builder.OwnsOne(
                    e => e.Event,
                    ownedNavigationBuilder => ownedNavigationBuilder.ToJson()
                );
            }
        }
    }

    public abstract class EventInbox
    {
        public int Id { get; set; }

        internal sealed class BaseConfiguration : IEntityTypeConfiguration<EventInbox>
        {
            public void Configure(EntityTypeBuilder<EventInbox> builder)
            {
                builder.ToTable("EventInbox");
            }
        }
    }
}

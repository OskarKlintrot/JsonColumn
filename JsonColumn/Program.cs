using JsonColumns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using static System.Console;

using (var dbContext = new AuthorContext())
{
    await AuthorContext.InitializeAsync(dbContext);
}

using (var dbContext = new AuthorContext())
{
    dbContext.Log = WriteLine;

    dbContext.Authors.Add(
        new Author
        {
            Name = "Maddy Montaquila",
            Contact = new ContactDetails
            {
                Phone = "01632 12345",
                Address = new Address(
                    street: "1 Main St",
                    city: "Camberwick Green",
                    postcode: "CW1 5ZH",
                    country: "UK"
                )
            }
        }
    );

    await dbContext.SaveChangesAsync();
}

using (var dbContext = new AuthorContext())
{
    dbContext.Log = WriteLine;

    var author = await dbContext.Authors.SingleAsync(x => x.Name == "Maddy Montaquila");

    WriteLine(author);
}

namespace JsonColumns
{
    public class AuthorContext : DbContext
    {
        public Action<string>? Log { get; set; }
        public LogLevel LogLevel { get; set; } = LogLevel.Information;

        public DbSet<Author> Authors { get; set; }

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
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthorContext).Assembly);
        }

        public static async Task InitializeAsync(AuthorContext context)
        {
            await context.Database.EnsureDeletedAsync();

            await context.Database.MigrateAsync();
        }
    }

    public class ContactDetails
    {
        public Address Address { get; set; } = null!;
        public string? Phone { get; set; }
    }

    public class Address
    {
        public Address(string street, string city, string postcode, string country)
        {
            Street = street;
            City = city;
            Postcode = postcode;
            Country = country;
        }

        public string Street { get; set; }
        public string City { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
    }

    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public ContactDetails Contact { get; set; } = null!;

        internal sealed class Configuration : IEntityTypeConfiguration<Author>
        {
            public void Configure(EntityTypeBuilder<Author> builder)
            {
                builder.OwnsOne(
                    author => author.Contact,
                    ownedNavigationBuilder =>
                    {
                        ownedNavigationBuilder.ToJson();
                        ownedNavigationBuilder.OwnsOne(contactDetails => contactDetails.Address);
                    }
                );
            }
        }
    }
}

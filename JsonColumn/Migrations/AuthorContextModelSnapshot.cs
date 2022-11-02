﻿// <auto-generated />
using JsonColumns;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace JsonColumn.Migrations
{
    [DbContext(typeof(AuthorContext))]
    partial class AuthorContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc.2.22472.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("JsonColumns.Author", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Authors");
                });

            modelBuilder.Entity("JsonColumns.Author", b =>
                {
                    b.OwnsOne("JsonColumns.ContactDetails", "Contact", b1 =>
                        {
                            b1.Property<int>("AuthorId")
                                .HasColumnType("int");

                            b1.Property<string>("Phone")
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("AuthorId");

                            b1.ToTable("Authors");

                            b1.ToJson("Contact");

                            b1.WithOwner()
                                .HasForeignKey("AuthorId");

                            b1.OwnsOne("JsonColumns.Address", "Address", b2 =>
                                {
                                    b2.Property<int>("ContactDetailsAuthorId")
                                        .HasColumnType("int");

                                    b2.Property<string>("City")
                                        .IsRequired()
                                        .HasColumnType("nvarchar(max)");

                                    b2.Property<string>("Country")
                                        .IsRequired()
                                        .HasColumnType("nvarchar(max)");

                                    b2.Property<string>("Postcode")
                                        .IsRequired()
                                        .HasColumnType("nvarchar(max)");

                                    b2.Property<string>("Street")
                                        .IsRequired()
                                        .HasColumnType("nvarchar(max)");

                                    b2.HasKey("ContactDetailsAuthorId");

                                    b2.ToTable("Authors");

                                    b2.WithOwner()
                                        .HasForeignKey("ContactDetailsAuthorId");
                                });

                            b1.Navigation("Address")
                                .IsRequired();
                        });

                    b.Navigation("Contact")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}

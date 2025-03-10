﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using QuickForm.Modules.Users.Persistence;

#nullable disable

namespace QuickForm.Modules.Users.Persistence.Migrations
{
    [DbContext(typeof(UsersDbContext))]
    [Migration("20250310021344_TrackingFieldInboxMessage")]
    partial class TrackingFieldInboxMessage
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("Auth")
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("QuickForm.Common.Domain.AuditLog", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ChangesValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClassOrigin")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("CurrentValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("IdEntity")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("OriginalValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TableName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("TransactionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int?>("Type")
                        .HasColumnType("int");

                    b.Property<string>("TypeName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserTransaction")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Audit", "Auth");
                });

            modelBuilder.Entity("QuickForm.Common.Infrastructure.InboxMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Error")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("IdDomainEvent")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("OccurredOnUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ProcessedOnUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("inbox_messages", "Auth");
                });

            modelBuilder.Entity("QuickForm.Common.Infrastructure.InboxMessageConsumer", b =>
                {
                    b.Property<Guid>("InboxMessageId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("InboxMessageId", "Name");

                    b.ToTable("inbox_message_consumers", "Auth");
                });

            modelBuilder.Entity("QuickForm.Common.Infrastructure.OutboxMessage", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ClassOrigin")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Error")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("OccurredOnUtc")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("ProcessedOnUtc")
                        .HasColumnType("datetime2");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<Guid>("TransactionId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("outbox_messages", "Auth");
                });

            modelBuilder.Entity("QuickForm.Common.Infrastructure.OutboxMessageConsumer", b =>
                {
                    b.Property<Guid>("OutboxMessageId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)");

                    b.HasKey("OutboxMessageId", "Name");

                    b.ToTable("outbox_message_consumers", "Auth");
                });

            modelBuilder.Entity("QuickForm.Modules.Users.Domain.AuthActionDomain", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("AuthActions", "Auth");
                });

            modelBuilder.Entity("QuickForm.Modules.Users.Domain.AuthActionTokenDomain", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("datetime2")
                        .HasColumnName("ExpiresAt");

                    b.Property<Guid>("IdUser")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("IdUserAction")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("ModifiedBy")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("Token");

                    b.Property<bool>("Used")
                        .HasColumnType("bit")
                        .HasColumnName("Used");

                    b.HasKey("Id");

                    b.HasIndex("IdUser");

                    b.HasIndex("IdUserAction");

                    b.ToTable("AuthActionTokens", "Auth");
                });

            modelBuilder.Entity("QuickForm.Modules.Users.Domain.UserDomain", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("Email");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(true);

                    b.Property<bool>("IsEmailVerify")
                        .HasColumnType("bit");

                    b.Property<bool>("IsPasswordChanged")
                        .HasColumnType("bit");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("LastName");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)");

                    b.Property<DateTime?>("ModifiedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Name");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("PasswordHash");

                    b.HasKey("Id");

                    b.ToTable("Users", "Auth");
                });

            modelBuilder.Entity("QuickForm.Modules.Users.Domain.AuthActionDomain", b =>
                {
                    b.OwnsOne("QuickForm.Modules.Users.Domain.ActionDescriptionVO", "Description", b1 =>
                        {
                            b1.Property<Guid>("AuthActionDomainId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(255)
                                .HasColumnType("nvarchar(255)")
                                .HasColumnName("Description");

                            b1.HasKey("AuthActionDomainId");

                            b1.ToTable("AuthActions", "Auth");

                            b1.WithOwner()
                                .HasForeignKey("AuthActionDomainId");
                        });

                    b.Navigation("Description")
                        .IsRequired();
                });

            modelBuilder.Entity("QuickForm.Modules.Users.Domain.AuthActionTokenDomain", b =>
                {
                    b.HasOne("QuickForm.Modules.Users.Domain.UserDomain", "User")
                        .WithMany("AuthActionTokens")
                        .HasForeignKey("IdUser")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("QuickForm.Modules.Users.Domain.AuthActionDomain", "Action")
                        .WithMany("UserActionTokens")
                        .HasForeignKey("IdUserAction")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Action");

                    b.Navigation("User");
                });

            modelBuilder.Entity("QuickForm.Modules.Users.Domain.AuthActionDomain", b =>
                {
                    b.Navigation("UserActionTokens");
                });

            modelBuilder.Entity("QuickForm.Modules.Users.Domain.UserDomain", b =>
                {
                    b.Navigation("AuthActionTokens");
                });
#pragma warning restore 612, 618
        }
    }
}

﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Server.Data;

#nullable disable

namespace Server.Data.Migrations
{
    [DbContext(typeof(AppDBContext))]
    partial class AppDBContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.9");

            modelBuilder.Entity("Shared.Models.Category", b =>
                {
                    b.Property<int>("CategoryId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasMaxLength(1024)
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<string>("ThumbnailImagePath")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.HasKey("CategoryId");

                    b.ToTable("Categories");

                    b.HasData(
                        new
                        {
                            CategoryId = 1,
                            Description = "A description of category 1",
                            Name = "Category 1",
                            ThumbnailImagePath = "uploads/placeholder.jpg"
                        },
                        new
                        {
                            CategoryId = 2,
                            Description = "A description of category 2",
                            Name = "Category 2",
                            ThumbnailImagePath = "uploads/placeholder.jpg"
                        },
                        new
                        {
                            CategoryId = 3,
                            Description = "A description of category 3",
                            Name = "Category 3",
                            ThumbnailImagePath = "uploads/placeholder.jpg"
                        });
                });

            modelBuilder.Entity("Shared.Models.Post", b =>
                {
                    b.Property<int>("PostId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<int>("CategoryId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasMaxLength(65536)
                        .HasColumnType("TEXT");

                    b.Property<string>("Excerpt")
                        .IsRequired()
                        .HasMaxLength(512)
                        .HasColumnType("TEXT");

                    b.Property<string>("PublishDate")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("TEXT");

                    b.Property<bool>("Published")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ThumbnailImagePath")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.HasKey("PostId");

                    b.HasIndex("CategoryId");

                    b.ToTable("Posts");

                    b.HasData(
                        new
                        {
                            PostId = 1,
                            Author = "Jimmy Townsend",
                            CategoryId = 1,
                            Content = "",
                            Excerpt = "This is the excerpt for post 1. An excerpt is a little extraction from a larger piece of text. Sort of like a preview.",
                            PublishDate = "10/10/2022 04:06",
                            Published = true,
                            ThumbnailImagePath = "uploads/placeholder.jpg",
                            Title = "First post"
                        },
                        new
                        {
                            PostId = 2,
                            Author = "Jimmy Townsend",
                            CategoryId = 2,
                            Content = "",
                            Excerpt = "This is the excerpt for post 2. An excerpt is a little extraction from a larger piece of text. Sort of like a preview.",
                            PublishDate = "10/10/2022 04:06",
                            Published = true,
                            ThumbnailImagePath = "uploads/placeholder.jpg",
                            Title = "Second post"
                        },
                        new
                        {
                            PostId = 3,
                            Author = "Jimmy Townsend",
                            CategoryId = 3,
                            Content = "",
                            Excerpt = "This is the excerpt for post 3. An excerpt is a little extraction from a larger piece of text. Sort of like a preview.",
                            PublishDate = "10/10/2022 04:06",
                            Published = true,
                            ThumbnailImagePath = "uploads/placeholder.jpg",
                            Title = "Third post"
                        },
                        new
                        {
                            PostId = 4,
                            Author = "Jimmy Townsend",
                            CategoryId = 1,
                            Content = "",
                            Excerpt = "This is the excerpt for post 4. An excerpt is a little extraction from a larger piece of text. Sort of like a preview.",
                            PublishDate = "10/10/2022 04:06",
                            Published = true,
                            ThumbnailImagePath = "uploads/placeholder.jpg",
                            Title = "Fourth post"
                        },
                        new
                        {
                            PostId = 5,
                            Author = "Jimmy Townsend",
                            CategoryId = 2,
                            Content = "",
                            Excerpt = "This is the excerpt for post 5. An excerpt is a little extraction from a larger piece of text. Sort of like a preview.",
                            PublishDate = "10/10/2022 04:06",
                            Published = true,
                            ThumbnailImagePath = "uploads/placeholder.jpg",
                            Title = "Fifth post"
                        },
                        new
                        {
                            PostId = 6,
                            Author = "Jimmy Townsend",
                            CategoryId = 3,
                            Content = "",
                            Excerpt = "This is the excerpt for post 6. An excerpt is a little extraction from a larger piece of text. Sort of like a preview.",
                            PublishDate = "10/10/2022 04:06",
                            Published = true,
                            ThumbnailImagePath = "uploads/placeholder.jpg",
                            Title = "Sixth post"
                        });
                });

            modelBuilder.Entity("Shared.Models.Post", b =>
                {
                    b.HasOne("Shared.Models.Category", "Category")
                        .WithMany("Posts")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Shared.Models.Category", b =>
                {
                    b.Navigation("Posts");
                });
#pragma warning restore 612, 618
        }
    }
}

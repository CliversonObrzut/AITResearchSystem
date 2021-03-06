﻿// <auto-generated />
using AITResearchSystem.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using System;

namespace AITResearchSystem.Migrations
{
    [DbContext(typeof(AitResearchDbContext))]
    [Migration("20180506041942_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.2-rtm-10011")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AITResearchSystem.Data.Models.Answer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("OptionId");

                    b.Property<int>("QuestionId");

                    b.Property<int>("RespondentId");

                    b.Property<string>("Text")
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("OptionId");

                    b.HasIndex("QuestionId");

                    b.HasIndex("RespondentId");

                    b.ToTable("Answer");
                });

            modelBuilder.Entity("AITResearchSystem.Data.Models.Option", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("NextQuestion");

                    b.Property<int>("QuestionId");

                    b.Property<string>("Text")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("QuestionId");

                    b.ToTable("QuestionOption");
                });

            modelBuilder.Entity("AITResearchSystem.Data.Models.Question", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("Order");

                    b.Property<int>("QuestionTypeId");

                    b.Property<string>("Text")
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("QuestionTypeId");

                    b.ToTable("Question");
                });

            modelBuilder.Entity("AITResearchSystem.Data.Models.QuestionType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Type")
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.ToTable("QuestionType");
                });

            modelBuilder.Entity("AITResearchSystem.Data.Models.Respondent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Date")
                        .HasColumnType("date");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("date");

                    b.Property<string>("GivenNames")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<string>("IpAddress")
                        .IsRequired()
                        .HasColumnType("varchar(15)");

                    b.Property<string>("LastName")
                        .HasColumnType("varchar(50)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("varchar(15)");

                    b.HasKey("Id");

                    b.ToTable("Respondent");
                });

            modelBuilder.Entity("AITResearchSystem.Data.Models.Staff", b =>
                {
                    b.Property<string>("Email")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.HasKey("Email");

                    b.ToTable("Staff");
                });

            modelBuilder.Entity("AITResearchSystem.Data.Models.Answer", b =>
                {
                    b.HasOne("AITResearchSystem.Data.Models.Option", "Option")
                        .WithMany("OptionAnswers")
                        .HasForeignKey("OptionId");

                    b.HasOne("AITResearchSystem.Data.Models.Question", "Question")
                        .WithMany("QuestionAnswers")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AITResearchSystem.Data.Models.Respondent", "Respondent")
                        .WithMany("RespondentAnswers")
                        .HasForeignKey("RespondentId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AITResearchSystem.Data.Models.Option", b =>
                {
                    b.HasOne("AITResearchSystem.Data.Models.Question", "Question")
                        .WithMany("QuestionOptions")
                        .HasForeignKey("QuestionId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AITResearchSystem.Data.Models.Question", b =>
                {
                    b.HasOne("AITResearchSystem.Data.Models.QuestionType", "QuestionType")
                        .WithMany("QuestionTypeQuestions")
                        .HasForeignKey("QuestionTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}

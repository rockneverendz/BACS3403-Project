using System;
using System.Collections.Generic;
using System.Text;
using BACS3403_Project.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BACS3403_Project.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<BACS3403_Project.Models.Answer> Answers { get; set; }
        public DbSet<BACS3403_Project.Models.RecordingList> RecordingLists { get; set; }
        public DbSet<BACS3403_Project.Models.MarkScheme> MarkSchemes { get; set; }
        public DbSet<BACS3403_Project.Models.QuestionGroup> Questions { get; set; }
        public DbSet<BACS3403_Project.Models.Recording> Recordings { get; set; }
        public DbSet<BACS3403_Project.Models.Seat> Seats { get; set; }
        public DbSet<BACS3403_Project.Models.Test> Tests { get; set; }
        public DbSet<BACS3403_Project.Models.Candidate> Candidates { get; set; }
        public DbSet<BACS3403_Project.Models.Examiner> Examiners { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Call back the overridden function
            modelBuilder.Entity<Answer>().ToTable("Answer");
            modelBuilder.Entity<RecordingList>().ToTable("RecordingList");
            modelBuilder.Entity<MarkScheme>().ToTable("MarkScheme");
            modelBuilder.Entity<QuestionGroup>().ToTable("QuestionGroup");
            modelBuilder.Entity<Recording>().ToTable("Recording");
            modelBuilder.Entity<Seat>().ToTable("Seat");
            modelBuilder.Entity<Test>().ToTable("Test");
            modelBuilder.Entity<Candidate>().ToTable("Candidate");
            modelBuilder.Entity<Examiner>().ToTable("Examiner");

            modelBuilder.Entity<RecordingList>()
                .HasKey(rl => new { rl.CandidateID, rl.RecordingID });

            modelBuilder.Entity<Answer>()
                .HasOne(a => a.RecordingList)
                .WithMany(rl => rl.Answers)
                .HasForeignKey(a => new { a.CandidateID, a.RecordingID });
        }
    }
}

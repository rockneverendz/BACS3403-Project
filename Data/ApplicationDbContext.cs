using System;
using System.Collections.Generic;
using System.Text;
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
        public DbSet<BACS3403_Project.Models.Examiner> Examiners { get; set; }
        public DbSet<BACS3403_Project.Models.Recording> Recordings { get; set; }
        public DbSet<BACS3403_Project.Models.QuestionGroup> Questions { get; set; }
        public DbSet<BACS3403_Project.Models.Question> Answers { get; set; }
    }
}

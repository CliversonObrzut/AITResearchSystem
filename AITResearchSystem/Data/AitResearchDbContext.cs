using AITResearchSystem.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AITResearchSystem.Data
{
    public class AitResearchDbContext : DbContext
    {
        public AitResearchDbContext(DbContextOptions options) : base(options)
        {
            
        }

        public DbSet<Answer> Answers { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<QuestionType> QuestionTypes { get; set; }
        public DbSet<Respondent> Respondents { get; set; }
        public DbSet<Staff> Staves { get; set; }
    }
}

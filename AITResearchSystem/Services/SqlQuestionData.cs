using System.Collections.Generic;
using System.Linq;
using AITResearchSystem.Data;
using AITResearchSystem.Data.Models;
using AITResearchSystem.Services.Interfaces;

namespace AITResearchSystem.Services
{
    public class SqlQuestionData : IQuestion
    {
        private readonly AitResearchDbContext _context;

        public SqlQuestionData(AitResearchDbContext context)
        {
            _context = context;
        }

        public Question Get(int id)
        {
            return _context.Questions.FirstOrDefault(answer => answer.Id == id);
        }

        public IEnumerable<Question> GetAll()
        {
            return _context.Questions.ToList();
        }
    }
}

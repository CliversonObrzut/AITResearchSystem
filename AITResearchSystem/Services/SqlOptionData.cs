using System.Collections.Generic;
using System.Linq;
using AITResearchSystem.Data;
using AITResearchSystem.Data.Models;
using AITResearchSystem.Services.Interfaces;

namespace AITResearchSystem.Services
{
    public class SqlOptionData : IOption
    {
        private readonly AitResearchDbContext _context;

        public SqlOptionData(AitResearchDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Option> GetByQuestion(int questionId)
        {
            return _context.Options.Where(option => option.QuestionId == questionId);
        }

        public IEnumerable<Option> GetAll()
        {
            return _context.Options.ToList();
        }
    }
}

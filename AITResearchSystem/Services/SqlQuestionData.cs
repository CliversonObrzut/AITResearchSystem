using System.Collections.Generic;
using System.Linq;
using AITResearchSystem.Data;
using AITResearchSystem.Data.Models;
using AITResearchSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AITResearchSystem.Services
{
    public class SqlQuestionData : IQuestion
    {
        private readonly AitResearchDbContext _context;

        public SqlQuestionData(AitResearchDbContext context)
        {
            _context = context;
        }

        public Question GetById(int id)
        {
            return _context.Questions
                .Include(qt => qt.QuestionType)
                .Include(qo => qo.QuestionOptions)
                .FirstOrDefault(question => question.Id == id);
        }

        public IEnumerable<Question> GetAll()
        {
            return _context.Questions.ToList();
        }

        public Question GetByOrder(int order)
        {
            return _context.Questions
                .Include(qt => qt.QuestionType)
                .Include(qo => qo.QuestionOptions)
                .FirstOrDefault(question => question.Order == order);
        }

        public bool IsNextQuestionAvailable(int order)
        {
            return _context.Questions.Any(q => q.Order == order);
        }
    }
}

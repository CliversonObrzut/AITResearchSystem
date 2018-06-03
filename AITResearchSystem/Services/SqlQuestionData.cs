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

        /// <summary>
        /// Returns from db a Question by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Question GetById(int id)
        {
            return _context.Questions
                .Include(qt => qt.QuestionType)
                .Include(qo => qo.QuestionOptions)
                .FirstOrDefault(question => question.Id == id);
        }

        /// <summary>
        /// Returns from db all the Questions.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Question> GetAll()
        {
            return _context.Questions.ToList();
        }

        /// <summary>
        /// Returns from db a Question by order.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public Question GetByOrder(int order)
        {
            return _context.Questions
                .Include(qt => qt.QuestionType)
                .Include(qo => qo.QuestionOptions)
                .FirstOrDefault(question => question.Order == order);
        }

        /// <summary>
        /// Returns if the next question order is available inside database.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool IsNextQuestionAvailable(int order)
        {
            return _context.Questions.Any(q => q.Order == order);
        }
    }
}

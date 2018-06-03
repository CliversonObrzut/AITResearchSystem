using System;
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

        /// <summary>
        /// Returns from db a Option by question id.
        /// </summary>
        /// <param name="questionId"></param>
        /// <returns></returns>
        public IEnumerable<Option> GetByQuestion(int questionId)
        {
            try
            {
                return _context.Options.Where(option => option.QuestionId == questionId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        /// <summary>
        /// Returns from db all the Options.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Option> GetAll()
        {
            try
            {
                return _context.Options.ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}

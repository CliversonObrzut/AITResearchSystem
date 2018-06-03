using System.Collections.Generic;
using System.Linq;
using AITResearchSystem.Data;
using AITResearchSystem.Data.Models;
using AITResearchSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AITResearchSystem.Services
{
    public class SqlRespondentData : IRespondent
    {
        private readonly AitResearchDbContext _context;

        public SqlRespondentData(AitResearchDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns from db all Respondents.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Respondent> GetAll()
        {
            return _context.Respondents
                .Include(a => a.RespondentAnswers).ToList();
        }

        /// <summary>
        /// Returns from db a Respondent by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Respondent Get(int id)
        {
            return _context.Respondents
                .Include(an => an.RespondentAnswers)
                .FirstOrDefault(user => user.Id == id);
        }

        /// <summary>
        /// Adds a Respondent to database.
        /// </summary>
        /// <param name="respondent"></param>
        public void Add(Respondent respondent)
        {
            _context.Respondents.Add(respondent);
            _context.SaveChanges();
        }
    }
}

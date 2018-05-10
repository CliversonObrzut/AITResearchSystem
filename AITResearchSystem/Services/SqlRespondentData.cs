using System.Collections.Generic;
using System.Linq;
using AITResearchSystem.Data;
using AITResearchSystem.Data.Models;
using AITResearchSystem.Services.Interfaces;

namespace AITResearchSystem.Services
{
    public class SqlRespondentData : IRespondent
    {
        private readonly AitResearchDbContext _context;

        public SqlRespondentData(AitResearchDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Respondent> GetAll()
        {
            return _context.Respondents.ToList();
        }

        public Respondent Get(int id)
        {
            return _context.Respondents.FirstOrDefault(user => user.Id == id);
        }

        public void Add(Respondent respondent)
        {
            _context.Respondents.Add(respondent);
            _context.SaveChanges();
        }
    }
}

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

        public Option Get(int id)
        {
            return _context.Options.FirstOrDefault(option => option.Id == id);
        }

        public IEnumerable<Option> GetAll()
        {
            return _context.Options.ToList();
        }
    }
}

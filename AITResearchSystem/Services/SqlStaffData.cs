using System.Linq;
using AITResearchSystem.Data;
using AITResearchSystem.Data.Models;
using AITResearchSystem.Services.Interfaces;

namespace AITResearchSystem.Services
{
    public class SqlStaffData : IStaff
    {
        private readonly AitResearchDbContext _context;

        public SqlStaffData(AitResearchDbContext context)
        {
            _context = context;
        }

        public Staff GetByEmailAndPassword(string email, string password)
        {
            return _context.Staves.FirstOrDefault(user => user.Email == email && user.Password == password);
        }
    }
}

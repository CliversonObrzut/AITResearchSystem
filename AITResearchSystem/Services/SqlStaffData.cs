using System;
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

        /// <summary>
        /// Returns from db a Staff by email and password.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Staff GetByEmailAndPassword(string email, string password)
        {
            try
            {
                return _context.Staves.FirstOrDefault(user => user.Email == email && user.Password == password);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}

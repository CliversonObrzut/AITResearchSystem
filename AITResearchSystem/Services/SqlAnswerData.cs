using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AITResearchSystem.Data;
using AITResearchSystem.Data.Models;
using AITResearchSystem.Services.Interfaces;

namespace AITResearchSystem.Services
{
    public class SqlAnswerData : IAnswer
    {
        private readonly AitResearchDbContext _context;

        public SqlAnswerData(AitResearchDbContext context)
        {
            _context = context;
        }
        public void Add(Answer answer)
        {
            try
            {
                _context.Answers.Add(answer);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                //TODO Implement add answer exception
            }
        }

        public IEnumerable<Answer> FilterByAgeRange(string ageRange)
        {
            return _context.Answers.Where(answer => answer.Option.Text == ageRange)
                .OrderBy(answer => answer.Respondent.LastName);
        }

        public IEnumerable<Answer> FilterByGenre(string genre)
        {
            return _context.Answers.Where(answer => answer.Option.Text == genre)
                .OrderBy(answer => answer.Respondent.LastName);
        }

        public IEnumerable<Answer> FilterByState(string state)
        {
            return _context.Answers.Where(answer => answer.Option.Text == state)
                .OrderBy(answer => answer.Respondent.LastName);
        }

        public Answer Get(int id)
        {
            return _context.Answers.FirstOrDefault(answer => answer.Id == id);
        }

        public IEnumerable GetAll()
        {
            return _context.Answers.ToList().OrderBy(answer => answer.Respondent.LastName);
        }

        public IEnumerable<Answer> Search(string query)
        {
            // TODO implement Search Function
            return null;
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AITResearchSystem.Data;
using AITResearchSystem.Data.Models;
using AITResearchSystem.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

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

        public void AddRange(List<Answer> answers)
        {
            try
            {
                _context.Answers.AddRange(answers);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                //TODO Implement add answer exception
            }
        }

        public IEnumerable<Answer> FilterByOptionId(int optionId)
        {
            return _context.Answers
                .Include(o => o.Option)
                .Where(answer => answer.Option.Id == optionId);
        }

        public IEnumerable<Answer> Search(string text)
        {
            return _context.Answers
                .Include(r => r.Respondent)
                .Include(o => o.Option)
                .Where(a => a.Text.Contains(text)
                         || a.Option.Text.Contains(text)
                         || a.Respondent.LastName.Contains(text)
                         || a.Respondent.GivenNames.Contains(text)
                         || a.Respondent.PhoneNumber.Contains(text));
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

        //public IEnumerable<Answer> Search(string query)
        //{
        //    // TODO implement Search Function
        //    return null;
        //}
    }
}

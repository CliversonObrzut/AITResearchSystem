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

        /// <summary>
        /// Add an Answer to database
        /// </summary>
        /// <param name="answer"></param>
        public void Add(Answer answer)
        {
            try
            {
                _context.Answers.Add(answer);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Adds a list of Answers to database.
        /// </summary>
        /// <param name="answers"></param>
        public void AddRange(List<Answer> answers)
        {
            try
            {
                _context.Answers.AddRange(answers);
                _context.SaveChanges();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Returns a list of answers from database filtered by quesiton option id.
        /// </summary>
        /// <param name="optionId"></param>
        /// <returns></returns>
        public IEnumerable<Answer> FilterByOptionId(int optionId)
        {
            try
            {
                return _context.Answers
                    .Include(o => o.Option)
                    .Where(answer => answer.Option.Id == optionId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        /// <summary>
        /// Returns a list of answers that contains the text passed.
        /// Includes verification in answer, respondent and option database tables.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public IEnumerable<Answer> Search(string text)
        {
            try
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        /// <summary>
        /// Return an answer by id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Answer Get(int id)
        {
            try
            {
                return _context.Answers.FirstOrDefault(answer => answer.Id == id);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        /// <summary>
        /// Return all answers from database.
        /// </summary>
        /// <returns></returns>
        public IEnumerable GetAll()
        {
            try
            {
                return _context.Answers.ToList().OrderBy(answer => answer.Respondent.LastName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}

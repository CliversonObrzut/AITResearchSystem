using AITResearchSystem.Data.Models;
using System.Collections.Generic;
using AITResearchSystem.ViewModels;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace AITResearchSystem.Services
{
    public class SessionService
    {
        private const string SessionKeyRespondent = "_respondent";
        private const string SessionKeyAnswers = "_answers";
        private const string SessionKeyFollowUpQuestions = "_followUpQuestions";
        private const string SessionKeySessionQuestions = "_sessionQuestions";

        private readonly IHttpContextAccessor _accessor;

        public SessionService(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public void SetRespondent(Respondent resp)
        {
            _accessor.HttpContext.Session.SetString(SessionKeyRespondent, JsonConvert.SerializeObject(resp));
        }

        public Respondent GetRespondent()
        {
            var value = _accessor.HttpContext.Session.GetString(SessionKeyRespondent);
            var respondent = value != null ? JsonConvert.DeserializeObject<Respondent>(value) : null;
            return respondent;
        }

        public void AddFollowUpQuestion(Question question)
        {
            List<Question> questions = GetFollowUpQuestions();
            questions.Add(question);
            SetFollowUpQuestions(questions);
        }

        public void SetFollowUpQuestions(List<Question> questions)
        {
            _accessor.HttpContext.Session.SetString(SessionKeyFollowUpQuestions, JsonConvert.SerializeObject(questions));
        }

        public List<Question> GetFollowUpQuestions()
        {
            var value = _accessor.HttpContext.Session.GetString(SessionKeyFollowUpQuestions);
            var followUpQuestions = value != null ? JsonConvert.DeserializeObject<List<Question>>(value) : new List<Question>();
            return followUpQuestions;
        }

        public void AddNewAnswers(List<AnswerViewModel> newAnswers)
        {
            List<AnswerViewModel> answers = GetAnswers();
            answers.AddRange(newAnswers);
            SetAnswers(answers);
        }

        public void AddNewAnswer(AnswerViewModel newAnswer)
        {
            List<AnswerViewModel> answers = GetAnswers();
            answers.Add(newAnswer);
            SetAnswers(answers);
        }

        public void SetAnswers(List<AnswerViewModel> answers)
        {
            _accessor.HttpContext.Session.SetString(SessionKeyAnswers, JsonConvert.SerializeObject(answers));
        }

        public List<AnswerViewModel> GetAnswers()
        {
            var value = _accessor.HttpContext.Session.GetString(SessionKeyAnswers);
            var answers = value != null ? JsonConvert.DeserializeObject<List<AnswerViewModel>>(value) : new List<AnswerViewModel>();
            return answers;
        }

        public void Clear()
        {
            _accessor.HttpContext.Session.Clear();
        }

        public void AddQuestion(Question question)
        {
            List<Question> sessionQuestions = GetSessionQuestions();
            sessionQuestions.Add(question);
            _accessor.HttpContext.Session.SetString(SessionKeySessionQuestions, JsonConvert.SerializeObject(sessionQuestions));
        }

        public void RemoveCurrentQuestion()
        {
            List<Question> sessionQuestions = GetSessionQuestions();
            int size = sessionQuestions.Count;
            sessionQuestions.RemoveAt(size-1);
            _accessor.HttpContext.Session.SetString(SessionKeySessionQuestions, JsonConvert.SerializeObject(sessionQuestions));
        }

        public Question GetCurrentQuestion()
        {
            List<Question> sessionQuestions = GetSessionQuestions();
            int size = sessionQuestions.Count;
            return sessionQuestions[size - 1];
        }

        public List<Question> GetSessionQuestions()
        {
            var value = _accessor.HttpContext.Session.GetString(SessionKeySessionQuestions);
            var questions = value != null ? JsonConvert.DeserializeObject<List<Question>>(value) : new List<Question>();
            return questions;
        }
    }
}

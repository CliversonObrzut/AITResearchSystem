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
        private const string SessionKeyQuestionnaireDone = "_questionnaireDone";
        private const string SessionKeyAdminEmail = "_adminEmail";
        private const string SessionKeyIsFollowUpQuestion = "_isFollowUpQuestion";
        private const string SessionKeyFilteredRespondents = "_filteredRespondents";

        private readonly IHttpContextAccessor _accessor;

        public SessionService(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        /// <summary>
        /// Set in session a list of int values representing repondent Ids 
        /// </summary>
        /// <param name="respondentsId"></param>
        public void SetFilteredRespondentsId(List<int> respondentsId)
        {
            _accessor.HttpContext.Session.SetString(SessionKeyFilteredRespondents, JsonConvert.SerializeObject(respondentsId));
        }

        /// <summary>
        /// Returns a List of int representing respondent Ids stored in session
        /// </summary>
        /// <returns></returns>
        public List<int> GetFilteredRespondents()
        {
            var value = _accessor.HttpContext.Session.GetString(SessionKeyFilteredRespondents);
            var done = value != null ? JsonConvert.DeserializeObject<List<int>>(value) : new List<int>();
            return done;
        }

        /// <summary>
        /// Clear in session the list of int values representing respondent ids
        /// </summary>
        public void ClearFilteredRespondents()
        {
            SetFilteredRespondentsId(new List<int>());
        }

        /// <summary>
        /// Set in session a string of "true" or "false" to indicate
        /// the next to be called is a follow up question
        /// </summary>
        /// <param name="value"></param>
        public void SetIsFollowUpQuestion(string value)
        {
            _accessor.HttpContext.Session.SetString(SessionKeyIsFollowUpQuestion,value);
        }

        /// <summary>
        /// Return a string of "true" or "false" to indicate
        /// if the current question is follow up or not
        /// </summary>
        /// <returns></returns>
        public string GetIsFollowUpQuestion()
        {
            return _accessor.HttpContext.Session.GetString(SessionKeyIsFollowUpQuestion);
        }

        /// <summary>
        /// Set in session the Admin email.
        /// </summary>
        /// <param name="email"></param>
        public void SetAdminEmail(string email)
        {
            _accessor.HttpContext.Session.SetString(SessionKeyAdminEmail,email);
        }

        /// <summary>
        /// Returns the Admin email from session.
        /// </summary>
        /// <returns></returns>
        public string GetAdminEmail()
        {
            var value = _accessor.HttpContext.Session.GetString(SessionKeyAdminEmail);
            var done = value ?? "session expired";
            return done;
        }

        /// <summary>
        /// Set in session a boolean indicating if the questionnaire
        /// was complete or not
        /// </summary>
        /// <param name="done"></param>
        public void SetQuestionnaireDone(bool done)
        {
            _accessor.HttpContext.Session.SetString(SessionKeyQuestionnaireDone,JsonConvert.SerializeObject(done));
        }

        /// <summary>
        /// Returns the boolean value indicating if the questionnaire
        /// was complete or not
        /// </summary>
        /// <returns></returns>
        public string GetQuestionnaireDone()
        {
            var value = _accessor.HttpContext.Session.GetString((SessionKeyQuestionnaireDone));
            var done = value != null ? JsonConvert.DeserializeObject<string>(value) : null;
            return done;
        }

        /// <summary>
        /// Add the current respondent to session.
        /// </summary>
        /// <param name="resp"></param>
        public void SetRespondent(Respondent resp)
        {
            _accessor.HttpContext.Session.SetString(SessionKeyRespondent, JsonConvert.SerializeObject(resp));
        }

        /// <summary>
        /// Return from session the current Respondent
        /// </summary>
        /// <returns></returns>
        public Respondent GetRespondent()
        {
            var value = _accessor.HttpContext.Session.GetString(SessionKeyRespondent);
            var respondent = value != null ? JsonConvert.DeserializeObject<Respondent>(value) : null;
            return respondent;
        }

        /// <summary>
        /// Add a follow up question to the List of follow up questions
        /// current in session
        /// </summary>
        /// <param name="question"></param>
        public void AddFollowUpQuestion(Question question)
        {
            List<Question> questions = GetFollowUpQuestions();
            questions.Add(question);
            SetFollowUpQuestions(questions);
        }

        /// <summary>
        /// Set in session a list of follow up questions.
        /// </summary>
        /// <param name="questions"></param>
        public void SetFollowUpQuestions(List<Question> questions)
        {
            _accessor.HttpContext.Session.SetString(SessionKeyFollowUpQuestions, JsonConvert.SerializeObject(questions));
        }

        /// <summary>
        /// Return the list of follow up questions stored in session.
        /// </summary>
        /// <returns></returns>
        public List<Question> GetFollowUpQuestions()
        {
            var value = _accessor.HttpContext.Session.GetString(SessionKeyFollowUpQuestions);
            var followUpQuestions = value != null ? JsonConvert.DeserializeObject<List<Question>>(value) : new List<Question>();
            return followUpQuestions;
        }

        /// <summary>
        /// Add a list of answers to the current list of answers
        /// stored in session.
        /// </summary>
        /// <param name="newAnswers"></param>
        public void AddNewAnswers(List<AnswerViewModel> newAnswers)
        {
            List<AnswerViewModel> answers = GetAnswers();
            answers.AddRange(newAnswers);
            SetAnswers(answers);
        }

        /// <summary>
        /// Add one question to the current list of answers stored in session
        /// </summary>
        /// <param name="newAnswer"></param>
        public void AddNewAnswer(AnswerViewModel newAnswer)
        {
            List<AnswerViewModel> answers = GetAnswers();
            answers.Add(newAnswer);
            SetAnswers(answers);
        }

        /// <summary>
        /// Set in session a list of answers
        /// </summary>
        /// <param name="answers"></param>
        public void SetAnswers(List<AnswerViewModel> answers)
        {
            _accessor.HttpContext.Session.SetString(SessionKeyAnswers, JsonConvert.SerializeObject(answers));
        }

        /// <summary>
        /// Return the list of answers stored in session.
        /// </summary>
        /// <returns></returns>
        public List<AnswerViewModel> GetAnswers()
        {
            var value = _accessor.HttpContext.Session.GetString(SessionKeyAnswers);
            var answers = value != null ? JsonConvert.DeserializeObject<List<AnswerViewModel>>(value) : new List<AnswerViewModel>();
            return answers;
        }

        /// <summary>
        /// Clear all the data stored in session.
        /// </summary>
        public void Clear()
        {
            _accessor.HttpContext.Session.Clear();
        }

        /// <summary>
        /// Add one question to the current list of questions
        /// stored in session.
        /// </summary>
        /// <param name="question"></param>
        public void AddQuestion(Question question)
        {
            List<Question> sessionQuestions = GetSessionQuestions();
            sessionQuestions.Add(question);
            _accessor.HttpContext.Session.SetString(SessionKeySessionQuestions, JsonConvert.SerializeObject(sessionQuestions));
        }

        /// <summary>
        /// Removes the current Question from the list of answered questions
        /// </summary>
        public void RemoveCurrentQuestion()
        {
            List<Question> sessionQuestions = GetSessionQuestions();
            int size = sessionQuestions.Count;
            sessionQuestions.RemoveAt(size-1);
            _accessor.HttpContext.Session.SetString(SessionKeySessionQuestions, JsonConvert.SerializeObject(sessionQuestions));
        }

        /// <summary>
        /// Returns a question from the list of previous answered questions by
        /// position in the list from the last to first.
        /// </summary>
        /// <param name="backwardPosition"></param>
        /// <returns></returns>
        public Question GetPreviousQuestionByPosition(int backwardPosition)
        {
            List<Question> sessionQuestions = GetSessionQuestions();
            int size = sessionQuestions.Count;
            return sessionQuestions[size - backwardPosition];
        }

        /// <summary>
        /// Returns the current question from the list of answered questions
        /// </summary>
        /// <returns></returns>
        public Question GetCurrentQuestion()
        {
            List<Question> sessionQuestions = GetSessionQuestions();
            int size = sessionQuestions.Count;
            return sessionQuestions[size - 1];
        }

        /// <summary>
        /// Returns the list of answered questions stored in session.
        /// </summary>
        /// <returns></returns>
        public List<Question> GetSessionQuestions()
        {
            var value = _accessor.HttpContext.Session.GetString(SessionKeySessionQuestions);
            var questions = value != null ? JsonConvert.DeserializeObject<List<Question>>(value) : new List<Question>();
            return questions;
        }
    }
}

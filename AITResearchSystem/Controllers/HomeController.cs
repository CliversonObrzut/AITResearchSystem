using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AITResearchSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using AITResearchSystem.Models;
using AITResearchSystem.Services;
using AITResearchSystem.Services.Interfaces;
using AITResearchSystem.ViewModels;

namespace AITResearchSystem.Controllers
{
    public class HomeController : Controller
    {
        private Respondent respondent;
        private List<Answer> answers;
        private List<Question> followUpQuestions;
        private int questionNumber;
        private readonly IpAddressService _ipService;
        private readonly IStaff _staffData;
        private readonly IQuestion _questionData;
        private readonly IOption _optionData;

        public HomeController(
            IpAddressService ipService, 
            IStaff staffDb,
            IQuestion questionData,
            IOption optionData)
        {
            respondent = new Respondent();
            answers = new List<Answer>();
            followUpQuestions = new List<Question>();
            _ipService = ipService;
            _staffData = staffDb;
            _questionData = questionData;
            _optionData = optionData;
            questionNumber = 0;
        }

        public IActionResult Index()
        {
            ViewData["User"] = "Anonymous";
            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            var model = new StaffViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(StaffViewModel model)
        {
            if (ModelState.IsValid)
            {
                Staff staff = _staffData.GetByEmailAndPassword(model.Email, model.Password);
                if (staff == null)
                {
                    ModelState.AddModelError("", "wrong email or password");
                    return View(model);
                }
                return RedirectToAction(nameof(Admin), new {email = staff.Email});
            }
            return View();
        }

        public IActionResult Logout()
        {
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Register()
        {
            var model = new RegisterFirstViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterFirstViewModel model)
        {
            if (ModelState.IsValid)
            {
                respondent.GivenNames = model.GivenNames;
                respondent.LastName = model.LastName;
                respondent.DateOfBirth = model.DateOfBirth;
                respondent.PhoneNumber = model.PhoneNumber;
                respondent.Date = DateTime.Now;
                respondent.IpAddress = _ipService.GetIpAddress();

                return RedirectToAction(nameof(Question), new {nextQuestion = 1});
            }
            return View();
        }

        [HttpGet]
        public IActionResult Question(int nextQuestion)
        {
            if (respondent.LastName == "Anonymous")
                ViewData["User"] = "Anonymous";
            else
                ViewData["User"] = respondent.GivenNames + " " + respondent.LastName;

            questionNumber++;

            Question question = _questionData.GetByOrder(nextQuestion);

            var model = new QuestionViewModel()
            {
                Type = question.QuestionType.Type,
                Text = question.Text,
                Options = question.QuestionOptions,
                QuestionSequence = questionNumber,
                Order = question.Order
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Question(QuestionViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Order == null)
                {
                    if (followUpQuestions.Count == 0)
                    {
                        questionNumber = 0;
                        respondent = null;
                        answers = null;
                        followUpQuestions = null;
                        return RedirectToAction(nameof(Index)); 
                    }
                    var order = followUpQuestions[0].Order;
                    followUpQuestions.RemoveAt(0);
                    return RedirectToAction(nameof(Question), new { nextQuestion = order});
                }
                if (model.Type == "radio")
                {
                    return RedirectToAction(nameof(Question), new { nextQuestion = model.Order + 1 });
                }

                if (model.Type == "text")
                {
                    return RedirectToAction(nameof(Question), new { nextQuestion = model.Order + 1 });
                }

                if (model.Type == "checkbox")
                {
                    return RedirectToAction(nameof(Question), new { nextQuestion = model.Order + 1 });
                }
            }
            return View(model);
        }

        public IActionResult Admin(string email)
        {
            ViewData["User"] = email;
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult StartAnonymously()
        {
            respondent.GivenNames = "";
            respondent.LastName = "Anonymous";
            respondent.DateOfBirth = new DateTime();
            respondent.PhoneNumber = "";
            respondent.Date = DateTime.Now;
            respondent.IpAddress = _ipService.GetIpAddress();
            return RedirectToAction(nameof(Question), new { nextQuestion = 1});
        }
    }
}

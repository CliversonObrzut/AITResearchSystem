using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AITResearchSystem.Data.Models;
using Microsoft.AspNetCore.Mvc;
using AITResearchSystem.Models;
using AITResearchSystem.Services;
using AITResearchSystem.Services.Interfaces;
using AITResearchSystem.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace AITResearchSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly SessionService _session;
        private readonly IpAddressService _ipService;
        private readonly IStaff _staffData;
        private readonly IQuestion _questionData;
        private readonly IOption _optionData;
        private readonly IRespondent _respondentData;
        private readonly IAnswer _answerData;

        public HomeController(
            IpAddressService ipService, 
            IStaff staffDb,
            IQuestion questionData,
            IRespondent respondentData,
            IOption optionData,
            IAnswer answerData,
            SessionService session)
        {
            _ipService = ipService;
            _staffData = staffDb;
            _questionData = questionData;
            _respondentData = respondentData;
            _optionData = optionData;
            _answerData = answerData;
            _session = session;
        }

        public IActionResult Index()
        {
            if (_session.GetRespondent() != null)
                ViewData["Session"] = "true";
            else
                ViewData["Session"] = "false";
            return View();
        }

        public IActionResult Restart()
        {
            _session.Clear();
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Continue()
        {
            var question = _session.GetCurrentQuestion();
            _session.RemoveCurrentQuestion();
            return RedirectToAction(nameof(Question), new { nextQuestion = question.Order });
        }

        [HttpGet]
        public IActionResult Login()
        {
            var model = new StaffViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(StaffViewModel model)
        {
            if (ModelState.IsValid)
            {
                Staff staff = _staffData.GetByEmailAndPassword(model.Email, model.Password);
                if (staff == null)
                {
                    ModelState.AddModelError("", "wrong email or password");
                    return View(model);
                }
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, staff.Email));
                identity.AddClaim(new Claim(ClaimTypes.Name, staff.Email));
                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = true });
                
                return RedirectToAction(nameof(Admin), new {email = staff.Email});
            }
            return View();
        }

        [Authorize]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (_session.GetRespondent() != null)
            {
                var question = _session.GetCurrentQuestion();
                _session.RemoveCurrentQuestion();
                return RedirectToAction(nameof(Question), new {nextQuestion = question.Order});
            }
            var model = new RegisterFirstViewModel();
            return View(model);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterFirstViewModel model)
        {
            if (ModelState.IsValid)
            {
                Respondent respondent = new Respondent
                {
                    GivenNames = model.GivenNames,
                    LastName = model.LastName,
                    DateOfBirth = model.DateOfBirth,
                    PhoneNumber = model.PhoneNumber,
                    Date = DateTime.Now,
                    IpAddress = _ipService.GetIpAddress()
                };

                _session.SetRespondent(respondent);

                return RedirectToAction(nameof(Question), new {nextQuestion = 1});
            }
            return View();
        }

        [HttpGet]
        public IActionResult Question(int nextQuestion)
        {
            var respondent = _session.GetRespondent();
            if (respondent == null)
            {
                return RedirectToAction(nameof(Index));
            }
            
            if (respondent.LastName == "Anonymous")
                ViewData["User"] = "Anonymous";
            else
                ViewData["User"] = respondent.GivenNames + " " + respondent.LastName;

            Question question = _questionData.GetByOrder(nextQuestion);

            if(_session.GetSessionQuestions().Any(q => q.Id == question.Id))
            {
                var questionInSession = _session.GetCurrentQuestion();
                _session.RemoveCurrentQuestion();
                return RedirectToAction(nameof(Question), new { nextQuestion = questionInSession.Order });
            }   

            _session.AddQuestion(question);

            var model = new QuestionViewModel
            {
                Type = question.QuestionType.Type,
                QuestionText = question.Text,
                Order = question.Order,
                Options = question.QuestionOptions
            };

            var answers = _session.GetAnswers();
            if (answers.Any(qa => qa.QuestionId == question.Id))
            {
                model.QuestionSequence = answers.FirstOrDefault(a => a.QuestionId == question.Id)?.QuestionSequence;

                if (question.QuestionType.Type == "radio")
                {
                    model.OptionRadioAnswer = answers.FirstOrDefault(a => a.QuestionId == question.Id)?.OptionId.ToString();
                }

                if (question.QuestionType.Type == "text")
                {
                    if (question.Order == 4)
                    {
                        model.Suburb = answers.Find(a => a.QuestionId == question.Id && !a.Suburb.Equals("")).Suburb;
                        //model.PostCode = answers.FirstOrDefault(a => a.QuestionId == question.Id && a.PostCode != null).PostCode;
                        model.Postcode = answers.Find(a => a.QuestionId == question.Id && !a.Postcode.Equals("")).Postcode;
                    }

                    if (question.Order == 5)
                    {
                        model.Email = answers.Find(a => a.QuestionId == question.Id).Email;
                    }
                }

                if (question.QuestionType.Type == "checkbox")
                {
                    List<CheckboxViewModel> checkboxesAnswers = new List<CheckboxViewModel>();
                    foreach (var option in model.Options)
                    {
                        CheckboxViewModel checkbox = new CheckboxViewModel
                        {
                            Id = option.Id,
                            NextQuestion = option.NextQuestion,
                            IsSelected = false
                        };
                        if (answers.Any(a => a.OptionId == checkbox.Id))
                            checkbox.IsSelected = true;
                        checkboxesAnswers.Add(checkbox);
                    }

                    model.OptionCheckboxAnswers = checkboxesAnswers;
                }
                var revisedAnswers = new List<AnswerViewModel>();
                foreach (var answer in answers)
                {
                    if (answer.QuestionId != question.Id)
                    {
                        revisedAnswers.Add(answer);
                    }
                }
                _session.SetAnswers(revisedAnswers);
            }
            else if(answers.Count == 0)
            {
                model.QuestionSequence = 1;
            }
            else
            {
                model.QuestionSequence = answers[answers.Count - 1].QuestionSequence + 1;
            }

            return View(model);
        }

        [HttpPost]
        public IActionResult Question(QuestionViewModel model)
        {
            var currentQuestion = _session.GetCurrentQuestion();
            CheckExtraModelState(model, currentQuestion);
            if (ModelState.IsValid)
            {
                if (currentQuestion.QuestionType.Type == "radio")
                {
                    var answer = new AnswerViewModel
                    {
                        QuestionSequence = model.QuestionSequence,
                        Email = "",
                        Suburb = "",
                        Postcode = "",
                        OptionId = int.Parse(model.OptionRadioAnswer),
                        QuestionId = currentQuestion.Id,
                    };
                    _session.AddNewAnswer(answer);
                }

                if (currentQuestion.QuestionType.Type == "text")
                {
                    if (currentQuestion.Order == 4)
                    {
                        var answers = new List<AnswerViewModel>();

                        var suburbAnswer = new AnswerViewModel()
                        {
                            QuestionSequence = model.QuestionSequence,
                            Email = "",
                            Suburb = model.Suburb,
                            Postcode = "",
                            OptionId = null,
                            QuestionId = currentQuestion.Id
                        };
                        answers.Add(suburbAnswer);

                        var postcodeAnswer = new AnswerViewModel()
                        {
                            QuestionSequence = model.QuestionSequence,
                            Email = "",
                            Suburb = "",
                            Postcode = model.Postcode,
                            OptionId = null,
                            QuestionId = currentQuestion.Id
                        };
                        answers.Add(postcodeAnswer);

                        _session.AddNewAnswers(answers);
                    }
                    else if (currentQuestion.Order == 5)
                    {
                        var emailAnswer = new AnswerViewModel
                        {
                            QuestionSequence = model.QuestionSequence,
                            Email = model.Email,
                            Suburb = "",
                            Postcode = "",
                            OptionId = null,
                            QuestionId = currentQuestion.Id
                        };
                        _session.AddNewAnswer(emailAnswer);
                    }
                }

                if (currentQuestion.QuestionType.Type == "checkbox")
                {
                    var answers = new List<AnswerViewModel>();
                    foreach (var option in model.OptionCheckboxAnswers)
                    {
                        if (option.IsSelected)
                        {
                            var answer = new AnswerViewModel()
                            {
                                QuestionSequence = model.QuestionSequence,
                                Email = "",
                                Suburb = "",
                                Postcode = "",
                                OptionId = option.Id,
                                QuestionId = currentQuestion.Id
                            };
                            answers.Add(answer);
                            if (option.NextQuestion != null)
                            {
                                var nextQuestion = option.NextQuestion ?? 1;
                                var question = _questionData.GetByOrder(nextQuestion);
                                var sessionFollowUpQuestions = _session.GetFollowUpQuestions();
                                if (sessionFollowUpQuestions.Count != 0)
                                {
                                    if (sessionFollowUpQuestions.All(fq => fq.Order != question.Order))
                                    {
                                        _session.AddFollowUpQuestion(question);
                                    }
                                }
                                else
                                {
                                    _session.AddFollowUpQuestion(question);
                                }
                            }
                        }
                    }
                    _session.AddNewAnswers(answers);
                }
                return CallNextQuestion(currentQuestion);
            }

            model.Type = currentQuestion.QuestionType.Type;
            model.QuestionText = currentQuestion.Text;
            model.Order = currentQuestion.Order;
            model.Options = currentQuestion.QuestionOptions;
            return View(model);
        }

        private void CheckExtraModelState(QuestionViewModel model, Question currentQuestion)
        {
            if (currentQuestion.QuestionType.Type == "radio")
            {
                if (model.OptionRadioAnswer == null)
                {
                    ModelState.AddModelError("OptionRadioAnswer", "one option is required");
                }
            }

            if (currentQuestion.QuestionType.Type == "text")
            {
                if (currentQuestion.Order == 4 &&
                    model.Suburb == null)
                {
                    ModelState.AddModelError("Suburb", "Suburb is required");
                }

                if (currentQuestion.Order == 4 &&
                    model.Postcode == null)
                {
                    ModelState.AddModelError("PostCode", "Postcode is required");
                }

                if (currentQuestion.Order == 5 &&
                    model.Email == null)
                {
                    ModelState.AddModelError("Email", "Email is required");
                }
            }

            if (currentQuestion.QuestionType.Type == "checkbox")
            {
                if (currentQuestion.Order == 6 &&
                    model.OptionCheckboxAnswers.Count(answer => answer.IsSelected) > 4)
                {
                    ModelState.AddModelError("OptionCheckboxAnswers", "Max of 4 banks");
                }

                if (currentQuestion.Order == 7 &&
                    model.OptionCheckboxAnswers.Count(answer => answer.IsSelected) > 2)
                {
                    ModelState.AddModelError("OptionCheckboxAnswers", "Max of 2 newspapers");
                }

                // Model State to have at least one checkbox marked
                //if (model.OptionCheckboxAnswers.All(answer => !answer.IsSelected))
                //{
                //    ModelState.AddModelError("OptionCheckboxAnswers", "one option is required");
                //}
            }
        }

        private IActionResult CallNextQuestion(Question currentQuestion)
        {
            if (!(currentQuestion.Order >= 6))
                return RedirectToAction(nameof(Question), new {nextQuestion = currentQuestion.Order + 1});

            var followUpQuestions = _session.GetFollowUpQuestions();
            if (followUpQuestions.Count == 0)
            {
                if (currentQuestion.Order != 7 && !(currentQuestion.Order >= 9))
                    return RedirectToAction(nameof(Question), new {nextQuestion = (int?) 7});
                UpdateAnswersInDatabase();
                _session.Clear();
                return RedirectToAction(nameof(End));
            }

            var order = followUpQuestions[0].Order;
            followUpQuestions.RemoveAt(0);
            _session.SetFollowUpQuestions(followUpQuestions);
            return RedirectToAction(nameof(Question), new {nextQuestion = order});
        }

        private void UpdateAnswersInDatabase()
        {
            Respondent respondent = _session.GetRespondent();
            _respondentData.Add(respondent);
            List<AnswerViewModel> answersInSession = _session.GetAnswers();
            List<Answer> answers = new List<Answer>();
            foreach (var answerInSession in answersInSession)
            {
                var answer = answerInSession.ConvertToAnswer(respondent);
                answers.Add(answer);
            }
            _answerData.AddRange(answers);
        }

        public IActionResult End()
        {
            if (_session.GetRespondent() != null)
            {
                var question = _session.GetCurrentQuestion();
                _session.RemoveCurrentQuestion();
                return RedirectToAction(nameof(Question), new { nextQuestion = question.Order });
            }
            
            return View();
        }

        [Authorize]
        public IActionResult Admin(string email)
        {
            ViewData["User"] = email;
            List<Respondent> respondents = _respondentData.GetAll().ToList();
            List<Option> questionOptions = _optionData.GetAll().ToList();


            var model = new AdminViewModel
            {
                Genders = new List<DropdownOption>(),
                AgeRanges = new List<DropdownOption>(),
                States = new List<DropdownOption>()
            };

            for (int i = 0; i < 19; i++)
            {
                // options range for Genders
                if (i >= 0 && i <= 1)
                {
                    DropdownOption gender = new DropdownOption
                    {
                        Id = questionOptions[i].Id,
                        Text = questionOptions[i].Text,
                        IsSelected = false
                    };
                    model.Genders.Add(gender);
                }

                // options range for Age Ranges
                if (i >= 2 && i <= 9)
                {
                    DropdownOption ageRange = new DropdownOption
                    {
                        Id = questionOptions[i].Id,
                        Text = questionOptions[i].Text,
                        IsSelected = false
                    };
                    model.AgeRanges.Add(ageRange);
                }

                // options range for States/Territories
                if (i >= 10 && i <= 18)
                {
                    DropdownOption state = new DropdownOption
                    {
                        Id = questionOptions[i].Id,
                        Text = questionOptions[i].Text,
                        IsSelected = false
                    };
                    model.States.Add(state);
                }
            }

            List<TableRowAnswer> tableRowAnswers = new List<TableRowAnswer>();
            foreach (var respondent in respondents)
            {
                var tableRowAnswer = new TableRowAnswer
                {
                    LastName = respondent.LastName,
                    GivenNames = respondent.GivenNames,
                    PhoneNumber = respondent.PhoneNumber
                };
                var raGenderOptionId = respondent.RespondentAnswers.Find(a => a.QuestionId == 1).OptionId;
                tableRowAnswer.Gender = questionOptions.Find(o => o.Id == raGenderOptionId).Text;
                var raAgeOptionId = respondent.RespondentAnswers.Find(a => a.QuestionId == 2).OptionId;
                tableRowAnswer.AgeRange = questionOptions.Find(o => o.Id == raAgeOptionId).Text;
                var raStateOptionId = respondent.RespondentAnswers.Find(a => a.QuestionId == 3).OptionId;
                tableRowAnswer.State = questionOptions.Find(o => o.Id == raStateOptionId).Text;
                var answersQuestionFour = respondent.RespondentAnswers.Where(a => a.QuestionId == 4).ToList();
                var raSuburb = answersQuestionFour[0].Text;
                var raPostCode = answersQuestionFour[1].Text;
                tableRowAnswer.Suburb = raSuburb;
                tableRowAnswer.Postcode = raPostCode;
                tableRowAnswer.Email = respondent.RespondentAnswers.Find(a => a.QuestionId == 5).Text;
      
                tableRowAnswers.Add(tableRowAnswer);
            }

            model.Answers = tableRowAnswers.OrderBy(a => a.LastName).ToList();
            return View(model);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult StartAnonymously()
        {
            if (_session.GetRespondent() != null)
            {
                var question = _session.GetCurrentQuestion();
                _session.RemoveCurrentQuestion();
                return RedirectToAction(nameof(Question), new { nextQuestion = question.Order });
            }
            var respondent = new Respondent
            {
                GivenNames = "",
                LastName = "Anonymous",
                DateOfBirth = new DateTime(),
                PhoneNumber = "",
                Date = DateTime.Now,
                IpAddress = _ipService.GetIpAddress()
            };

            _session.SetRespondent(respondent);

            return RedirectToAction(nameof(Question), new { nextQuestion = 1});
        }

        public IActionResult PreviousQuestion()
        {
            _session.RemoveCurrentQuestion();
            Question previousQuestion = _session.GetCurrentQuestion();
            _session.RemoveCurrentQuestion();
            return RedirectToAction(nameof(Question), new { nextQuestion = previousQuestion.Order });
        }
    }
}

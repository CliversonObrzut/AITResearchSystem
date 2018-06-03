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
        // Injected all necessary services.
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

        /// <summary>
        /// Return the View Index
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            if (_session.GetRespondent() != null)
                ViewData["Session"] = "true";
            else
                ViewData["Session"] = "false";
            return View();
        }

        /// <summary>
        /// Button Restart will redirect to Index page.
        /// </summary>
        /// <returns></returns>
        public IActionResult Restart()
        {
            _session.Clear();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Button Continue will redirect to the last question in the current session.
        /// </summary>
        /// <returns></returns>
        public IActionResult Continue()
        {
            var question = _session.GetCurrentQuestion();
            _session.RemoveCurrentQuestion();
            // follow up questions have order null
            if (question.Order == null) // defines if the next question will be called by order or by id.
            {
                _session.SetIsFollowUpQuestion("true");
                return RedirectToAction(nameof(Question), new { nextQuestion = question.Id });
            }
            return RedirectToAction(nameof(Question), new { nextQuestion = question.Order });
        }

        /// <summary>
        /// Returns the View Login passing StaffViewModel.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Login()
        {
            var model = new StaffViewModel();
            return View(model);
        }

        /// <summary>
        /// Post method to authenticate staff and redirect do Admin Page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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
                _session.SetAdminEmail(staff.Email);
                return RedirectToAction(nameof(Admin));
            }
            return View();
        }

        /// <summary>
        /// Button Logout redirects to Index page.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _session.Clear();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Returns the View Register passing RegisterFirstViewModel.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Register()
        {
            if (_session.GetRespondent() != null)
            {
                var question = _session.GetCurrentQuestion();
                _session.RemoveCurrentQuestion();
                if (question.Order == null)
                {
                    _session.SetIsFollowUpQuestion("true");
                    return RedirectToAction(nameof(Question), new { nextQuestion = question.Id });
                }
                return RedirectToAction(nameof(Question), new { nextQuestion = question.Order });
            }
            var model = new RegisterFirstViewModel();
            return View(model);

        }

        /// <summary>
        /// Post method to store respondent information in session and
        /// redirects to first question.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
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
                _session.SetIsFollowUpQuestion("false");

                return RedirectToAction(nameof(Question), new { nextQuestion = 1 });
            }
            return View();
        }

        /// <summary>
        /// Returns the Question View loading questions information from QuestionViewModel.
        /// </summary>
        /// <param name="nextQuestion"></param>
        /// <returns></returns>
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

            var question = _session.GetIsFollowUpQuestion() == "false" ? _questionData.GetByOrder(nextQuestion) : _questionData.GetById(nextQuestion);
            _session.SetIsFollowUpQuestion("false");

            if (_session.GetSessionQuestions().Any(q => q.Id == question.Id))
            {
                var questionInSession = _session.GetCurrentQuestion();
                _session.RemoveCurrentQuestion();
                if (questionInSession.Order == null)
                {
                    _session.SetIsFollowUpQuestion("true");
                    return RedirectToAction(nameof(Question), new { nextQuestion = questionInSession.Id });
                }
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
                    model.TextAnswer = answers.Find(a => a.QuestionId == question.Id).Text;
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
            else if (answers.Count == 0)
            {
                model.QuestionSequence = 1;
            }
            else
            {
                model.QuestionSequence = answers[answers.Count - 1].QuestionSequence + 1;
            }

            return View(model);
        }

        /// <summary>
        /// Post Method to receive questionnaire answers and store in session.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Question(QuestionViewModel model)
        {
            var respondent = _session.GetRespondent();
            if (respondent == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var currentQuestion = _session.GetCurrentQuestion();
            CheckExtraModelState(model, currentQuestion);
            if (ModelState.IsValid)
            {
                if (currentQuestion.QuestionType.Type == "radio")
                {
                    var answer = new AnswerViewModel
                    {
                        QuestionSequence = model.QuestionSequence,
                        Text = "",
                        OptionId = int.Parse(model.OptionRadioAnswer),
                        QuestionId = currentQuestion.Id,
                    };
                    _session.AddNewAnswer(answer);
                }

                if (currentQuestion.QuestionType.Type == "text")
                {
                    var textAnswer = new AnswerViewModel()
                    {
                        QuestionSequence = model.QuestionSequence,
                        Text = model.TextAnswer,
                        OptionId = null,
                        QuestionId = currentQuestion.Id
                    };
                    _session.AddNewAnswer(textAnswer);
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
                                Text = "",
                                OptionId = option.Id,
                                QuestionId = currentQuestion.Id
                            };
                            answers.Add(answer);
                            if (option.NextQuestion != null)
                            {
                                var nextQuestion = option.NextQuestion ?? 1;
                                var question = _questionData.GetById(nextQuestion);
                                var sessionFollowUpQuestions = _session.GetFollowUpQuestions();
                                if (sessionFollowUpQuestions.Count != 0)
                                {
                                    if (sessionFollowUpQuestions.All(fq => fq.Id != question.Id))
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

        /// <summary>
        /// Check model state based in questionnaire rules.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="currentQuestion"></param>
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
                if (model.TextAnswer.Equals(""))
                {
                    ModelState.AddModelError("TextAnswer", "Any answer is required");
                }
            }

            if (currentQuestion.QuestionType.Type == "checkbox")
            {
                if (currentQuestion.Order == 7 &&
                    model.OptionCheckboxAnswers.Count(answer => answer.IsSelected) > 4)
                {
                    ModelState.AddModelError("OptionCheckboxAnswers", "Max of 4 banks");
                }

                if (currentQuestion.Order == 8 &&
                    model.OptionCheckboxAnswers.Count(answer => answer.IsSelected) > 2)
                {
                    ModelState.AddModelError("OptionCheckboxAnswers", "Max of 2 newspapers");
                }
            }
        }

        /// <summary>
        /// Determines which question to call next.
        /// </summary>
        /// <param name="currentQuestion"></param>
        /// <returns></returns>
        private IActionResult CallNextQuestion(Question currentQuestion)
        {
            var followUpQuestions = _session.GetFollowUpQuestions();
            // if there are not follow up questions in Session
            if (followUpQuestions.Count == 0)
            {
                // check if the currect question is a regular question or a follow up question
                var regularQuestion = currentQuestion;
                if (regularQuestion.Order == null) // follow up questions have Order = null in Database.
                {
                    int backwardPosition = 1;
                    while (regularQuestion.Order == null) // start reading the previous questions stored in Session until find a regular question
                    {
                        regularQuestion = _session.GetPreviousQuestionByPosition(backwardPosition);
                        backwardPosition++;
                    }
                }
                // when find the last regular question it gets the order and increment to find the next regular question
                var nextQuestionOrder = regularQuestion.Order + 1 ?? 0;
                // checks if the next regular question exist or if the last regular question is the last one from the questionnaire
                if (_questionData.IsNextQuestionAvailable(nextQuestionOrder))
                {
                    return RedirectToAction(nameof(Question), new { nextQuestion = nextQuestionOrder });
                }
                // in case the next question does not exist, saves the answers in database and clear the session
                UpdateAnswersInDatabase();
                _session.Clear();
                _session.SetQuestionnaireDone(true); // sets the questionnaire done in session to call the End page from the Question Page avoiding attempts to call End page typing in the URL.
                return RedirectToAction(nameof(End));
            }

            var followUpQuestionId = followUpQuestions[0].Id;
            followUpQuestions.RemoveAt(0);
            _session.SetIsFollowUpQuestion("true");
            _session.SetFollowUpQuestions(followUpQuestions);
            return RedirectToAction(nameof(Question), new { nextQuestion = followUpQuestionId });
        }

        /// <summary>
        /// Insert Respondent and Answers data into database. Clear the Session.
        /// </summary>
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

        /// <summary>
        /// Returns the View End page.
        /// </summary>
        /// <returns></returns>
        public IActionResult End()
        {
            // if trying to open End page trough URL it's going to redirect the user to Home Page
            if (_session.GetQuestionnaireDone() != "true")
            {
                return RedirectToAction(nameof(Index));
            }
            // clear everything in session
            _session.Clear();

            // loads End Page
            return View();
        }

        /// <summary>
        /// Returns the View Admin with the Filter options and Respondent Answers.
        /// </summary>
        /// <param name="filtered"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public IActionResult Admin(int filtered)
        {
            var admin = _session.GetAdminEmail();
            if (admin.Equals("session expired"))
            {
                return RedirectToAction(nameof(Logout));
            }

            ViewData["User"] = admin;

            List<Respondent> respondents = new List<Respondent>();
            if (filtered == 0)
            {
                respondents = _respondentData.GetAll().ToList();
            }
            else
            {
                List<int> respondentsId = _session.GetFilteredRespondents();
                foreach (var respondentId in respondentsId)
                {
                    respondents.Add(_respondentData.Get(respondentId));
                }
                _session.ClearFilteredRespondents();
            }

            var model = new AdminViewModel
            {
                Genders = _optionData.GetByQuestion(1).ToList(),
                AgeRanges = _optionData.GetByQuestion(2).ToList(),
                States = _optionData.GetByQuestion(3).ToList(),
                Answers = LoadAnswerTable(respondents)
                    .OrderBy(a => a.LastName == "Anonymous")
                    .ThenBy(a => a.LastName)
                    .ToList()
            };

            return View(model);
        }

        /// <summary>
        /// Returns the Answers that Contain the query string.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Search(AdminViewModel model)
        {
            if (ModelState.IsValid)
            {
                List<Answer> matchedAnswers = _answerData.Search(model.SearchQuery).ToList();
                List<int> matchedRespondentsId = new List<int>();
                foreach (var answer in matchedAnswers)
                {
                    if (matchedRespondentsId.All(r => r != answer.RespondentId))
                    {
                        matchedRespondentsId.Add(answer.RespondentId);
                    }
                }
                _session.SetFilteredRespondentsId(matchedRespondentsId);
                return RedirectToAction(nameof(Admin), new { filtered = 1 });
            }

            return RedirectToAction(nameof(Admin));
        }

        /// <summary>
        /// Returns the Answers that matches the options selected in the Filter.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Filter(AdminViewModel model)
        {
            if (ModelState.IsValid && model.SelectedFilters.Any(option => option != 0))
            {
                List<Answer> filterOne = new List<Answer>();
                List<Answer> filterTwo = new List<Answer>();
                List<Answer> filterThree = new List<Answer>();
                bool matchCondition = true;
                foreach (var option in model.SelectedFilters)
                {
                    if (option != 0)
                    {
                        var filteredAnswers = _answerData.FilterByOptionId(option).ToList();
                        if (filteredAnswers.Count > 0)
                        {
                            if (filteredAnswers[0].QuestionId == 1)
                            {
                                filterOne.AddRange(filteredAnswers);
                            }
                            else if (filteredAnswers[0].QuestionId == 2)
                            {
                                filterTwo.AddRange(filteredAnswers);
                            }
                            else
                            {
                                filterThree.AddRange(filteredAnswers);
                            }
                        }
                        else
                        {
                            matchCondition = false;
                        }
                    }
                }

                List<Answer> matchedAnswers = new List<Answer>();
                if (matchCondition)
                {
                    matchedAnswers = filterOne;
                    if (matchedAnswers.Count != 0)
                    {
                        if (filterTwo.Count != 0)
                        {
                            List<Answer> tempAnswers = new List<Answer>();
                            foreach (var filteredAnswer in filterTwo)
                            {
                                if (matchedAnswers.Any(a => a.RespondentId == filteredAnswer.RespondentId))
                                {
                                    tempAnswers.Add(filteredAnswer);
                                }
                            }
                            matchedAnswers = tempAnswers;
                        }
                    }
                    else
                    {
                        matchedAnswers = filterTwo;
                    }

                    if (matchedAnswers.Count != 0)
                    {
                        if (filterThree.Count != 0)
                        {
                            List<Answer> tempAnswers = new List<Answer>();
                            foreach (var filteredAnswer in filterThree)
                            {
                                if (matchedAnswers.Any(a => a.RespondentId == filteredAnswer.RespondentId))
                                {
                                    tempAnswers.Add(filteredAnswer);
                                }
                            }

                            matchedAnswers = tempAnswers;
                        }
                    }
                    else
                    {
                        matchedAnswers = filterThree;
                    }
                }


                List<int> matchedRespondentsId = new List<int>();
                foreach (var answer in matchedAnswers)
                {
                    if (matchedRespondentsId.All(r => r != answer.RespondentId))
                    {
                        matchedRespondentsId.Add(answer.RespondentId);
                    }
                }
                _session.SetFilteredRespondentsId(matchedRespondentsId);
                return RedirectToAction(nameof(Admin), new { filtered = 1 });
            }
            return RedirectToAction(nameof(Admin));
        }

        /// <summary>
        /// Clear Search and Filters loading all Answers.
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ClearAll()
        {
            return RedirectToAction(nameof(Admin));
        }

        /// <summary>
        /// Build the Answers table with the list of respondents passed.
        /// </summary>
        /// <param name="respondents"></param>
        /// <returns></returns>
        private List<TableRowAnswer> LoadAnswerTable(List<Respondent> respondents)
        {
            List<TableRowAnswer> tableRowAnswers = new List<TableRowAnswer>();
            List<Option> questionOptions = _optionData.GetAll().ToList();
            foreach (var respondent in respondents)
            {
                var tableRowAnswer = new TableRowAnswer
                {
                    LastName = respondent.LastName,
                    GivenNames = respondent.GivenNames,
                    PhoneNumber = respondent.PhoneNumber,
                    Suburb = respondent.RespondentAnswers.Find(t => t.QuestionId == 4).Text,
                    Postcode = respondent.RespondentAnswers.Find(t => t.QuestionId == 5).Text,
                    Email = respondent.RespondentAnswers.Find(a => a.QuestionId == 6).Text
                };
                var raGenderOptionId = respondent.RespondentAnswers.Find(a => a.QuestionId == 1).OptionId;
                tableRowAnswer.Gender = questionOptions.Find(o => o.Id == raGenderOptionId).Text;
                var raAgeOptionId = respondent.RespondentAnswers.Find(a => a.QuestionId == 2).OptionId;
                tableRowAnswer.AgeRange = questionOptions.Find(o => o.Id == raAgeOptionId).Text;
                var raStateOptionId = respondent.RespondentAnswers.Find(a => a.QuestionId == 3).OptionId;
                tableRowAnswer.State = questionOptions.Find(o => o.Id == raStateOptionId).Text;

                var banks = new List<Option>();
                var banksAnswers = respondent.RespondentAnswers.Where(a => a.QuestionId == 7).ToList();
                foreach (var banksAnswer in banksAnswers)
                {
                    Option option = questionOptions.Find(o => o.Id == banksAnswer.OptionId);
                    banks.Add(option);
                }

                var bankServices = new List<Option>();
                var bankServicesAnswers = respondent.RespondentAnswers.Where(a => a.QuestionId == 9).ToList();
                foreach (var bankServiceAnswer in bankServicesAnswers)
                {
                    Option option = questionOptions.Find(o => o.Id == bankServiceAnswer.OptionId);
                    bankServices.Add(option);
                }

                var newspapers = new List<Option>();
                var newspapersAnswers = respondent.RespondentAnswers.Where(a => a.QuestionId == 8).ToList();
                foreach (var newspapersAnswer in newspapersAnswers)
                {
                    Option option = questionOptions.Find(o => o.Id == newspapersAnswer.OptionId);
                    newspapers.Add(option);
                }

                var newspaperSections = new List<Option>();
                var newspaperSectionsAnswers = respondent.RespondentAnswers.Where(a => a.QuestionId == 10).ToList();
                foreach (var newspaperSectionsAnswer in newspaperSectionsAnswers)
                {
                    Option option = questionOptions.Find(o => o.Id == newspaperSectionsAnswer.OptionId);
                    newspaperSections.Add(option);
                }

                var sportsSection = new List<Option>();
                var sportsSectionAnswers = respondent.RespondentAnswers.Where(a => a.QuestionId == 11).ToList();
                foreach (var sportsSectionAnswer in sportsSectionAnswers)
                {
                    Option option = questionOptions.Find(o => o.Id == sportsSectionAnswer.OptionId);
                    sportsSection.Add(option);
                }

                var travelsSection = new List<Option>();
                var travelsSectionAnswers = respondent.RespondentAnswers.Where(a => a.QuestionId == 12).ToList();
                foreach (var travelsSectionAnswer in travelsSectionAnswers)
                {
                    Option option = questionOptions.Find(o => o.Id == travelsSectionAnswer.OptionId);
                    travelsSection.Add(option);
                }

                var tableRowExpanded = new TableRowExpanded
                {
                    Banks = banks,
                    BankServices = bankServices,
                    Newspapers = newspapers,
                    NewspaperSections = newspaperSections,
                    SportsSection = sportsSection,
                    TravelsSection = travelsSection
                };

                tableRowAnswer.TableRowExpanded = tableRowExpanded;
                tableRowAnswers.Add(tableRowAnswer);
            }

            return tableRowAnswers;
        }

        /// <summary>
        /// Error page for Production Environment.
        /// </summary>
        /// <returns></returns>
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        /// <summary>
        /// Stores the anonymous respondent in session and Return the View Question
        /// </summary>
        /// <returns></returns>
        public IActionResult StartAnonymously()
        {
            if (_session.GetRespondent() != null)
            {
                var question = _session.GetCurrentQuestion();
                _session.RemoveCurrentQuestion();
                if (question.Order == null)
                {
                    _session.SetIsFollowUpQuestion("true");
                    return RedirectToAction(nameof(Question), new { nextQuestion = question.Id });
                }
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
            _session.SetIsFollowUpQuestion("false");

            return RedirectToAction(nameof(Question), new { nextQuestion = 1 });
        }

        /// <summary>
        /// Returns the Question View displaying the previous question and answers.
        /// </summary>
        /// <returns></returns>
        public IActionResult PreviousQuestion()
        {
            if (_session.GetRespondent() == null)
            {
                return RedirectToAction(nameof(Index));
            }
            _session.RemoveCurrentQuestion();
            Question previousQuestion = _session.GetCurrentQuestion();
            _session.RemoveCurrentQuestion();
            if (previousQuestion.Order == null)
            {
                _session.SetIsFollowUpQuestion("true");
                return RedirectToAction(nameof(Question), new { nextQuestion = previousQuestion.Id });
            }
            return RedirectToAction(nameof(Question), new { nextQuestion = previousQuestion.Order });
        }
    }
}

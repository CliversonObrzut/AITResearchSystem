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
    // Controller for all AITR Pages and Requests
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

        // Constructor initialize all services.
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
            // When loading View Index, it checks if is currently in a Session
            if (_session.GetRespondent() != null)
                ViewData["Session"] = "true";
            else
                ViewData["Session"] = "false";
            //open Index Page
            return View();
        }

        /// <summary>
        /// Button Restart will redirect to Index page.
        /// </summary>
        /// <returns></returns>
        public IActionResult Restart()
        {
            // Clean the Session and redirect to page Index
            _session.Clear();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Button Continue will redirect to the last question in the current session.
        /// </summary>
        /// <returns></returns>
        public IActionResult Continue()
        {
            // get question from Session the users stopped
            var question = _session.GetCurrentQuestion();
            // Remove the current queston from session (It will be recreated)
            _session.RemoveCurrentQuestion();
            // follow up questions have Order null
            if (question.Order == null) // defines if the next question will be called by order or by id.
            {
                // indicate in session that the next question is follow up type.
                _session.SetIsFollowUpQuestion("true");
                // redirect to Question passing the follow up question Id.
                return RedirectToAction(nameof(Question), new { nextQuestion = question.Id });
            }
            // redirect to Question passing the regular question Order.
            return RedirectToAction(nameof(Question), new { nextQuestion = question.Order });
        }

        /// <summary>
        /// Returns the View Login passing StaffViewModel.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Login()
        {
            // Creates the staff view model that will receive the login and password inputs from admin
            var model = new StaffViewModel();
            // call Login page
            return View(model);
        }

        /// <summary>
        /// Post method to authenticate staff and redirect do Admin Page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost] // post method for Login Page
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(StaffViewModel model)
        {
            // Model state identifies if all validation criteria was met
            if (ModelState.IsValid)
            {
                // looks for existing staff in database
                Staff staff = _staffData.GetByEmailAndPassword(model.Email, model.Password);
                // if staff not found, add error message to Model State and return Login page
                if (staff == null)
                {
                    ModelState.AddModelError("", "wrong email or password");
                    return View(model);
                }
                // if staff found, it creates a new identity for cookie authentication
                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, ClaimTypes.Name, ClaimTypes.Role);
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, staff.Email));
                identity.AddClaim(new Claim(ClaimTypes.Name, staff.Email));
                var principal = new ClaimsPrincipal(identity);
                // Sing In the admin user
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties { IsPersistent = true });
                // add the admin email to session
                _session.SetAdminEmail(staff.Email);
                // redirects to Admin Page
                return RedirectToAction(nameof(Admin));
            }
            // if model state not valid, it will return Login Page displaying validation errors.
            return View();
        }

        /// <summary>
        /// Button Logout redirects to Index page.
        /// </summary>
        /// <returns></returns>
        [Authorize] // only authenticated users have access
        public IActionResult Logout()
        {
            // Executes log out for current authenticated admin
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // clear the entire session
            _session.Clear();
            // retirect to Index page
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Returns the View Register passing RegisterFirstViewModel.
        /// </summary>
        /// <returns></returns>
        [HttpGet] // get method for Register Page
        public IActionResult Register()
        {
            // if try open Register Page but a questionnaire is already in session
            if (_session.GetRespondent() != null)
            {
                // get current question from session
                var question = _session.GetCurrentQuestion();
                _session.RemoveCurrentQuestion();
                // if follow up question (order = null)
                if (question.Order == null)
                {
                    // identifies the question as follow up in session
                    _session.SetIsFollowUpQuestion("true");
                    // redirects to Question page calling follow up question by id
                    return RedirectToAction(nameof(Question), new { nextQuestion = question.Id });
                }
                // redirects to Question page calling regular question by Order
                return RedirectToAction(nameof(Question), new { nextQuestion = question.Order });
            }
            // if not in session, creates a Register model to receive the inputs from respondents
            var model = new RegisterFirstViewModel();
            // returns Register view
            return View(model);

        }

        /// <summary>
        /// Post method to store respondent information in session and
        /// redirects to first question.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost] // Post method for Register page
        [ValidateAntiForgeryToken] // security token
        public IActionResult Register(RegisterFirstViewModel model)
        {
            // if model state of Register page is valid
            if (ModelState.IsValid)
            {
                // initialize a Respondent Object as assign the values from Register page, 
                // adding date of creation and getting the user IpAddress
                Respondent respondent = new Respondent
                {
                    GivenNames = model.GivenNames,
                    LastName = model.LastName,
                    DateOfBirth = model.DateOfBirth,
                    PhoneNumber = model.PhoneNumber,
                    Date = DateTime.Now,
                    IpAddress = _ipService.GetIpAddress()
                };

                // adds the respondent to session
                _session.SetRespondent(respondent);
                // setting false to follow up question before call question 1
                _session.SetIsFollowUpQuestion("false");

                // redirects to Question page calling the first question
                return RedirectToAction(nameof(Question), new { nextQuestion = 1 });
            }
            // if model state is not valid, return Register page with validation errors
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
            // checks if is still in session before calling any question
            var respondent = _session.GetRespondent();
            if (respondent == null)
            {
                // redirects to Index page if there is no session
                return RedirectToAction(nameof(Index));
            }

            // checks if the current respondent is anonymous or registered.
            // pass to view the correct name to display in navbar.
            if (respondent.LastName == "Anonymous")
                ViewData["User"] = "Anonymous";
            else
                ViewData["User"] = respondent.GivenNames + " " + respondent.LastName;

            // get question from database. 
            //It looks for question by Order for regular questions and by Id for Follow Up questions.
            var question = _session.GetIsFollowUpQuestion() == "false" ? _questionData.GetByOrder(nextQuestion) : _questionData.GetById(nextQuestion);
            // reset the info about follow up question in session before calling next question.
            _session.SetIsFollowUpQuestion("false");
            
            // if any previou question answered have the same Id as the question loaded.
            // it will load the last question in session.
            if (_session.GetSessionQuestions().Any(q => q.Id == question.Id))
            {
                var questionInSession = _session.GetCurrentQuestion();
                _session.RemoveCurrentQuestion();
                // checks if it is follow up question (Order == null)
                if (questionInSession.Order == null)
                {
                    // identifies that next question to be called is of type follow up
                    _session.SetIsFollowUpQuestion("true");
                    // Redirects to Question page calling follow up question by Id
                    return RedirectToAction(nameof(Question), new { nextQuestion = questionInSession.Id });
                }
                // Redirects to Question page calling regular question by Order
                return RedirectToAction(nameof(Question), new { nextQuestion = questionInSession.Order });
            }

            // add the loaded question to Session
            _session.AddQuestion(question);

            // initialize question view model initializing question information from db.
            var model = new QuestionViewModel
            {
                Type = question.QuestionType.Type,
                QuestionText = question.Text,
                Order = question.Order,
                Options = question.QuestionOptions
            };

            // checks if the loaded question it was already answered with info from Session.
            // Most used when pressing the button "Previews".
            var answers = _session.GetAnswers();
            if (answers.Any(qa => qa.QuestionId == question.Id))
            {
                // add the question sequence from session to the model.
                model.QuestionSequence = answers.FirstOrDefault(a => a.QuestionId == question.Id)?.QuestionSequence;

                // identifies the question type and load the proper answered option(s) or texts to the model.
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
                // identifies the answers from user for the loaded question to remove them from session.
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
            // if no answers in session, it is the first question to load.
            else if (answers.Count == 0)
            {
                model.QuestionSequence = 1;
            }
            // if there are answers in the session, but not for the loaded question, the question sequence will be the previous + 1.
            else
            {
                model.QuestionSequence = answers[answers.Count - 1].QuestionSequence + 1;
            }

            // return Question page
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
            // checks if the there is session when Question post method is called.
            var respondent = _session.GetRespondent();
            if (respondent == null)
            {
                // Redirects to Index if there is no session.
                return RedirectToAction(nameof(Index));
            }

            // gets the current question from session
            var currentQuestion = _session.GetCurrentQuestion();

            // checks if there are invalid model states from custom states
            CheckExtraModelState(model, currentQuestion);

            // if the model state is valid with question answers
            if (ModelState.IsValid)
            {
                // if question type "radio" creates a Answer object and add to Session
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

                // if question type "text" creates a Answer object and add to Session
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

                // if question type "checkbox" creates a list of Answer object for each checked box
                // and add to session
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

                            // checks if the current selected option should bring a follow up question
                            if (option.NextQuestion != null)
                            {
                                var nextQuestion = option.NextQuestion ?? 1;
                                // brings the follow up question from db
                                var question = _questionData.GetById(nextQuestion);

                                // adds the follow up question to Session
                                var sessionFollowUpQuestions = _session.GetFollowUpQuestions();
                                if (sessionFollowUpQuestions.Count != 0)
                                {
                                    // checks if the follow up question is already in Session to not duplicate it.
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
                    // adds all the checkbox answers to Session
                    _session.AddNewAnswers(answers);
                }
                // calls the method to load the next question
                return CallNextQuestion(currentQuestion);
            }

            // if model state is invalid, recreate the model with question info to display 
            // for the user.
            model.Type = currentQuestion.QuestionType.Type;
            model.QuestionText = currentQuestion.Text;
            model.Order = currentQuestion.Order;
            model.Options = currentQuestion.QuestionOptions;

            // return the Question page with model state errors
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
                // state for radio answers needs at least one answer
                if (model.OptionRadioAnswer == null)
                {
                    ModelState.AddModelError("OptionRadioAnswer", "one option is required");
                }
            }

            if (currentQuestion.QuestionType.Type == "text")
            {
                // state for text inputs needs something typed
                if (model.TextAnswer.Equals(""))
                {
                    ModelState.AddModelError("TextAnswer", "Any answer is required");
                }
            }

            if (currentQuestion.QuestionType.Type == "checkbox")
            {
                // if question about Bank, allow max of 4 banks selected
                if (currentQuestion.Order == 7 &&
                    model.OptionCheckboxAnswers.Count(answer => answer.IsSelected) > 4)
                {
                    ModelState.AddModelError("OptionCheckboxAnswers", "Max of 4 banks");
                }

                // if question about newspaper, allows max of 2 newspapers selected
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

            // if there is follow up question
            var followUpQuestionId = followUpQuestions[0].Id;
            followUpQuestions.RemoveAt(0); // remove the next follow up question from the list
            _session.SetIsFollowUpQuestion("true"); // identifies that the next question will be the follow up type
            _session.SetFollowUpQuestions(followUpQuestions); // set the new list of follow up questions to Session
            return RedirectToAction(nameof(Question), new { nextQuestion = followUpQuestionId }); // redirects to the next question passing the follow up question Id.
        }

        /// <summary>
        /// Insert Respondent and Answers data into database. Clear the Session.
        /// </summary>
        private void UpdateAnswersInDatabase()
        {
            // get the respondent information from Session and adds to Database
            Respondent respondent = _session.GetRespondent();
            _respondentData.Add(respondent);

            // then, gets the answers from Session and assign the respondent Id to each answer
            List<AnswerViewModel> answersInSession = _session.GetAnswers();
            List<Answer> answers = new List<Answer>();
            foreach (var answerInSession in answersInSession)
            {
                var answer = answerInSession.ConvertToAnswer(respondent);
                answers.Add(answer);
            }
            // Adds the list of answers to database
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
            // checks if there is session trying to bring the admin email from session
            // if not, redirects to Logout action
            var admin = _session.GetAdminEmail();
            if (admin.Equals("session expired"))
            {
                return RedirectToAction(nameof(Logout));
            }

            // Pass the admin email to display in the admin page navbar
            ViewData["User"] = admin;

            // Start the list of respondents data to display in Admin Page
            List<Respondent> respondents = new List<Respondent>();

            // if the value passed through parameter was 0 it will get all the respondent answers from db.
            if (filtered == 0)
            {
                respondents = _respondentData.GetAll().ToList();
            }
            else // or it will load only the answers from respondents that were searched or filtered by the Admin
            {
                List<int> respondentsId = _session.GetFilteredRespondents();
                foreach (var respondentId in respondentsId)
                {
                    respondents.Add(_respondentData.Get(respondentId));
                }
                // after adding all respondent data to the list, it clears the filtered respondents in Session
                _session.ClearFilteredRespondents();
            }

            // Starts the Admin View Model to load all the filter options and
            // answers to display in Admin Page.
            var model = new AdminViewModel
            {
                Genders = _optionData.GetByQuestion(1).ToList(), // load gender options
                AgeRanges = _optionData.GetByQuestion(2).ToList(), // load age range options
                States = _optionData.GetByQuestion(3).ToList(), // load the states options
                Answers = LoadAnswerTable(respondents)  // Load the Table with answers ordered by Last name and pushing the Anonymous ones to the bottom of list.
                    .OrderBy(a => a.LastName == "Anonymous")
                    .ThenBy(a => a.LastName)
                    .ToList()
            };

            // returns the Admin view
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
            // if the model state of search is valid
            if (ModelState.IsValid)
            {
                // uses the Search method to bring all the answers that contains the query
                // in Answers table, Respondents table and Options table.
                List<Answer> matchedAnswers = _answerData.Search(model.SearchQuery).ToList();
                List<int> matchedRespondentsId = new List<int>();
                foreach (var answer in matchedAnswers)
                {
                    // adds all the respondents Id inside matched answers avoiding
                    // to repeat respondents to the list
                    if (matchedRespondentsId.All(r => r != answer.RespondentId))
                    {
                        matchedRespondentsId.Add(answer.RespondentId);
                    }
                }
                // adds the list of respondents id to Session and redirects
                // to Admin page again
                _session.SetFilteredRespondentsId(matchedRespondentsId);
                return RedirectToAction(nameof(Admin), new { filtered = 1 });
            }
            // if model state is invalid reloads Admin page
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
            // if model state is valid and at least one option to filter was selected
            if (ModelState.IsValid && model.SelectedFilters.Any(option => option != 0))
            {
                // initialize one filter list for each filter of options
                List<Answer> filterOne = new List<Answer>();
                List<Answer> filterTwo = new List<Answer>();
                List<Answer> filterThree = new List<Answer>();
                int countOne = 0;
                int countTwo = 0;
                int countThree = 0;
                bool matchCondition = true;

                // for each option selected to filter
                foreach (var option in model.SelectedFilters)
                {
                    // if different from the default option value 0
                    if (option != 0)
                    {
                        // find in db all the answers with the option id
                        var filteredAnswers = _answerData.FilterByOptionId(option).ToList();
                        if (filteredAnswers.Count > 0)
                        {
                            // checks the question Id of the options filtered and add
                            // the entire list to the right filter list
                            // Then, increments the number of times an option from the same
                            // question id was added to the filter
                            if (filteredAnswers[0].QuestionId == 1)
                            {
                                filterOne.AddRange(filteredAnswers);
                                countOne++;
                            }
                            else if (filteredAnswers[0].QuestionId == 2)
                            {
                                filterTwo.AddRange(filteredAnswers);
                                countTwo++;
                            }
                            else
                            {
                                filterThree.AddRange(filteredAnswers);
                                countThree++;
                            }
                        }
                    }
                }

                // check if there was no answer for all the options filtered in each question
                if (countOne > 0 && filterOne.Count == 0
                    || countTwo > 0 && filterTwo.Count == 0 
                    || countThree > 0 && filterThree.Count == 0)
                {
                    matchCondition = false;
                }

                List<Answer> matchedAnswers = new List<Answer>();
                if (matchCondition)
                {
                    matchedAnswers = filterOne;
                    if (matchedAnswers.Count != 0) // if this is 0 means the admin did not selected any option in filter with question id = 1
                    {
                        if (filterTwo.Count != 0) //  if the admin selected at least one option to filter from question 2
                        {
                            List<Answer> tempAnswers = new List<Answer>();
                            foreach (var filteredAnswer in filterTwo)
                            {
                                // it checks if the answer in filter two matches the same user in filter one
                                if (matchedAnswers.Any(a => a.RespondentId == filteredAnswer.RespondentId))
                                {
                                    // add the answer to the temp list
                                    tempAnswers.Add(filteredAnswer);
                                }
                            }
                            // assign the new filtered list to matchedAnswers
                            matchedAnswers = tempAnswers;
                        }
                    }
                    else //  if the admin did not select any option from question id = 1
                    {
                        matchedAnswers = filterTwo;
                    }

                    if (matchedAnswers.Count != 0) // if this is 0 means the admin did not selected any option in filter with question id = 2
                    {
                        if (filterThree.Count != 0) //  if the admin selected at least one option to filter from question 3
                        {
                            List<Answer> tempAnswers = new List<Answer>();
                            foreach (var filteredAnswer in filterThree)
                            {
                                // it checks if the answer in filter three matches the same user in filter two or one
                                if (matchedAnswers.Any(a => a.RespondentId == filteredAnswer.RespondentId))
                                {
                                    // add the answers to the temp list
                                    tempAnswers.Add(filteredAnswer);
                                }
                            }
                            // assign the new filtered list to matchedAnswers
                            matchedAnswers = tempAnswers;
                        }
                    }
                    else //  if the admin did not select any option from question id = 2
                    {
                        matchedAnswers = filterThree;
                    }
                }

                // adds to the list of ints the respondent ids from the matched answers
                // without repetition
                List<int> matchedRespondentsId = new List<int>();
                foreach (var answer in matchedAnswers)
                {
                    if (matchedRespondentsId.All(r => r != answer.RespondentId))
                    {
                        matchedRespondentsId.Add(answer.RespondentId);
                    }
                }
                // adds to the session the list of respondent ids filtered
                _session.SetFilteredRespondentsId(matchedRespondentsId);
                // redirectos to Admin page with the filtered parameter flag
                return RedirectToAction(nameof(Admin), new { filtered = 1 });
            }
            // if the model state is invalid or nothing was selected to filter
            // redirects to load the Admin page
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
            // build the table of answers with respondent information
            // and the respondent answers for each question
            List<TableRowAnswer> tableRowAnswers = new List<TableRowAnswer>();
            List<Option> questionOptions = _optionData.GetAll().ToList();
            foreach (var respondent in respondents)
            {
                // model to receive all user personal information
                var tableRowAnswer = new TableRowAnswer
                {
                    LastName = respondent.LastName,
                    GivenNames = respondent.GivenNames,
                    PhoneNumber = respondent.PhoneNumber,
                    Suburb = respondent.RespondentAnswers.Find(t => t.QuestionId == 4).Text,
                    Postcode = respondent.RespondentAnswers.Find(t => t.QuestionId == 5).Text,
                    Email = respondent.RespondentAnswers.Find(a => a.QuestionId == 6).Text
                };
                // identifies the option the respondent gave for Gender and get the proper text
                // from Options table to display at screen
                var raGenderOptionId = respondent.RespondentAnswers.Find(a => a.QuestionId == 1).OptionId;
                tableRowAnswer.Gender = questionOptions.Find(o => o.Id == raGenderOptionId).Text;
                // identifies the option the respondent gave for Age range and get the proper text
                // from Options table to display at screen
                var raAgeOptionId = respondent.RespondentAnswers.Find(a => a.QuestionId == 2).OptionId;
                tableRowAnswer.AgeRange = questionOptions.Find(o => o.Id == raAgeOptionId).Text;
                // identifies the option the respondent gave for State and get the proper text
                // from Options table to display at screen
                var raStateOptionId = respondent.RespondentAnswers.Find(a => a.QuestionId == 3).OptionId;
                tableRowAnswer.State = questionOptions.Find(o => o.Id == raStateOptionId).Text;

                // identifies the banks the respondent selected and get the proper text
                // from Options table to display at screen
                var banks = new List<Option>();
                var banksAnswers = respondent.RespondentAnswers.Where(a => a.QuestionId == 7).ToList();
                foreach (var banksAnswer in banksAnswers)
                {
                    Option option = questionOptions.Find(o => o.Id == banksAnswer.OptionId);
                    banks.Add(option);
                }

                // identifies the bank's services the respondent selected and get the proper text
                // from Options table to display at screen
                var bankServices = new List<Option>();
                var bankServicesAnswers = respondent.RespondentAnswers.Where(a => a.QuestionId == 9).ToList();
                foreach (var bankServiceAnswer in bankServicesAnswers)
                {
                    Option option = questionOptions.Find(o => o.Id == bankServiceAnswer.OptionId);
                    bankServices.Add(option);
                }

                // identifies the newspapers the respondent selected and get the proper text
                // from Options table to display at screen
                var newspapers = new List<Option>();
                var newspapersAnswers = respondent.RespondentAnswers.Where(a => a.QuestionId == 8).ToList();
                foreach (var newspapersAnswer in newspapersAnswers)
                {
                    Option option = questionOptions.Find(o => o.Id == newspapersAnswer.OptionId);
                    newspapers.Add(option);
                }

                // identifies the newspaper's sections the respondent selected and get the proper text
                // from Options table to display at screen
                var newspaperSections = new List<Option>();
                var newspaperSectionsAnswers = respondent.RespondentAnswers.Where(a => a.QuestionId == 10).ToList();
                foreach (var newspaperSectionsAnswer in newspaperSectionsAnswers)
                {
                    Option option = questionOptions.Find(o => o.Id == newspaperSectionsAnswer.OptionId);
                    newspaperSections.Add(option);
                }

                // identifies the sports the respondent selected and get the proper text
                // from Options table to display at screen
                var sportsSection = new List<Option>();
                var sportsSectionAnswers = respondent.RespondentAnswers.Where(a => a.QuestionId == 11).ToList();
                foreach (var sportsSectionAnswer in sportsSectionAnswers)
                {
                    Option option = questionOptions.Find(o => o.Id == sportsSectionAnswer.OptionId);
                    sportsSection.Add(option);
                }

                // identifies the travels the respondent selected and get the proper text
                // from Options table to display at screen
                var travelsSection = new List<Option>();
                var travelsSectionAnswers = respondent.RespondentAnswers.Where(a => a.QuestionId == 12).ToList();
                foreach (var travelsSectionAnswer in travelsSectionAnswers)
                {
                    Option option = questionOptions.Find(o => o.Id == travelsSectionAnswer.OptionId);
                    travelsSection.Add(option);
                }

                // table row expanded model will receive all respondent answers
                // from checkboxes questions
                var tableRowExpanded = new TableRowExpanded
                {
                    Banks = banks,
                    BankServices = bankServices,
                    Newspapers = newspapers,
                    NewspaperSections = newspaperSections,
                    SportsSection = sportsSection,
                    TravelsSection = travelsSection
                };

                // assign the created Table Row Expanded to the 
                // to the Table Row Answer Model
                tableRowAnswer.TableRowExpanded = tableRowExpanded;

                // adds each Table Row Answer (each respondent data and answer)
                // to the Table
                tableRowAnswers.Add(tableRowAnswer);
            }

            // return the the Table Row Answers list
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
            // if already there is a session running
            if (_session.GetRespondent() != null)
            {
                // redirects to the last question in Session to continue the questionnaire
                var question = _session.GetCurrentQuestion();
                _session.RemoveCurrentQuestion();
                if (question.Order == null) // means the last question was of type follow up
                {
                    _session.SetIsFollowUpQuestion("true");
                    // redirect to Question page passing the follow up question id
                    return RedirectToAction(nameof(Question), new { nextQuestion = question.Id });
                }
                // if not follow up redirects to Question page passin the regular question order.
                return RedirectToAction(nameof(Question), new { nextQuestion = question.Order });
            }
            // if no session active it creates a new anonymous respondent
            var respondent = new Respondent
            {
                GivenNames = "",
                LastName = "Anonymous",
                DateOfBirth = new DateTime(),
                PhoneNumber = "",
                Date = DateTime.Now,
                IpAddress = _ipService.GetIpAddress()
            };
            // Then sets the respondent to the Session
            _session.SetRespondent(respondent);
            _session.SetIsFollowUpQuestion("false");

            // and redirects to start at Question 1.
            return RedirectToAction(nameof(Question), new { nextQuestion = 1 });
        }

        /// <summary>
        /// Returns the Question View displaying the previous question and answers.
        /// </summary>
        /// <returns></returns>
        public IActionResult PreviousQuestion()
        {
            // if there is no session active, redirects to Index page
            if (_session.GetRespondent() == null)
            {
                return RedirectToAction(nameof(Index));
            }
            // Otherwise, brings the previous question from the Session
            _session.RemoveCurrentQuestion();
            Question previousQuestion = _session.GetCurrentQuestion();
            _session.RemoveCurrentQuestion();
            if (previousQuestion.Order == null) // if previsou question was follow up type
            {
                _session.SetIsFollowUpQuestion("true");
                // returns the previous question passing question id
                return RedirectToAction(nameof(Question), new { nextQuestion = previousQuestion.Id });
            }
            // if regular question, returns the previous question passing question order
            return RedirectToAction(nameof(Question), new { nextQuestion = previousQuestion.Order });
        }
    }
}

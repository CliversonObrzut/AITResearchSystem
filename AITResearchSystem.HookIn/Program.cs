using System;
using System.Collections.Generic;
using AITResearchSystem.Data;
using AITResearchSystem.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AITResearchSystem.HookIn
{
    public class Program
    {
        private static AitResearchDbContext _context;

        static void Main(/*string[] args*/)
        {
            _context = new AitResearchDbContext(CreateNewContextOptions());
            bool exit = false;
            while (!exit)
            {
                int option = 0;
                while (option != 1 && option != 2 && option != 3)
                {
                    Console.Write("Type\n - 1 - To seed database\n - 2 - to clean and reset table data\n - 3 - To exit progam\n-> ");
                    try
                    {
                        option = Int32.Parse(Console.ReadLine());
                    }
                    catch (Exception er)
                    {
                        Console.WriteLine("Something wrong happened with option selection!\nContact the software admin!\n{0}", er);
                        Console.ReadLine();
                        Environment.Exit(1);
                    }
                }

                if (option == 1)
                {
                    SeedDatabase();
                    Console.Clear();
                }
                else if (option == 2)
                {
                    CleanAndResetDatabase();
                    Console.Clear();
                }
                else
                {
                    exit = true;
                } 
            }
        }

        private static void SeedDatabase()
        {
            //seed data in tables
            SeedStaves();
            SeedQuestionType();
            SeedQuestion();
            SeedOption();
            SeedRespondent();
            SeedAnswer();
            Console.WriteLine("Database sucessfully seeded!");
            Console.WriteLine("(Enter to finish)");
            Console.ReadLine();
        }

        private static void CleanAndResetDatabase()
        {
            // delete data from tables and reset identity
            CleanAndResetAnswers();
            CleanAndResetRespondents();
            CleanAndResetOptions();
            CleanAndResetQuestions();
            CleanAndResetQuestionTypes();
            CleanAndResetStaves();
            
            Console.WriteLine("(Enter to finish)");
            Console.ReadLine();
        }

        private static void CleanAndResetQuestionTypes()
        {
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE from QuestionType");
                _context.Database.ExecuteSqlCommand("DBCC CHECKIDENT('dbo.QuestionType', RESEED, 0);");
                Console.WriteLine("Cleaned QuestionType table!");
            }
            catch (Exception er)
            {
                Console.WriteLine("\nFailed to remove all data from QuestionType table!\n\n{0}", er);
            }
        }

        private static void CleanAndResetQuestions()
        {
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE from Question");
                _context.Database.ExecuteSqlCommand("DBCC CHECKIDENT('dbo.Question', RESEED, 0);");
                Console.WriteLine("Cleaned Question table!");
            }
            catch (Exception er)
            {
                Console.WriteLine("\nFailed to remove all data from Question table!\n\n{0}", er);
            }
        }

        private static void CleanAndResetOptions()
        {
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE from QuestionOption");
                _context.Database.ExecuteSqlCommand("DBCC CHECKIDENT('dbo.QuestionOption', RESEED, 0);");
                Console.WriteLine("Cleaned QuestionOption table!");
            }
            catch (Exception er)
            {
                Console.WriteLine("\nFailed to remove all data from QuestionOption table!\n\n{0}", er);
            }
        }

        private static void CleanAndResetRespondents()
        {
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE from Respondent");
                _context.Database.ExecuteSqlCommand("DBCC CHECKIDENT('dbo.Respondent', RESEED, 0);");
                Console.WriteLine("Cleaned Respondent table!");
            }
            catch (Exception er)
            {
                Console.WriteLine("\nFailed to remove all data from Respondent table!\n\n{0}", er);
            }
        }

        private static void CleanAndResetAnswers()
        {
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE from Answer");
                _context.Database.ExecuteSqlCommand("DBCC CHECKIDENT('dbo.Answer', RESEED, 0);");
                Console.WriteLine("Cleaned Answer table!");
            }
            catch (Exception er)
            {
                Console.WriteLine("\nFailed to remove all data from Answer table!\n\n{0}", er);
            }
        }

        private static void CleanAndResetStaves()
        {
            try
            {
                _context.Database.ExecuteSqlCommand("DELETE from Staff");
                Console.WriteLine("Cleaned Staff table!");
            }
            catch (Exception er)
            {
                Console.WriteLine("\nFailed to remove all data from Staff table!\n\n{0}", er);
            }
        }

        private static void SeedAnswer()
        {
            var answers = new List<Answer>
            {
                new Answer
                {
                    Text = "",
                    QuestionId = 1,
                    OptionId = 1,
                    RespondentId = 1
                },
                new Answer
                {
                    Text = "",
                    QuestionId = 2,
                    OptionId = 8,
                    RespondentId = 1
                },
                new Answer
                {
                    Text = "",
                    QuestionId = 3,
                    OptionId = 11,
                    RespondentId = 1
                },
                new Answer
                {
                    Text = "Manly",
                    QuestionId = 4,
                    OptionId = null,
                    RespondentId = 1
                },
                new Answer
                {
                    Text = "2095",
                    QuestionId = 4,
                    OptionId = null,
                    RespondentId = 1
                },
                new Answer
                {
                    Text = "tony.abbott@gmail.com",
                    QuestionId = 5,
                    OptionId = null,
                    RespondentId = 1
                },
                new Answer
                {
                    Text = "",
                    QuestionId = 6,
                    OptionId = 21,
                    RespondentId = 1
                },
                new Answer
                {
                    Text = "",
                    QuestionId = 6,
                    OptionId = 24,
                    RespondentId = 1
                },
                new Answer
                {
                    Text = "",
                    QuestionId = 8,
                    OptionId = 36,
                    RespondentId = 1
                },
                new Answer
                {
                    Text = "",
                    QuestionId = 8,
                    OptionId = 39,
                    RespondentId = 1
                },
                new Answer
                {
                    Text = "",
                    QuestionId = 7,
                    OptionId = 27,
                    RespondentId = 1
                },
                new Answer
                {
                    Text = "",
                    QuestionId = 7,
                    OptionId = 31,
                    RespondentId = 1
                },
                new Answer
                {
                    Text = "",
                    QuestionId = 7,
                    OptionId = 34,
                    RespondentId = 1
                },
                new Answer
                {
                    Text = "",
                    QuestionId = 9,
                    OptionId = 41,
                    RespondentId = 1
                },
                new Answer
                {
                    Text = "",
                    QuestionId = 9,
                    OptionId = 42,
                    RespondentId = 1
                },
                new Answer
                {
                    Text = "",
                    QuestionId = 9,
                    OptionId = 46,
                    RespondentId = 1
                },
                new Answer
                {
                    Text = "",
                    QuestionId = 10,
                    OptionId = 47,
                    RespondentId = 1
                },
                new Answer
                {
                    Text = "",
                    QuestionId = 10,
                    OptionId = 48,
                    RespondentId = 1
                },
                new Answer
                {
                    Text = "",
                    QuestionId = 10,
                    OptionId = 49,
                    RespondentId = 1
                },
                new Answer
                {
                    Text = "",
                    QuestionId = 10,
                    OptionId = 50,
                    RespondentId = 1
                },

                // respondent 2 - anonymous
                new Answer
                {
                    Text = "",
                    QuestionId = 1,
                    OptionId = 2,
                    RespondentId = 2
                },
                new Answer
                {
                    Text = "",
                    QuestionId = 2,
                    OptionId = 6,
                    RespondentId = 2
                },
                new Answer
                {
                    Text = "",
                    QuestionId = 3,
                    OptionId = 12,
                    RespondentId = 2
                },
                new Answer
                {
                    Text = "Ascot",
                    QuestionId = 4,
                    OptionId = null,
                    RespondentId = 2
                },
                new Answer
                {
                    Text = "4007",
                    QuestionId = 4,
                    OptionId = null,
                    RespondentId = 2
                },
                new Answer
                {
                    Text = "a.smith@gmail.com",
                    QuestionId = 5,
                    OptionId = null,
                    RespondentId = 2
                },
                new Answer
                {
                    Text = "",
                    QuestionId = 6,
                    OptionId = 20,
                    RespondentId = 2
                },
                new Answer
                {
                    Text = "",
                    QuestionId = 6,
                    OptionId = 22,
                    RespondentId = 2
                },
                new Answer
                {
                    Text = "",
                    QuestionId = 8,
                    OptionId = 36,
                    RespondentId = 2
                },
                new Answer
                {
                    Text = "",
                    QuestionId = 8,
                    OptionId = 37,
                    RespondentId = 2
                },
                new Answer
                {
                    Text = "",
                    QuestionId = 7,
                    OptionId = 27,
                    RespondentId = 2
                },
                new Answer
                {
                    Text = "",
                    QuestionId = 9,
                    OptionId = 43,
                    RespondentId = 2
                },
                new Answer
                {
                    Text = "",
                    QuestionId = 9,
                    OptionId = 44,
                    RespondentId = 2
                },
                new Answer
                {
                    Text = "",
                    QuestionId = 9,
                    OptionId = 45,
                    RespondentId = 2
                },
                new Answer
                {
                    Text = "",
                    QuestionId = 11,
                    OptionId = 56,
                    RespondentId = 2
                },
                new Answer
                {
                    Text = "",
                    QuestionId = 11,
                    OptionId = 58,
                    RespondentId = 2
                },
                new Answer
                {
                    Text = "",
                    QuestionId = 9,
                    OptionId = 59,
                    RespondentId = 2
                },
            };
            try
            {
                foreach (var answer in answers)
                {
                    _context.Answers.Add(answer);
                    _context.SaveChanges();
                }
                //_context.Answers.AddRange(answers);
                //_context.SaveChanges();
                Console.WriteLine("Seeded Answer table!");
            }
            catch (Exception er)
            {
                Console.WriteLine("\nFailed to Seed Answers!\n\n{0}", er);
            }
        }

        private static void SeedRespondent()
        {
            var respondents = new List<Respondent>
            {
                new Respondent // id = 1
                {
                    GivenNames = "Tony",
                    LastName = "Abbott",
                    DateOfBirth = DateTime.Parse("1957-11-04"),
                    PhoneNumber = "0299776411",
                    IpAddress = "192.168.234.99",
                    Date = DateTime.Parse("2018-05-02")
                },
                new Respondent // id = 2
                {
                    GivenNames = "Anonymous",
                    LastName = "",
                    DateOfBirth = new DateTime(),
                    PhoneNumber = "",
                    IpAddress = "192.168.251.198",
                    Date = DateTime.Parse("2018-05-04")
                }
            };
            try
            {
                _context.Respondents.AddRange(respondents);
                _context.SaveChanges();
                Console.WriteLine("Seeded Respondent table!");
            }
            catch (Exception er)
            {
                Console.WriteLine("\nFailed to Seed Respondents!\n\n{0}", er);
            }
        }

        private static void SeedOption()
        {
            var options = new List<Option>
            {
                // Gender (1 to 2)
                new Option
                {
                    Text = "Male",
                    NextQuestion = 2,
                    QuestionId = 1
                },
                new Option
                {
                    Text = "Female",
                    NextQuestion = 2,
                    QuestionId = 1
                },

                // Age range (3 to 10)
                new Option
                {
                    Text = "Under 18 years old",
                    NextQuestion = 3,
                    QuestionId = 2
                },
                new Option
                {
                    Text = "18 - 24 years old",
                    NextQuestion = 3,
                    QuestionId = 2
                },
                new Option
                {
                    Text = "25 - 34 years old",
                    NextQuestion = 3,
                    QuestionId = 2
                },
                new Option
                {
                    Text = "35 - 44 years old",
                    NextQuestion = 3,
                    QuestionId = 2
                },
                new Option
                {
                    Text = "45 - 54 years old",
                    NextQuestion = 3,
                    QuestionId = 2
                },
                new Option
                {
                    Text = "55 - 64 years old",
                    NextQuestion = 3,
                    QuestionId = 2
                },
                new Option
                {
                    Text = "65 - 74 years old",
                    NextQuestion = 3,
                    QuestionId = 2
                },
                new Option
                {
                    Text = "75 years or older",
                    NextQuestion = 3,
                    QuestionId = 2
                },

                // Australian States and Territories (11 to 19)
                new Option
                {
                    Text = "New South Wales (NSW)",
                    NextQuestion = 4,
                    QuestionId = 3
                },
                new Option
                {
                    Text = "Queensland (QLD)",
                    NextQuestion = 4,
                    QuestionId = 3
                },
                new Option
                {
                    Text = "South Australia (SA)",
                    NextQuestion = 4,
                    QuestionId = 3
                },
                new Option
                {
                    Text = "Tasmania (TAS)",
                    NextQuestion = 4,
                    QuestionId = 3
                },
                new Option
                {
                    Text = "Victoria (VIC)",
                    NextQuestion = 4,
                    QuestionId = 3
                },
                new Option
                {
                    Text = "Western Australia (WA)",
                    NextQuestion = 4,
                    QuestionId = 3
                },
                new Option
                {
                    Text = "Australia Capital Territory (ACT)",
                    NextQuestion = 4,
                    QuestionId = 3
                },
                new Option
                {
                    Text = "Jervis Bay Territory (JBT)",
                    NextQuestion = 4,
                    QuestionId = 3
                },
                new Option
                {
                    Text = "Northern Territory (NT)",
                    NextQuestion = 4,
                    QuestionId = 3
                },

                // Australian Banks (20 to 25)
                new Option
                {
                    Text = "Commonwealth",
                    NextQuestion = 8,
                    QuestionId = 6
                },
                new Option
                {
                    Text = "ANZ",
                    NextQuestion = 8,
                    QuestionId = 6
                },
                new Option
                {
                    Text = "Westpac",
                    NextQuestion = 8,
                    QuestionId = 6
                },
                new Option
                {
                    Text = "Bendigo",
                    NextQuestion = 7,
                    QuestionId = 6
                },
                new Option
                {
                    Text = "St George",
                    NextQuestion = 7,
                    QuestionId = 6
                },
                new Option
                {
                    Text = "Suncorp",
                    NextQuestion = 7,
                    QuestionId = 6
                },

                // Australian newspapers (26 to 35)
                new Option
                {
                    Text = "Herald Sun",
                    NextQuestion = 9,
                    QuestionId = 7
                },
                new Option
                {
                    Text = "Daily/Sunday Telegraph",
                    NextQuestion = 9,
                    QuestionId = 7
                },
                new Option
                {
                    Text = "Courier Mail",
                    NextQuestion = 9,
                    QuestionId = 7
                },
                new Option
                {
                    Text = "West Australian",
                    NextQuestion = 9,
                    QuestionId = 7
                },
                new Option
                {
                    Text = "Advertiser",
                    NextQuestion = 9,
                    QuestionId = 7
                },
                new Option
                {
                    Text = "Sydney Morning Herald",
                    NextQuestion = 9,
                    QuestionId = 7
                },
                new Option
                {
                    Text = "Age",
                    NextQuestion = 9,
                    QuestionId = 7
                },
                new Option
                {
                    Text = "Australian",
                    NextQuestion = 9,
                    QuestionId = 7
                },
                new Option
                {
                    Text = "Canberra Times",
                    NextQuestion = 9,
                    QuestionId = 7
                },
                new Option
                {
                    Text = "Northern Territory News",
                    NextQuestion = 9,
                    QuestionId = 7
                },

                // Bank Services (36 to 39)
                new Option
                {
                    Text = "Internet Banking",
                    NextQuestion = 7,
                    QuestionId = 8
                },
                new Option
                {
                    Text = "Home Loan",
                    NextQuestion = 7,
                    QuestionId = 8
                },
                new Option
                {
                    Text = "Credit Card",
                    NextQuestion = 7,
                    QuestionId = 8
                },
                new Option
                {
                    Text = "Share Investment",
                    NextQuestion = 7,
                    QuestionId = 8
                },

                // Newspaper Sections (40 to 46)
                new Option
                {
                    Text = "Property",
                    NextQuestion = null,
                    QuestionId = 9
                },
                new Option
                {
                    Text = "Sport",
                    NextQuestion = 10,
                    QuestionId = 9
                },
                new Option
                {
                    Text = "Financial",
                    NextQuestion = null,
                    QuestionId = 9
                },
                new Option
                {
                    Text = "Entertainment",
                    NextQuestion = null,
                    QuestionId = 9
                },
                new Option
                {
                    Text = "Lifestyle",
                    NextQuestion = null,
                    QuestionId = 9
                },
                new Option
                {
                    Text = "Travel",
                    NextQuestion = 11,
                    QuestionId = 9
                },
                new Option
                {
                    Text = "Politics",
                    NextQuestion = null,
                    QuestionId = 9
                },

                // Newspaper sports section (47 to 54)
                new Option
                {
                    Text = "AFL",
                    NextQuestion = null,
                    QuestionId = 10
                },
                new Option
                {
                    Text = "Rugby",
                    NextQuestion = null,
                    QuestionId = 10
                },
                new Option
                {
                    Text = "Football",
                    NextQuestion = null,
                    QuestionId = 10
                },
                new Option
                {
                    Text = "Cricket",
                    NextQuestion = null,
                    QuestionId = 10
                },
                new Option
                {
                    Text = "Racing",
                    NextQuestion = null,
                    QuestionId = 10
                },
                new Option
                {
                    Text = "Motorsport",
                    NextQuestion = null,
                    QuestionId = 10
                },
                new Option
                {
                    Text = "Basketball",
                    NextQuestion = null,
                    QuestionId = 10
                },
                new Option
                {
                    Text = "Tennis",
                    NextQuestion = null,
                    QuestionId = 10
                },

                // Newspaper Travel section (55 to 62)
                new Option
                {
                    Text = "Australia",
                    NextQuestion = null,
                    QuestionId = 11
                },
                new Option
                {
                    Text = "Europe",
                    NextQuestion = null,
                    QuestionId = 11
                },
                new Option
                {
                    Text = "Pacific",
                    NextQuestion = null,
                    QuestionId = 11
                },
                new Option
                {
                    Text = "North America",
                    NextQuestion = null,
                    QuestionId = 11
                },
                new Option
                {
                    Text = "South America",
                    NextQuestion = null,
                    QuestionId = 11
                },
                new Option
                {
                    Text = "Asia",
                    NextQuestion = null,
                    QuestionId = 11
                },
                new Option
                {
                    Text = "Middle East",
                    NextQuestion = null,
                    QuestionId = 11
                },
                new Option
                {
                    Text = "Africa",
                    NextQuestion = null,
                    QuestionId = 11
                }
            };
            try
            {
                foreach (var option in options)
                {
                    _context.Options.Add(option);
                    _context.SaveChanges();
                }
                //_context.Options.AddRange(options);
                //_context.SaveChanges();
                Console.WriteLine("Seeded QuestionOption table!");
            }
            catch (Exception er)
            {
                Console.WriteLine("\nFailed to Seed QuestionOption!\n\n{0}", er);
            }
        }

        private static void SeedQuestion()
        {
            var questions = new List<Question>
            {
                new Question
                {
                    Text = "What is your gender?", // id = 1
                    Order = 1,
                    QuestionTypeId = 3 // radio
                },
                new Question
                {
                    Text = "What is your age range?", // id = 2
                    Order = 2,
                    QuestionTypeId = 3 // radio
                },
                new Question
                {
                    Text = "What is your State/Territory of Australia?", // id = 3
                    Order = 3,
                    QuestionTypeId = 3 // radio
                },
                new Question
                {
                    Text = "What is your Home Suburb and Post Code?", // id = 4
                    Order = 4,
                    QuestionTypeId = 1 // text
                },
                new Question
                {
                    Text = "What is your email address?", // id = 5
                    Order = 5,
                    QuestionTypeId = 1 // text
                },
                new Question
                {
                    Text = "What bank do you use? (max of 4)", // id = 6
                    Order = 6,
                    QuestionTypeId = 2 // checkbox
                },
                new Question
                {
                    Text = "What newspaper do you usually read? (max of 2)", // id = 7
                    Order = 7,
                    QuestionTypeId = 2 // checkbox
                },
                new Question
                {
                    Text = "What bank services do you use?", // id = 8
                    Order = null,
                    QuestionTypeId = 2 // checkbox
                },
                new Question
                {
                    Text = "In the newspaper, what are your most interested news section?", // id = 9
                    Order = null,
                    QuestionTypeId = 2 // checkbox
                },
                new Question
                {
                    Text = "About what sports do you most read in the sports section?", // id = 10
                    Order = null,
                    QuestionTypeId = 2 // checkbox
                },
                new Question
                {
                    Text = "About what places do you like to read in the travel section?", // id = 11
                    Order = null,
                    QuestionTypeId = 2 // checkbox
                }
            };
            try
            {
                _context.Questions.AddRange(questions);
                _context.SaveChanges();
                Console.WriteLine("Seeded Question table!");
            }
            catch (Exception er)
            {
                Console.WriteLine("\nFailed to Seed Question!\n\n{0}", er);
            }
        }

        private static void SeedQuestionType()
        {
            var questionTypes = new List<QuestionType>
            {
                new QuestionType
                {
                    Type = "text" // id = 1
                },
                new QuestionType
                {
                    Type = "checkbox" // id = 2
                },
                new QuestionType
                {
                    Type = "radio" // id = 3
                },
            };
            try
            {
                _context.QuestionTypes.AddRange(questionTypes);
                _context.SaveChanges();
                Console.WriteLine("Seeded QuestionType table!");
            }
            catch (Exception er)
            {
                Console.WriteLine("\nFailed to Seed QuestionTypes!\n\n{0}", er);
            }
        }

        private static void SeedStaves()
        {
            var staves = new List<Staff>
            {
                // Default user admin
                new Staff()
                {
                    Email = "admin@aitr.com",
                    Password = "password123"
                }
            };
            try
            {
                _context.Staves.AddRange(staves);
                _context.SaveChanges();
                Console.WriteLine("Seeded Staff table!");
            }
            catch (Exception er)
            {
                Console.WriteLine("\nFailed to Seed Staff!\n\n{0}", er);
            }
        }


        private static DbContextOptions<AitResearchDbContext> CreateNewContextOptions()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkSqlServer()
                .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<AitResearchDbContext>();
            builder.UseSqlServer("Server = SQL5031.site4now.net; MultipleActiveResultSets = true; Initial Catalog = DB_9AB8B7_B18DDA6204; User Id = DB_9AB8B7_B18DDA6204_admin; Password = 2B8bSpSe")
                .UseInternalServiceProvider(serviceProvider);

            return builder.Options;
        }
    }
}

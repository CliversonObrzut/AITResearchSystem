using AITResearchSystem.Data.Models;

namespace AITResearchSystem.ViewModels
{
    public class AnswerViewModel
    {
        public int? QuestionSequence { get; set; }

        public string Suburb { get; set; }

        public string Postcode { get; set; }

        public string Email { get; set; }

        public int? OptionId { get; set; }

        public int QuestionId { get; set; }

        public Answer ConvertToAnswer(Respondent respondent)
        {
            Answer answer = new Answer
            {
                OptionId = OptionId,
                QuestionId = QuestionId,
                RespondentId = respondent.Id,
                Text = ""
            };

            if (!Suburb.Equals(""))
            {
                answer.Text = Suburb;
            }

            if (!Postcode.Equals(""))
            {
                answer.Text = Postcode;
            }

            if (!Email.Equals(""))
            {
                answer.Text = Email;
            }

            return answer;
        }
    }
}

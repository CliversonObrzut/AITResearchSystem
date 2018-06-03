using AITResearchSystem.Data.Models;

namespace AITResearchSystem.ViewModels
{
    public class AnswerViewModel
    {
        public int? QuestionSequence { get; set; }
        public string Text { get; set; }
        public int? OptionId { get; set; }
        public int QuestionId { get; set; }

        /// <summary>
        /// Convert a AnswerViewModel to Answer.
        /// </summary>
        /// <param name="respondent"></param>
        /// <returns></returns>
        public Answer ConvertToAnswer(Respondent respondent)
        {
            Answer answer = new Answer
            {
                OptionId = OptionId,
                QuestionId = QuestionId,
                RespondentId = respondent.Id,
                Text = Text
            };

            return answer;
        }
    }
}

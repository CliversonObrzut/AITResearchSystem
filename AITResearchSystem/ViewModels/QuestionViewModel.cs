using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AITResearchSystem.Data.Models;

namespace AITResearchSystem.ViewModels
{
    public class QuestionViewModel
    {
        public string QuestionText { get; set; }

        public string Type { get; set; }

        public int? Order { get; set; }

        public int? QuestionSequence { get; set; }

        public string OptionRadioAnswer { get; set; }

        public List<CheckboxViewModel> OptionCheckboxAnswers { get; set; }

        [StringLength(255, ErrorMessage = "Limit is 255 characteres")]
        public string Suburb { get; set; }

        [StringLength(255, ErrorMessage = "Limit is 255 characteres")]
        [DataType(DataType.PostalCode,ErrorMessage = "It is not a valid post code")]
        public string PostCode { get; set; }

        [StringLength(255, ErrorMessage = "Limit is 255 characteres")]
        [DataType(DataType.EmailAddress, ErrorMessage = "It is not a valid email address")]
        public string Email { get; set; }

        public List<Option> Options { get; set; }

        public string QuestionNumber => string.Format("Q{0}.",QuestionSequence);
    }
}

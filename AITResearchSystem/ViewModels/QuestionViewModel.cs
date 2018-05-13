using System;
using System.Collections.Generic;
using AITResearchSystem.Data.Models;

namespace AITResearchSystem.ViewModels
{
    public class QuestionViewModel
    {
        public string Text { get; set; }

        public int? Order { get; set; }

        public string Type { get; set; }

        public int QuestionSequence { get; set; }

        public List<Option> Options { get; set; }

        public string QuestionNumber => String.Format("Q{0}.",Order);
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AITResearchSystem.Data.Models
{
    [Table("Question")]
    public class Question
    {
        // Primary Key
        [Key]
        public int Id{ get; set; }

        // Fields
        [Column(TypeName = "varchar(255)")]
        public string Text { get; set; }

        public int? Order { get; set; }

        // Foreign Key
        public int QuestionTypeId { get; set; }

        // Table Relationship
        public QuestionType QuestionType { get; set; }
        public List<Option> QuestionOptions { get; set; }
        public List<Answer> QuestionAnswers { get; set; }
    }
}

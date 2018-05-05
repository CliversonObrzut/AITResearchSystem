using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace AITResearchSystem.Data.Models
{
    [Table("Question")]
    public class Question
    {
        // Primary Key
        [Key]
        [Column(TypeName = "integer(10)")]
        public int Id{ get; set; }

        // Fields
        [Column(TypeName = "varchar(255)")]
        public string Text { get; set; }

        [Column(TypeName = "integer(10)")]
        public int? Order { get; set; }

        // Foreign Key
        public int QuestionTypeId { get; set; }

        // Table Relationship
        public QuestionType QuestionType { get; set; }
        public IQueryable<Option> QuestionOptions { get; set; }
        public IQueryable<Answer> QuestionAnswers { get; set; }
    }
}

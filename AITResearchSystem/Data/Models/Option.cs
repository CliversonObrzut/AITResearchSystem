using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AITResearchSystem.Data.Models
{
    [Table("QuestionOption")]
    public class Option
    {
        // Primary Key
        [Key]
        public int Id { get; set; }

        // Fields
        [Required]
        [Column(TypeName = "varchar(255)")]
        public string Text { get; set; }

        public int? NextQuestion { get; set; }

        // Foreign Key
        public int QuestionId { get; set; }

        // Table Relationship
        public Question Question { get; set; }
        public List<Answer> OptionAnswers { get; set; }

    }
}

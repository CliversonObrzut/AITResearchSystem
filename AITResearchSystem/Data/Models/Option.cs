using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AITResearchSystem.Data.Models
{
    public class Option
    {
        //Table particular data
        [Key]
        [Column(TypeName = "integer(10)")]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "varchar(255)")]
        public string Text { get; set; }

        [Column(TypeName = "integer(10)")]
        public int? NextQuestion { get; set; }

        // Table Foreign Keys
        public int QuestionId { get; set; }

        // Table Relationship
        public Question Question { get; set; }
        public List<Answer> OptionAnswers { get; set; }

    }
}

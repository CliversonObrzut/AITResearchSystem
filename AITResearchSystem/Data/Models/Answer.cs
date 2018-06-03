using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace AITResearchSystem.Data.Models
{
    [Table("Answer")]
    public class Answer
    {
        //Primary Key
        [Key]
        public int Id { get; set; }
        
        // Fields
        [Column(TypeName = "varchar(255)")]
        public string Text { get; set; }

        // Foreign Keys
        public int QuestionId { get; set; }
        public int? OptionId { get; set; }
        public int RespondentId { get; set; }

        // Table Relationship
        public Question Question { get; set; }
        public Option Option { get; set; }
        public Respondent Respondent { get; set; }
    }
}

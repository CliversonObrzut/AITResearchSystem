using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace AITResearchSystem.Data.Models
{
    [Table("QuestionType")]
    public class QuestionType
    {
        // Primary Key
        [Key]
        public int Id { get; set; }

        // Fields
        [Column(TypeName = "varchar(255)")]
        public string Type { get; set; }

        // Table Relationship
        public IQueryable<Question> QuestionTypeQuestions { get; set; }
    }
}

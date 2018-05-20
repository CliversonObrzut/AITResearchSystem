using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        [JsonIgnore]
        public List<Question> QuestionTypeQuestions { get; set; }
    }
}

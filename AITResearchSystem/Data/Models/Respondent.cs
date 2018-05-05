using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace AITResearchSystem.Data.Models
{
    [Table("Respondent")]
    public class Respondent
    {
        //Primary key
        [Key]
        public int Id { get; set; }

        // Fields
        [Required]
        [Column(TypeName = "varchar(100)")]
        public string GivenNames { get; set; }

        [Column(TypeName = "varchar(50)")]
        public string LastName { get; set; }

        [Column(TypeName = "date")]
        public DateTime DateOfBirth { get; set; }

        [Column(TypeName = "varchar(15)")]
        public string PhoneNumber { get; set; }

        [Required]
        [Column(TypeName = "varchar(15)")]
        public string IpAddress { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        // Table Relationship
        public IQueryable<Answer> RespondentAnswers { get; set; }
    }
}

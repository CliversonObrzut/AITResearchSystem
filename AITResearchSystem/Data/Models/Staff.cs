using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AITResearchSystem.Data.Models
{
    [Table("Staff")]
    public class Staff
    {
        // Primary Key
        [Key]
        [Column(TypeName = "varchar(100)")]
        public string Email { get; set; }

        // Fields
        [Required]
        [Column(TypeName = "varchar(100)")]
        [MinLength(4)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}

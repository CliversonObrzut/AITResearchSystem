using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using AITResearchSystem.Services;

namespace AITResearchSystem.ViewModels
{
    public class RegisterFirstViewModel
    {
        [Required(ErrorMessage = "Given names are required")]
        [DisplayName("Given Names")]
        [StringLength(100,ErrorMessage = "The given names can not be larger than 100 characteres")]
        public string GivenNames { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [DisplayName("Last Name")]
        [StringLength(50, ErrorMessage = "The last name can not be larger than 50 characteres")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        [DisplayName("Date of Birth")]
        [DataType(DataType.Date, ErrorMessage = "The DoB must be a date")]
        [MyDateRange(ErrorMessage = "DoB must be between 01/01/1900 and today")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Phone Number is required")]
        [DisplayName("Phone Number")]
        [DataType(DataType.PhoneNumber, ErrorMessage = "It must be a valid phone number")]
        [StringLength(15, ErrorMessage = "Phone number can not be larger than 15 digits")]
        public string PhoneNumber { get; set; }
    }
}

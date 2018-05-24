using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AITResearchSystem.ViewModels
{
    public class AdminViewModel
    {
        public List<DropdownOption> Genders { get; set; }
        public List<DropdownOption> AgeRanges { get; set; }
        public List<DropdownOption> States { get; set; }
        public string SearchQuery { get; set; }
        public List<DropdownOption> SelectedFilters { get; set; }
        public List<TableRowAnswer> Answers { get; set; }

    }

    public class DropdownOption
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsSelected { get; set; }
    }

    public class TableRowAnswer
    {
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Given Names")]
        public string GivenNames { get; set; }
        [Display(Name = "Gender")]
        public string Gender { get; set; }
        [Display(Name = "Age Range")]
        public string AgeRange { get; set; }
        [Display(Name = "State")]
        public string State { get; set; }
        [Display(Name = "Suburb")]
        public string Suburb { get; set; }
        [Display(Name = "Post Code")]
        public string Postcode { get; set; }
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}

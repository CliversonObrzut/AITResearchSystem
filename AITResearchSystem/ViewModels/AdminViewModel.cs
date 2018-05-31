using AITResearchSystem.Data.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AITResearchSystem.ViewModels
{
    public class AdminViewModel
    {
        [Display(Name = "Genders")]
        public List<SelectListOption> Genders { get; set; }
        [Display(Name = "Age Ranges")]
        public List<SelectListOption> AgeRanges { get; set; }
        [Display(Name = "States")]
        public List<SelectListOption> States { get; set; }
        public string SearchQuery { get; set; }
        public int [] SelectedFilters { get; set; }
        public List<TableRowAnswer> Answers { get; set; }
    }

    public class SelectListOption
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public bool IsSelected { get; set; }
    }

    public class TableRowAnswer
    {
        [Display(Name = "Last Name")] public string LastName { get; set; }
        [Display(Name = "Given Names")] public string GivenNames { get; set; }
        [Display(Name = "Gender")] public string Gender { get; set; }
        [Display(Name = "Age Range")] public string AgeRange { get; set; }
        [Display(Name = "State")] public string State { get; set; }
        [Display(Name = "Suburb")] public string Suburb { get; set; }
        [Display(Name = "Post Code")] public string Postcode { get; set; }
        [Display(Name = "Phone Number")] public string PhoneNumber { get; set; }
        [Display(Name = "Email")] public string Email { get; set; }

        public bool Expanded { get; set; }

        public TableRowExpanded TableRowExpanded { get; set; }

        public void InvertExpand()
        {
            Expanded = !Expanded;
        }
    }

    public class TableRowExpanded
    {
        [Display(Name = "Banks")] public List<Option> Banks { get; set; }
        [Display(Name = "Bank Services")] public List<Option> BankServices { get; set; }
        [Display(Name = "Newspapers")] public List<Option> Newspapers { get; set; }
        [Display(Name = "Newspaper Sections")] public List<Option> NewspaperSections { get; set; }
        [Display(Name = "Sports Section")] public List<Option> SportsSection { get; set; }
        [Display(Name = "Travels Section")] public List<Option> TravelsSection { get; set; }
    }
}
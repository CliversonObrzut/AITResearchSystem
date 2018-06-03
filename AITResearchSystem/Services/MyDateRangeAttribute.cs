using System;
using System.ComponentModel.DataAnnotations;

namespace AITResearchSystem.Services
{
    public class MyDateRangeAttribute : RangeAttribute
    {
        // Custom Date Range to use as annotation during respondent
        // registration of DOB. Used for validation.
        public MyDateRangeAttribute() :
            base(typeof(DateTime), "01/01/1900", DateTime.Now.ToShortDateString())
        {

        }
    }
}

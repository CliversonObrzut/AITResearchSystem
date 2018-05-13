using System;
using System.ComponentModel.DataAnnotations;

namespace AITResearchSystem.Services
{
    public class MyDateRangeAttribute : RangeAttribute
    {
        public MyDateRangeAttribute() :
            base(typeof(DateTime), "01/01/1900", DateTime.Now.ToShortDateString())
        {

        }
    }
}

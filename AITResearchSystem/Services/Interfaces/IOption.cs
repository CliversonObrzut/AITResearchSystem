using System.Collections.Generic;
using AITResearchSystem.Data.Models;

namespace AITResearchSystem.Services.Interfaces
{
    interface IOption
    {
        IEnumerable<Option> GetAll();
        Option Get(int id);
    }
}
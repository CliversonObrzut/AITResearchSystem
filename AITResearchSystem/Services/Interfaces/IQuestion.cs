using System.Collections.Generic;
using AITResearchSystem.Data.Models;

namespace AITResearchSystem.Services.Interfaces
{
    interface IQuestion
    {
        IEnumerable<Question> GetAll();
        Question Get(int id);
    }
}

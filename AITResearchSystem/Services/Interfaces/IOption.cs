using System.Collections.Generic;
using AITResearchSystem.Data.Models;

namespace AITResearchSystem.Services.Interfaces
{
    public interface IOption
    {
        IEnumerable<Option> GetAll();
        IEnumerable<Option> GetByQuestion(int questionId);
    }
}
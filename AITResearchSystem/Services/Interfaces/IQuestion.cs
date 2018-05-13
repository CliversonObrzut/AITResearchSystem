using System.Collections.Generic;
using AITResearchSystem.Data.Models;

namespace AITResearchSystem.Services.Interfaces
{
    public interface IQuestion
    {
        IEnumerable<Question> GetAll();
        Question GetById(int id);
        Question GetByOrder(int order);
    }
}

using System.Collections.Generic;
using AITResearchSystem.Data.Models;

namespace AITResearchSystem.Services.Interfaces
{
    public interface IRespondent
    {
        IEnumerable<Respondent> GetAll();
        Respondent Get(int id);
        void Add(Respondent respondent);
    }
}

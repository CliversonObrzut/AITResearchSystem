using System.Collections;
using System.Collections.Generic;
using AITResearchSystem.Data.Models;

namespace AITResearchSystem.Services.Interfaces
{
    public interface IAnswer
    {
        IEnumerable GetAll();
        Answer Get(int id);
        void Add(Answer answer);
        void AddRange(List<Answer> answers);
        IEnumerable<Answer> FilterByGenre(string genre);
        IEnumerable<Answer> FilterByAgeRange(string ageRange);
        IEnumerable<Answer> FilterByState(string state);
        IEnumerable<Answer> Search(string query);
    }
}

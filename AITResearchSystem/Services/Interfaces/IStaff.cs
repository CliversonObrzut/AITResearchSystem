using AITResearchSystem.Data.Models;

namespace AITResearchSystem.Services.Interfaces
{
    public interface IStaff
    {
        Staff GetByEmailAndPassword(string email, string password);
    }
}

using System.Threading.Tasks;

namespace AITResearchSystem.Services.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}

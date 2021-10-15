using System.Threading.Tasks;

namespace masz.Services
{
    public interface IAuditLogger
    {
        void RegisterEvents();
        void Startup();
        void QueueLog(string message);
        Task ExecuteWebhook();
    }
}

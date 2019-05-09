using System.Threading.Tasks;
using Techies.Clients.DTOs.Request;
using Techies.Clients.DTOs.Responses;

namespace Techies.Clients.ApplicationServices.Abstract
{
    public interface IClientApplicationService
    {
        Task<OperationResult<string>> Register(RegisterClient client);
    }
}

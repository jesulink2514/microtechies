using System.Threading.Tasks;
using Techies.Client.Stats.Api.Model;

namespace Techies.Client.Stats.Api.Application
{
    public interface IClientApplicationService
    {
        Task<ClientStatsResponse> CalculateStats();
        Task<ClientModel[]> ListClients(string search = null, int page = 1, int pageSize = 500);
        Task<ClientModel> GetClientById(string id);
        Task IndexNewClient(NewClient client);
    }
}
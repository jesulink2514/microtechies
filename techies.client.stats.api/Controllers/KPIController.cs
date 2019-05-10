using System.Threading.Tasks;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Techies.Client.Stats.Api.Application;
using Techies.Client.Stats.Api.Model;

namespace Techies.Client.Stats.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KPIController : Controller
    {
        private readonly IClientApplicationService _client;

        public KPIController(IClientApplicationService clientApplicationService)
        {
            _client = clientApplicationService;
        }
        
        [Route("stats")]
        public async Task<IActionResult> Index()
        {
            var stats = await _client.CalculateStats();
            return Ok(stats);
        }

        [CapSubscribe("client.services.registered"),NonAction]
        public async Task OnNewClientRegistered(NewClient client)
        {
            await _client.IndexNewClient(client);
        }
    }
}
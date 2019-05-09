using System;
using System.Threading.Tasks;
using DotNetCore.CAP;
using Microsoft.AspNetCore.Mvc;
using Nest;
using Techies.Client.Stats.Api.Model;

namespace Techies.Client.Stats.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KPIController : Controller
    {
        private readonly ElasticClient _client;

        public KPIController(ElasticClient elasticClient)
        {
            _client = elasticClient;
        }
        
        [Route("stats")]
        public async Task<IActionResult> Index()
        {
            var currentTime = DateTime.UtcNow.Year;

            const string ageScript = "params['currentTime'] - doc['birthdateYear'].value";

            var datas = await _client.SearchAsync<ClientModel>(s => s.Index("clientsmodel")
            .MatchAll()
            .Aggregations(a => a.ExtendedStats("stats",d => d.Script(sf => sf.Source(ageScript)
                     .Params(sp => sp.Add("currentTime", currentTime))))));

            var stats = (ExtendedStatsAggregate)datas.Aggregations["stats"];

            var response = new ClientStatsResponse()
            {
                AverageAge = stats.Average ?? 0,
                StdDeviationAge = stats.StdDeviation ?? 0
            };

            return Ok(response);
        }

        [CapSubscribe("client.services.registered"),NonAction]
        public async Task IndexingClient(NewClient client)
        {
            if(!(await _client.IndexExistsAsync("clientsmodel")).Exists)
            {
                var response = await _client.CreateIndexAsync("clientsmodel", c => c.Mappings(m => m.Map<ClientModel>(mp => mp.AutoMap())));
                if (!response.IsValid)
                {
                    throw response.OriginalException;
                }
            }

            var indexedClient = new ClientModel()
            {
                Id = client.Id,
                Birthdate = client.Birthdate,
                BirthdateYear = client.Birthdate.Year,
                ProbablyDeathDate = client.Birthdate.AddYears(75),
                FirstName = client.FirstName,
                LastName = client.LastName,
                CreatedBy = client.CreatedBy,
                CreationDate = client.CreationDate,
                UpdateDate = client.UpdateDate,
                UpdatedBy = client.UpdatedBy
            };

            var result = await _client.IndexAsync(indexedClient,id=> id.Index("clientsmodel"));

            if(!result.IsValid) throw result.OriginalException;
        }
    }
}
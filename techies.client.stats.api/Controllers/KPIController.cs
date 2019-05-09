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
        private readonly ElasticClient _elasticClient;

        public KPIController(ElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }
        public async Task<IActionResult> Index()
        {
            var sumClients = await _elasticClient.SearchAsync<ClientModel>
                (c=> c.Aggregations(a=> a.Sum("sum_ages",m=>m.Field(o=>o.BirthdateYear))));

            //var sums = await _elasticClient.SearchAsync<ClientModel>(c=> 
            //c.ScriptFields(sf=> sf.ScriptField("age",sff=> sff.Source("doc['age'].value - 2019"))));                    

            var totalClients = (await _elasticClient.CountAsync<ClientModel>()).Count;

            if(totalClients == 0) return Ok(0);

            var average = ((DateTime.UtcNow.Year * totalClients) - sumClients.Aggregations.Sum("sum_ages").Value ?? 0)/totalClients;

            return Ok(average);
        }

        [CapSubscribe("client.services.registered"),NonAction]
        public async Task IndexingClient(NewClient client)
        {
            if(!(await _elasticClient.IndexExistsAsync("clientsmodel")).Exists)
            {
                _elasticClient.CreateIndex("clientsmodel", c => c.Mappings(m => m.Map<ClientModel>(mp => mp.AutoMap())));
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

            var result = await _elasticClient.IndexAsync(indexedClient,id=> id.Index("clientsmodel"));
        }
    }
}
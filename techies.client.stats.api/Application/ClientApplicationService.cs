﻿using Microsoft.Extensions.Logging;
using Nest;
using System;
using System.Linq;
using System.Threading.Tasks;
using Techies.Client.Stats.Api.Model;

namespace Techies.Client.Stats.Api.Application
{
    public class ClientApplicationService : IClientApplicationService
    {
        public const int MaxPageSize = 500;
        private const string StatsField = "stats";
        private readonly ElasticClient _client;
        private readonly ILogger<ClientApplicationService> _logger;

        public ClientApplicationService(
            ElasticClient client,
            ILogger<ClientApplicationService> logger)
        {
            _client = client;
            _logger = logger;
        }

        public async Task<PagedList<ClientModel>> ListClients(string search = null, int page = 1, int pageSize = 500)
        {
            var datas = await _client.SearchAsync<ClientModel>(s => s.Index("clientsmodel")
            .MatchAll()
            .From((page - 1) * pageSize)
            .Take(pageSize));

            var response = PagedList.With(datas.Documents.ToArray(), page)
                .WithPageSize(pageSize).WithTotal(datas.Total);

            return response;
        }

        public async Task<ClientModel> GetClientById(string id)
        {
            var datas = await _client.SearchAsync<ClientModel>(s => s.Index("clientsmodel")
            .Query(q => q.Ids(i => i.Values(id)))
            .Take(1));

            var client = datas.Documents.FirstOrDefault();

            return client;
        }

        public async Task<ClientStatsResponse> CalculateStats()
        {
            var currentTime = DateTime.UtcNow.Year;

            const string ageScript = "params['currentTime'] - doc['birthdateYear'].value";

            var datas = await _client.SearchAsync<ClientModel>(s => s.Index("clientsmodel")
            .MatchAll()
            .Aggregations(a => a.ExtendedStats(StatsField, d => d.Script(sf => sf.Source(ageScript)
                      .Params(sp => sp.Add("currentTime", currentTime))))));

            if (!datas.Aggregations.ContainsKey(StatsField)) return new ClientStatsResponse();

            var stats = datas.Aggregations[StatsField] as ExtendedStatsAggregate;

            var response = new ClientStatsResponse()
            {
                AverageAge = stats?.Average ?? 0,
                StdDeviationAge = stats?.StdDeviation ?? 0
            };

            return response;
        }

        public async Task IndexNewClient(NewClient client)
        {
            if (!(await _client.IndexExistsAsync("clientsmodel")).Exists)
            {
                var response = await _client.CreateIndexAsync("clientsmodel", c => c.Mappings(m => m.Map<ClientModel>(mp => mp.AutoMap())));
                if (!response.IsValid)
                {
                    _logger.LogError(response.OriginalException.ToString());
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

            var result = await _client.IndexAsync(indexedClient, id => id.Index("clientsmodel"));

            if (!result.IsValid)
            {
                _logger.LogError(result.OriginalException.ToString());
                throw result.OriginalException;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Techies.Client.Stats.Api.Model
{
    public class ClientStatsResponse
    {
        public double AverageAge { get; set; }
        public double StdDeviationAge { get; set; }
    }
}

using Microsoft.ML.Data;

namespace Techies.Client.Stats.Api.Infrastructure.ExternalServices.Prediction
{
    public class LiveData
    {
        [LoadColumn(0)]
        public float Year;

        [LoadColumn(1)]
        public float Age;
    }
}

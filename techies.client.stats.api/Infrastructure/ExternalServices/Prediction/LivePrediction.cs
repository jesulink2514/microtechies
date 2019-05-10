using Microsoft.ML.Data;

namespace Techies.Client.Stats.Api.Infrastructure.ExternalServices.Prediction
{
    public class LivePrediction
    {
        [ColumnName("Score")]
        public float PredictedAge;
    }
}

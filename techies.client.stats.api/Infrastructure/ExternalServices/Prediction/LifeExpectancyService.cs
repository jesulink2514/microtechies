using Microsoft.ML;
using System;
using System.Reflection;

namespace Techies.Client.Stats.Api.Infrastructure.ExternalServices.Prediction
{
    public class MLNetLifeExpectancyPredictionService : ILifeExpectancyPredictionService
    {
        private readonly PredictionEngine<LiveData, LivePrediction> _predEngine;

        public MLNetLifeExpectancyPredictionService()
        {
            var mlContext = new MLContext();

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Techies.Client.Stats.Api.Infrastructure.ExternalServices.Prediction.LiveExpectancyModel.zip";

            using (var model = assembly.GetManifestResourceStream(resourceName)) 
            {            
                var mlModel = mlContext.Model.Load(model, out var modelInputSchema);
                _predEngine = mlContext.Model.CreatePredictionEngine<LiveData, LivePrediction>(mlModel);
            }
        }

        public DateTime PredictDeath(DateTime birthdate)
        {
            var input = new LiveData() { Year = birthdate.Year };

            var result = _predEngine.Predict(input);

            return birthdate.AddDays(result.PredictedAge * 365.24);
        }
    }
}

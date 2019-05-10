using System;

namespace Techies.Client.Stats.Api.Infrastructure.ExternalServices.Prediction
{
    public interface ILifeExpectancyPredictionService
    {
        DateTime PredictDeath(DateTime birthdate);
    }
}
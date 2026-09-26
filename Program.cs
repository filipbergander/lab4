using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using PassengerInput;
using PassengerSchema;
using Passenger;
using static System.Console;

namespace predictTitanicSurvivorRate
{
    class Program
    {
        static void Main(string[] args)
        {
            // Machine learning modellen
            MLContext mlModel = new MLContext(seed: 1);

            // Datafiler över passagerare på titanic från Kaggle
            //string csvPath = "titanic_small.csv";
            string csvPath = "Titanic-Dataset.csv";

            // Skriver ut konsollen i grön text
            ForegroundColor = ConsoleColor.Yellow;

            // Laddar in data från kaggle-filen
            IDataView data = DataLoader.LoadData(mlModel, csvPath);

            // Skapar pipeline för data till modellen
            IEstimator<ITransformer> pipeline = PipeLine.CreatePipeline(mlModel);

            // 5-faldig korsvalidering
            TrainValidate.ValidateModel(mlModel, data, pipeline);

            // Tränar modellen
            ITransformer trainedModel = TrainValidate.TrainModel(mlModel, data, pipeline);

            // Det som användaren matar in i konsollen
            PassengerData passenger = PassengerInputService.PassengerInput();

            // Utvärderar sannolikheten för att en ny passagerare som matas in i konsollen överlever, enligt modellen
            PredictSurvival.predictPassengerSurvivalRate(mlModel, trainedModel, passenger);

            // Återställer konsollens textfärg
            WriteLine();
            ResetColor();
        }
    }
}

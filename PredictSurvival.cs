using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.ML;
using Microsoft.ML.Transforms;
using PassengerSchema;
using static System.Console;

namespace Passenger
{
    public class PredictSurvival
    {
        // Förutsäger sannolikheten att en passagerare överlever, baserat på det användaren matar in i konsollen
        public static void predictPassengerSurvivalRate(
            MLContext mlModel,
            ITransformer model,
            PassengerData passenger
        )
        {
            var predictionEngine = mlModel.Model.CreatePredictionEngine<
                PassengerData,
                PassengerPrediction
            >(model);
            // Förutsäger sannolikheten att en passagerare överlever, baserat på det användaren matar in i konsollen
            PassengerPrediction prediction = predictionEngine.Predict(passenger);

            // Utskrift av sannolikheten
            if (prediction.Survived)
            {
                ForegroundColor = ConsoleColor.Green;
                WriteLine("Resultat:");
                WriteLine("Modellen tror att passageraren överlever!");
            }
            else
            {
                ForegroundColor = ConsoleColor.DarkRed;
                WriteLine("Resultat:");
                WriteLine("Modellen tror att passageraren INTE överlever...");
            }
            WriteLine($"Sannolikhet för överlevnad: {prediction.Probability:P1}");
        }
    }
}

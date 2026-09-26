using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.ML;
using PassengerSchema;
using predictTitanicSurvivorRate;
using static System.Console;

namespace Passenger
{
    public class TrainValidate
    {
                      // Se över valideringen av datan som angetts mot modellen med 5-faldig korsvalidering
        public static void ValidateModel(
            MLContext mlModel,
            IDataView data,
            IEstimator<ITransformer> pipeline
        )
        {
            // 5-faldig korsvalidering
            var cvResults = mlModel.BinaryClassification.CrossValidate(
                data,
                pipeline,
                numberOfFolds: 5,
                labelColumnName: nameof(PassengerData.Survived)
            );

            // Mäter medelvärdet av precisionen i korsvalideringarna
            double avgAccuracy = cvResults.Average(res => res.Metrics.Accuracy);
            double avgAuc = cvResults.Average(res => res.Metrics.AreaUnderRocCurve);

            // Utskrift
            WriteLine("Resultat från 5-faldig korsvalidering");
            WriteLine($"Medelprecision: {avgAccuracy:P2}");
            WriteLine($"Medel AUC: {avgAuc:P2}\n");
        }

        // Träna & testa modellen
        public static ITransformer TrainModel(
            MLContext mlModel,
            IDataView data,
            IEstimator<ITransformer> pipeline
        )
        {
            // Testar modellen enligt 80/20, 80% träning & 20% test
            var split = mlModel.Data.TrainTestSplit(data, testFraction: 0.2, seed: 1);

            var modelTraining = pipeline.Fit(split.TrainSet);
            var testPredictions = modelTraining.Transform(split.TestSet);

            var testMetrics = mlModel.BinaryClassification.Evaluate(
                testPredictions,
                labelColumnName: nameof(PassengerData.Survived)
            );
            // Utskrift av träning & test utvärderingen
            WriteLine("80/20 utvärderingen");
            WriteLine($"Precision: {testMetrics.Accuracy:P2}");
            WriteLine($"AUC: {testMetrics.AreaUnderRocCurve:P2}");
            WriteLine($"F1-score: {testMetrics.F1Score:P2}\n");
            WriteLine(testMetrics.ConfusionMatrix.GetFormattedConfusionTable());

            return modelTraining;
        }
    }
}
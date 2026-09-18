using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using PassengerInput;
using PassengerSchema;
using static System.Console;

namespace predictTitanicSurvivorRate
{
    class Program
    {
        static void Main(string[] args)
        {
            // Machine learning modellen
            MLContext machineLearning = new MLContext(seed: 1);

            // Datafiler över passagerare på titanic från Kaggle
            //string csvPath = "titanic_small.csv";
            string csvPath = "Titanic-Dataset.csv";

            // Skriver ut konsollen i grön text
            ForegroundColor = ConsoleColor.Green;

            // Laddar in data från kaggle-filen
            IDataView data = LoadData(machineLearning, csvPath);

            // Skapar pipeline för data till modellen
            IEstimator<ITransformer> pipeline = CreatePipeline(machineLearning);

            // 5-faldig Korsvalidering
            ValidateModel(machineLearning, data, pipeline);
            // Återställer konsollens textfärg
            Console.ResetColor();
            // Det som användaren matar in i konsollen
            PassengerData passenger = PassengerInputService.PassengerInput();
        }

        public static IDataView LoadData(MLContext machineLearning, string csvPath)
        {
            if (!File.Exists(csvPath))
            {
                WriteLine($"Kunde inte hitta data-filen: {Path.GetFullPath(csvPath)}");
                Environment.Exit(1);
            }

            IDataView data = machineLearning.Data.LoadFromTextFile<PassengerData>(
                path: csvPath,
                hasHeader: true,
                separatorChar: ',',
                allowQuoting: true
            );

            List<PassengerData> passengers = machineLearning
                .Data.CreateEnumerable<PassengerData>(data, reuseRowObject: false)
                .ToList();
            // Om inte filen kunde behandlas
            if (passengers.Count == 0)
            {
                WriteLine("CSV-filen kunde inte tolkas...");
                Environment.Exit(1);
            }

            // Skriver ut antal passagerare totalt samt de första 5 raderna i datamängden
            WriteLine($"Antal passagerare: {passengers.Count}");
            var preview = data.Preview(maxRows: 20);
            WriteLine($"De första 5 raderna i datamängden:");
            foreach (var row in preview.RowView)
            {
                foreach (var column in row.Values)
                {
                    Write($"{column.Key}: {column.Value}\t");
                }
                WriteLine();
            }
            WriteLine();
            // Returnerar data-objektet för att använda till att träna modellen
            return data;
        }

        // Översätter all den data som matas in till ett format som ML kan använda för att träna modellen
        public static IEstimator<ITransformer> CreatePipeline(MLContext machineLearning)
        {
            return machineLearning
                .Transforms.Categorical.OneHotEncoding(
                    outputColumnName: "SexEncoded",
                    inputColumnName: nameof(PassengerData.Sex)
                )
                .Append(
                    machineLearning.Transforms.ReplaceMissingValues(
                        "AgeFilled",
                        nameof(PassengerData.Age),
                        replacementMode: MissingValueReplacingEstimator.ReplacementMode.Mean
                    )
                )
                .Append(
                    machineLearning.Transforms.ReplaceMissingValues(
                        outputColumnName: "ParentChildFilled",
                        inputColumnName: nameof(PassengerData.Parch),
                        replacementMode: MissingValueReplacingEstimator.ReplacementMode.DefaultValue
                    )
                )
                .Append(
                    machineLearning.Transforms.ReplaceMissingValues(
                        outputColumnName: "PclassFilled",
                        inputColumnName: nameof(PassengerData.Pclass),
                        replacementMode: MissingValueReplacingEstimator.ReplacementMode.Mean
                    )
                )
                .Append(
                    machineLearning.Transforms.ReplaceMissingValues(
                        outputColumnName: "FareFilled",
                        inputColumnName: nameof(PassengerData.Fare),
                        replacementMode: MissingValueReplacingEstimator.ReplacementMode.Mean
                    )
                )
                .Append(
                    machineLearning.Transforms.Concatenate(
                        "Features",
                        "AgeFilled",
                        "SexEncoded",
                        "ParentChildFilled",
                        "PclassFilled",
                        "FareFilled"
                    )
                )
                .Append(machineLearning.Transforms.NormalizeMinMax("Features"))
                .Append(
                    machineLearning.BinaryClassification.Trainers.SdcaLogisticRegression(
                        labelColumnName: nameof(PassengerData.Survived),
                        featureColumnName: "Features"
                    )
                );
        }

        // Se över valideringen av datan som angetts mot modellen
        static void ValidateModel(
            MLContext machineLearning,
            IDataView data,
            IEstimator<ITransformer> pipeline
        )
        {
            // 5-faldig korsvalidering
            var cvResults = machineLearning.BinaryClassification.CrossValidate(
                data,
                pipeline,
                numberOfFolds: 5,
                labelColumnName: nameof(PassengerData.Survived)
            );

            // Mäter medelvärdet av precisionen i korsvalideringarna
            double avgAccuracy = cvResults.Average(res => res.Metrics.Accuracy);
            double avgAuc = cvResults.Average(res => res.Metrics.AreaUnderRocCurve);

            // Utskrift
            WriteLine("Resultat från 5-faldig korsvalidering: ");
            WriteLine($"Medelprecision (5-faldig): {avgAccuracy:P2}");
            WriteLine($"Medel AUC (5-faldig): {avgAuc:P2}\n");
        }
    }
}

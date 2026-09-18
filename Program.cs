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
            string csvPath = "titanic_small.csv";
            //string csvPath = "Titanic-Dataset.csv";

            // Laddar in data från kaggle-filen
            IDataView data = LoadData(machineLearning, csvPath);

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
            if (passengers.Count == 0)
            {
                WriteLine("CSV-filen kunde inte översättas...");
                Environment.Exit(1);
            }
            // Skriver ut antal passagerare totalt samt de första 5 raderna i datamängden
            WriteLine($"Antal passagerare: {passengers.Count}");
            var preview = data.Preview(maxRows: 5);
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
            // Returnerar data för att använda till att träna modellen
            return data;
        }

        /*

        var split = machineLearning.Data.TrainTestSplit(data, testFraction: 0.2, seed: 1);
        long trainCount = machineLearning
            .Data.CreateEnumerable<PassengerData>(split.TrainSet, reuseRowObject: false)
            .LongCount();
        long testCount = machineLearning
            .Data.CreateEnumerable<PassengerData>(split.TestSet, reuseRowObject: false)
            .LongCount();

        var pipeline = machineLearning
            .Transforms.Categorical.OneHotEncoding(
                outputColumnName: "SexEncoded",
                inputColumnName: nameof(PassengerData.Sex)
            )
            .Append(
                machineLearning.Transforms.ReplaceMissingValues(
                    outputColumnName: "AgeFilled",
                    inputColumnName: nameof(PassengerData.Age),
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
            );*/
    }
}

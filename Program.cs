using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.ML;
using Microsoft.ML.Data;
using Microsoft.ML.Transforms;
using PassengerInfo;
using PassengerInput;
using static System.Console;

namespace predictTitanicSurvivorRate
{
    class Program
    {
        static void Main(string[] args)
        {
            string csvPath = "titanic_small.csv";
            //string csvPath = "Titanic-Dataset.csv";

            if (!File.Exists(csvPath))
            {
                WriteLine($"Kunde inte hitta data-filen: {Path.GetFullPath(csvPath)}");
                return;
            }
            PassengerInputService.PassengerInput();
            // PassengerPrediction prediction = predictionEngine.Predict
            //WriteLine($"{}");
        }

        public static void LoadData()
        {
            MLContext mlContext = new MLContext(seed: 1);
            IDataView data = mlContext.Data.LoadFromTextFile<PassengerData>(
                path: "titanic_small.csv",
                hasHeader: true,
                separatorChar: ',',
                allowQuoting: true
            );
            List<PassengerData> passengers = mlContext
                .Data.CreateEnumerable<PassengerData>(data, reuseRowObject: false)
                .ToList();
            if (passengers.Count == 0)
            {
                WriteLine("CSV-filen kunde inte översättas...");
                return;
            }
            WriteLine($"Antal passagerare: {passengers.Count}");
            var preview = data.Preview(maxRows: 10);
            WriteLine($"De första 10 raderna i datamängden:");
            foreach (var row in preview.RowView)
            {
                foreach (var column in row.Values)
                {
                    Write($"{column.Key}: {column.Value}\t");
                }
                WriteLine();
            }
            WriteLine();

            var split = mlContext.Data.TrainTestSplit(data, testFraction: 0.2, seed: 1);
            long trainCount = mlContext
                .Data.CreateEnumerable<PassengerData>(split.TrainSet, reuseRowObject: false)
                .LongCount();
            long testCount = mlContext
                .Data.CreateEnumerable<PassengerData>(split.TestSet, reuseRowObject: false)
                .LongCount();

            var pipeline = mlContext
                .Transforms.Categorical.OneHotEncoding(
                    outputColumnName: "SexEncoded",
                    inputColumnName: nameof(PassengerData.Sex)
                )
                .Append(
                    mlContext.Transforms.ReplaceMissingValues(
                        outputColumnName: "AgeFilled",
                        inputColumnName: nameof(PassengerData.Age),
                        replacementMode: MissingValueReplacingEstimator.ReplacementMode.Mean
                    )
                )
                .Append(
                    mlContext.Transforms.ReplaceMissingValues(
                        outputColumnName: "ParentChildFilled",
                        inputColumnName: nameof(PassengerData.Parch),
                        replacementMode: MissingValueReplacingEstimator.ReplacementMode.DefaultValue
                    )
                )
                .Append(
                    mlContext.Transforms.ReplaceMissingValues(
                        outputColumnName: "PclassFilled",
                        inputColumnName: nameof(PassengerData.Pclass),
                        replacementMode: MissingValueReplacingEstimator.ReplacementMode.Mean
                    )
                )
                .Append(
                    mlContext.Transforms.ReplaceMissingValues(
                        outputColumnName: "FareFilled",
                        inputColumnName: nameof(PassengerData.Fare),
                        replacementMode: MissingValueReplacingEstimator.ReplacementMode.Mean
                    )
                )
                .Append(
                    mlContext.Transforms.Concatenate(
                        "Features",
                        "AgeFilled",
                        "SexEncoded",
                        "ParentChildFilled",
                        "PclassFilled",
                        "FareFilled"
                    )
                )
                .Append(mlContext.Transforms.NormalizeMinMax("Features"))
                .Append(
                    mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                        labelColumnName: nameof(PassengerData.Survived),
                        featureColumnName: "Features"
                    )
                );
        }
    }
}

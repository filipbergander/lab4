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
    public class DataLoader
    {
        // Hämtar in data från kaggles Titanic-dataset
        public static IDataView LoadData(MLContext mlModel, string csvPath)
        {
            if (!File.Exists(csvPath))
            {
                WriteLine($"Kunde inte hitta data-filen: {Path.GetFullPath(csvPath)}");
                Environment.Exit(1);
            }

            IDataView data = mlModel.Data.LoadFromTextFile<PassengerData>(
                path: csvPath,
                hasHeader: true,
                separatorChar: ',',
                allowQuoting: true
            );

            List<PassengerData> passengers = mlModel
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

            // Returnerar data-objektet för att använda till att träna modellen
            return data;
        }
    }
}

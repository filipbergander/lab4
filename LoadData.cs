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

            // Skriver ut antal passagerare totalt samt överlevande eller inte
            int survived = passengers.Count(p => p.Survived);
            int notSurvived = passengers.Count(p => !p.Survived);
            double survivalRate = (double)survived / passengers.Count;
            WriteLine($"Antal passagerare: {passengers.Count}");
            WriteLine($"Överlevande: {survived}");
            WriteLine($"Omkomna: {notSurvived}");
            WriteLine($"Chans för överlevnad: {survivalRate:P1}\n");
            // Första raderna från datafilen
            var preview = data.Preview(maxRows: 5);
            WriteLine($"De första 5 raderna i datamängden:");
            foreach (var column in data.Schema)
            {
                Write($"{column.Name.PadRight(10)}");
            }
            WriteLine();
            foreach (var row in preview.RowView)
            {
                foreach (var column in row.Values)
                {
                     Write($"{column.Value?.ToString()!.PadRight(10)}");
                }
                WriteLine();
            }
            WriteLine();

            // Returnerar data-objektet för att använda till att träna modellen
            return data;
        }
    }
}

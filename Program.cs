using Microsoft.ML;
using static System.Console;

namespace predictTitanicSurvivorRate
{
    class Program
    {
        static void Main(string[] args)
        {
            string csvPath = "titanic_small.csv";
            //string csvPath = "Titanic-Dataset.csv";
            MLContext mlContext = new MLContext(seed: 1);
            if (!File.Exists(csvPath))
            {
                WriteLine($"Kunde inte hitta data-filen: {Path.GetFullPath(csvPath)}");
                return;
            }
            else
            {
                WriteLine("Hittade filen!");
            }
        }
    }
}

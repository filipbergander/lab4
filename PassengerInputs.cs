using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using PassengerSchema;
using predictTitanicSurvivorRate;
using static System.Console;

namespace PassengerInput
{
    // Frågar efter passagerares info för att träna och testa modellen
    public static class PassengerInputService
    {
        public static PassengerData PassengerInput()
        {
            bool programRunning = true;
            var passenger = new PassengerData();

            // Inputs för en passagerare
            WriteLine("Ange passagerarens uppgifter");

            // Ålder -> Age
            while (programRunning)
            {
                Write("Ange ålder: ");
                if (float.TryParse(ReadLine(), out float age) && age > 0 && age <= 120)
                {
                    passenger.Age = age;
                    break;
                }
                WriteLine("Ogiltig ålder! Ange ett nummer mellan 0 och 120...\n");
            }

            // Kön -> Sex
            while (programRunning)
            {
                Write("Ange kön, man eller kvinna: ");
                string sex = (ReadLine() ?? string.Empty).Trim().ToLowerInvariant();

                if (sex == "man")
                {
                    passenger.Sex = "male";
                    break;
                }
                else if (sex == "kvinna")
                {
                    passenger.Sex = "female";
                    break;
                }
                else
                {
                    WriteLine("Ogiltigt kön! Ange man eller kvinna...\n");
                }
            }
            // Biljettklass -> Pclass
            while (programRunning)
            {
                Write("Ange biljettklass 1, 2 eller 3: ");
                if (float.TryParse(ReadLine(), out float pclass) && pclass >= 1 && pclass <= 3)
                {
                    passenger.Pclass = pclass;
                    break;
                }
                WriteLine("Ogiltig biljettklass! Ange ett nummer mellan 1 och 3...\n");
            }
            // Partner/syskon -> SibSp
            while (programRunning)
            {
                Write("Ange antal partner eller syskon ombord: ");
                if (float.TryParse(ReadLine(), out float sibsp) && sibsp >= 0 && sibsp <= 10)
                {
                    passenger.SibSp = sibsp;
                    break;
                }
                WriteLine(
                    "Ogiltigt antal partner eller syskon! Ange ett nummer mellan 0 och 10...\n"
                );
            }
            // Föräldrar/barn -> Parch
            while (programRunning)
            {
                Write("Ange antal föräldrar eller barn ombord: ");
                if (float.TryParse(ReadLine(), out float parch) && parch >= 0 && parch <= 10)
                {
                    passenger.Parch = parch;
                    break;
                }
                WriteLine(
                    "Ogiltigt antal föräldrar eller barn! Ange ett nummer mellan 0 och 10...\n"
                );
            }
            // Biljettpris -> Fare
            while (programRunning)
            {
                Write("Ange biljettpris: ");
                if (float.TryParse(ReadLine(), out float fare) && fare >= 0 && fare <= 100000)
                {
                    passenger.Fare = fare;
                    break;
                }
                WriteLine("Biljetpriset kan inte vara under 0 eller över 100 000...\n");
            }
            // Utskrift av det som användaren har angett
            WriteLine("\nEn ny passagerare tillagd, datan behandlas...");
            WriteLine($"Ålder: {passenger.Age}");
            WriteLine($"Kön: {passenger.Sex}");
            WriteLine($"Klass: {passenger.Pclass}");
            WriteLine($"Partner/syskon: {passenger.SibSp}");
            WriteLine($"Föräldrar/barn: {passenger.Parch}");
            WriteLine($"Biljettpris: {passenger.Fare}");
            WriteLine("-----------------------------------");
            // Returnerar passageraren för att använda till att förutsäga chansen till överlevnad i ML modellen
            return passenger;
        }
    }
}

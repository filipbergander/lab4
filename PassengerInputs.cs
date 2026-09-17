using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using PassengerInfo;
using predictTitanicSurvivorRate;
using static System.Console;

namespace PassengerInput
{
    public static class PassengerInputService
    {
        public static PassengerData PassengerInput()
        {
            WriteLine($"Hittade filen");
            bool programRunning = true;
            var passengerInfo = new PassengerData();
            
            // Inputs för en passagerare
            // Ålder -> Age
            while (programRunning)
            {
                Write("Ange ålder: ");
                if (float.TryParse(ReadLine(), out float age) && age > 0 && age <= 120)
                {
                    passengerInfo.Age = age;
                    break;
                }
                WriteLine("Ogiltig ålder! Ange ett nummer mellan 0 och 120...\n");
            }

            // Kön -> Sex
            while (programRunning)
            {
                Write("Ange kön, man eller kvinna: ");
                string? sex = ReadLine().Trim().ToLower() ?? "";

                if (sex == "man" || sex == "kvinna")
                {
                    passengerInfo.Sex = sex;
                    break;
                }
                WriteLine("Ogiltigt kön! Ange man eller kvinna...\n");
            }
            // Biljettklass -> Pclass
            while (programRunning)
            {
                Write("Ange biljettklass 1, 2 eller 3: ");
                if (float.TryParse(ReadLine(), out float pclass) && pclass >= 1 && pclass <= 3)
                {
                    passengerInfo.Pclass = pclass;
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
                    passengerInfo.SibSp = sibsp;
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
                    passengerInfo.Parch = parch;
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
                    passengerInfo.Fare = fare;
                    break;
                }
                WriteLine("Biljetpriset kan inte vara under 0 eller över 100 000...\n");
            }
            WriteLine("\nEn ny passagerare tillagd, datan behandlas...");
            WriteLine(
                $"Ålder: {passengerInfo.Age}\nKön: {passengerInfo.Sex}\nKlass: {passengerInfo.Pclass}\nPartner/syskon: {passengerInfo.SibSp}\nFöräldrar/barn: {passengerInfo.Parch}\nBiljettpris: {passengerInfo.Fare}\n---------------"
            );
            return passengerInfo;
        }
    }
}

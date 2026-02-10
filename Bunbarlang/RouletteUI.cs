using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bunbarlang
{
    static class RouletteUI
    {
        static readonly string[] Wheel =
        {
        "0", "32", "15", "19", "4", "21", "2", "25",
        "17", "34", "6", "27", "13", "36", "11", "30",
        "8", "23", "10", "5", "24", "16", "33", "1",
        "20", "14", "31", "9", "22", "18", "29", "7",
        "28", "12", "35", "3", "26"
        };

        public static string DrawWheel(
            bool animate,
            int forcedResult,
            int spinCycles = 3,
            int delayMs = 80
        )
        {
            int targetIndex = Array.IndexOf(Wheel,Convert.ToString(forcedResult));
            if (targetIndex == -1)
                throw new ArgumentException("Invalid roulette number.");

            int currentIndex = 0;

            int stepsToTarget =
                (targetIndex - currentIndex + Wheel.Length) % Wheel.Length;

            int totalSteps = animate
                ? (Wheel.Length * spinCycles) + stepsToTarget
                : stepsToTarget;

            for (int i = 0; i < totalSteps; i++)
            {
                Console.Clear();
                DrawFrame(currentIndex);

                currentIndex = (currentIndex + 1) % Wheel.Length;

                if (animate)
                    Thread.Sleep(delayMs);
            }

            // Final guaranteed landing frame
            Console.Clear();
            DrawFrame(targetIndex);

            return Wheel[targetIndex];
        }

        private static void DrawFrame(int selectedIndex)
        {
            Console.WriteLine("====== ROULETTE ======\n");

            for (int i = -3; i <= 3; i++)
            {
                int wheelIndex =
                    (selectedIndex + i + Wheel.Length) % Wheel.Length;

                if (i == 0)
                    Console.WriteLine($">>> [{Wheel[wheelIndex]}] <<<");
                else
                    Console.WriteLine($"     {Wheel[wheelIndex]}");
            }

            Console.WriteLine("\n======================");
        }
    }


}

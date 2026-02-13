using System;
using System.Threading;

namespace Bunbarlang
{
    public class Coin : IGame
    {
        public string Name => "Coin Flip";
        public bool IsCompleted { get; private set; }

        private readonly Random rng = new();

        public void Play(Player player)
        {
            Console.Title = "Den of Sin – Coin Flip";
            Console.CursorVisible = true;

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== DEN OF SIN ===");
                Console.WriteLine($"Chips: {player.Chips}\n");

                Console.Write("Bet amount on the coinflip (enter 0 to return to lobby): ");
                if (!int.TryParse(Console.ReadLine(), out int bet) || bet < 0)
                {
                    Console.WriteLine("Invalid bet!");
                    Thread.Sleep(1200);
                    continue;
                }

                if (bet == 0)
                    break;

                if (bet > player.Chips)
                {
                    Console.WriteLine("You don't have enough chips!");
                    Thread.Sleep(1200);
                    continue;
                }

                Console.Write("Heads (H) or Tails (T)? ");
                string choice = Console.ReadLine()?.Trim().ToUpper();

                if (choice != "H" && choice != "T")
                {
                    Console.WriteLine("Invalid choice!");
                    Thread.Sleep(1200);
                    continue;
                }

                // 0 = Heads, 1 = Tails
                int flip = rng.Next(0, 2);
                bool playerWins = (flip == 0 && choice == "H") || (flip == 1 && choice == "T");

                if (playerWins)
                {
                    player.AddChips(bet);
                    Console.WriteLine($"\nYou won! +{bet} chips 🎉");
                }
                else
                {
                    player.RemoveChips(bet);
                    Console.WriteLine($"\nYou lost! -{bet} chips 💀");
                }

                Console.WriteLine($"\nCurrent chips: {player.Chips}");
                Console.WriteLine("\nENTER – play again | ESC – return to lobby");

                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Escape)
                    break;
            }

            Console.CursorVisible = false;
        }
    }
}

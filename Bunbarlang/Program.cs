using System;
using System.Collections.Generic;
using System.Threading;

namespace Bunbarlang
{
    public class WheelOfSin
    {
        private readonly List<IGame> _games;
        private readonly Random _rng = new Random();

        public WheelOfSin(List<IGame> games)
        {
            _games = games ?? throw new ArgumentNullException(nameof(games));
            if (_games.Count == 0) throw new ArgumentException("At least one game must be provided.", nameof(games));
        }

        public void ShowIntro(Player player, bool suppressContinuePrompt)
        {
            if (player is null) throw new ArgumentNullException(nameof(player));

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine("===== WHEEL OF SIN =====");
            Console.ResetColor();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Welcome {player.Name}! - The Den of Sin awaits. Your chips: {player.Chips}");
            Console.WriteLine();
            Console.WriteLine("Spin the Wheel to be sent to a random game. Some games may cost chips, others may reward you, but beware! \n If you leave a table you leave it for good...");
            Console.WriteLine();
            Console.ResetColor();

            if (!suppressContinuePrompt)
            {
                Console.WriteLine("Press any key to approach the wheel...");
                Console.ReadKey(true);
            }
        }


        public void Spin(Player player)
        {
            if (player is null) throw new ArgumentNullException(nameof(player));

            Console.Clear();
            Console.WriteLine($"Player: {player.Name}   Chips: {player.Chips}");
            Console.WriteLine();
            Console.WriteLine("The Wheel of Sin begins to turn...");
            Console.WriteLine();

            int target = _rng.Next(_games.Count);
            int cycles = 3;
            int steps = (_games.Count * cycles) + target;
            int cursorTop = Console.CursorTop;

            for (int i = 0; i <= steps; i++)
            {
                int index = i % _games.Count;
                RenderWheel(index, cursorTop);
                int delay = i < steps - _games.Count ? 90 : 40;
                Thread.Sleep(delay);
            }

            var chosen = _games[target];

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"The wheel lands on: {chosen.Name}");
            Console.ResetColor();
            Console.WriteLine("Press any key to proceed to the game...");
            Console.ReadKey(true);
            _games.Remove(chosen);
            chosen.Play(player);

        }

        private void RenderWheel(int selectedIndex, int top)
        {
            try
            {
                for (int i = 0; i < _games.Count; i++)
                {
                    int line = top + i;
                    Console.SetCursorPosition(0, line);
                    ClearLine();

                    if (i == selectedIndex)
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write($"> {_games[i].Name}");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write($"  {_games[i].Name}");
                    }
                }
                Console.SetCursorPosition(0, top + _games.Count);
            }
            catch
            {
                Console.Clear();
                for (int i = 0; i < _games.Count; i++)
                {
                    if (i == selectedIndex)
                        Console.WriteLine($"> {_games[i].Name}");
                    else
                        Console.WriteLine($"  {_games[i].Name}");
                }
            }
        }

        private static void ClearLine()
        {
            try
            {
                int width = Console.WindowWidth;
                Console.Write(new string(' ', Math.Max(0, width - 1)));
                Console.SetCursorPosition(0, Console.CursorTop);
            }
            catch
            {
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Den of Sin";

            Console.Write("Player name: ");
            var name = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(name))
                name = "Player";

            var player = new Player
            {
                Name = name,
                Chips = 1000
            };

            var games = new List<IGame>
            {
                new Roulette(),
                new Blackjack(),
                new Poker(),
                new Plinko(),
                new Slots(),
                new Coin(),
                new Lotto()
            };

            bool isDev = name.Equals("Dev", StringComparison.OrdinalIgnoreCase);

            // =====================================
            // DEV MODE = ORIGINAL LOBBY (UNCHANGED)
            // =====================================
            if (isDev)
            {
                int selected = 0;

                while (true)
                {
                    Console.Clear();
                    Console.WriteLine($"Player: {player.Name}   Chips: {player.Chips}");
                    Console.WriteLine("Use Up/Down arrows to select a game. Press Enter to play. Press Esc to exit.\n");

                    for (int i = 0; i < games.Count; i++)
                    {
                        if (i == selected)
                        {
                            Console.BackgroundColor = ConsoleColor.White;
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.WriteLine($"> {games[i].Name}");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.WriteLine($"  {games[i].Name}");
                        }
                    }

                    var key = Console.ReadKey(true);

                    if (key.Key == ConsoleKey.Escape)
                        break;

                    if (key.Key == ConsoleKey.UpArrow)
                    {
                        selected = (selected - 1 + games.Count) % games.Count;
                        continue;
                    }

                    if (key.Key == ConsoleKey.DownArrow)
                    {
                        selected = (selected + 1) % games.Count;
                        continue;
                    }

                    if (key.Key == ConsoleKey.Enter)
                    {
                        Console.Clear();
                        var chosen = games[selected];
                        chosen.Play(player);

                        Console.WriteLine();
                        Console.WriteLine($"Returning to lobby. Chips: {player.Chips}");
                        Console.WriteLine("Press any key to continue...");
                        Console.ReadKey(true);
                    }
                }

                Console.WriteLine("Goodbye.");
                return;
            }

            // =====================================
            // NORMAL MODE = WHEEL OF SIN
            // =====================================
            var wheel = new WheelOfSin(games);

            wheel.ShowIntro(player, false);

            while (true)
            {
                wheel.Spin(player);

                if (player.Chips <= 0)
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                    Console.WriteLine("You have no more credits or dignity...");
                    Console.ResetColor();
                    Console.ReadKey(true);
                    break;
                }

                if (games.Count == 0)
                {
                    new RussianRoulette().Play(player);
                }

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Press SPACE to sin again or ESCAPE to seek redemption.");
                Console.ResetColor();

                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.Escape)
                    break;

                if (key != ConsoleKey.Spacebar)
                    continue;
            }

            Console.Clear();
            Console.WriteLine("You fade with the Den of Sin...");
            Console.ReadKey(true);
        }
    }
}

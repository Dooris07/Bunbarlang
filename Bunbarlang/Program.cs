using System;
using System.Collections.Generic;

namespace Bunbarlang
{
	internal class Program
	{
		static void Main(string[] args)
		{
			Console.Write("Player name: ");
			var name = Console.ReadLine();
			if (string.IsNullOrWhiteSpace(name)) name = "Player";

			var player = new Player { Name = name, Chips = 10000 };

			var games = new List<IGame>
			{
				new Roulette(),
				new Blackjack(),
				new Poker(),
				new Plinko(),
				new Slots(),
				new RussianRoulette()
            };

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
		}
	}
}

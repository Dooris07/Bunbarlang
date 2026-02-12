using System;

namespace Bunbarlang
{
	public class Roulette : IGame
	{
		public string Name => "Roulette";
		public bool IsCompleted { get; private set; }

		// pocket -> color (0 = Green, 1 = Red, 2 = Black)
		static readonly int[] rouletteWheel = new int[] { 0, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 2, 1, 2, 1, 2, 1, 2, 1, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 2, 1, 2, 1, 2, 1, 2, 1 };
		static readonly string[] colors = new[] { "Green", "Red", "Black" };
		static readonly string[,] options = new string[,] { { "  1-12 ", " 13-24 ", " 25-36 " }, { "  1-18 ", "  Num  ", " 19-36 " }, { "  Red  ", "  Ext  ", " Black " } };

		public void Play(Player player)
		{
			if (player is null) throw new ArgumentNullException(nameof(player));
			IsCompleted = false;

			var rnd = new Random();
			bool isPlaying = true;

			while (isPlaying && player.Chips > 0)
			{
				switch (Menu())
				{
					case 1:
						{
							int spin = rnd.Next(37);
							int net = ThirdBet(spin, 1, player);
							player.AddChips(net);
							break;
						}
					case 2:
						{
							int spin = rnd.Next(37);
							int net = ThirdBet(spin, 2, player);
							player.AddChips(net);
							break;
						}
					case 3:
						{
							int spin = rnd.Next(37);
							int net = ThirdBet(spin, 3, player);
							player.AddChips(net);
							break;
						}
					case 4:
						{
							int spin = rnd.Next(37);
							int net = HalfBet(spin, 1, player);
							player.AddChips(net);
							break;
						}
					case 5:
						{
							int spin = rnd.Next(37);
							int choice;
							do
							{
								Console.Write("Pick a number (0-36): ");
							}
							while (!int.TryParse(Console.ReadLine(), out choice) || choice < 0 || choice > 36);

							int net = choice == 0 ? ColorBet(spin, 0, player) : NumberBet(spin, choice, player);
							player.AddChips(net);
							break;
						}
					case 6:
						{
							int spin = rnd.Next(37);
							int net = HalfBet(spin, 2, player);
							player.AddChips(net);
							break;
						}
					case 7:
						{
							int spin = rnd.Next(37);
							int net = ColorBet(spin, 1, player);
							player.AddChips(net);
							break;
						}
					case 8:
						isPlaying = false;
						break;
					case 9:
						{
							int spin = rnd.Next(37);
							int net = ColorBet(spin, 2, player);
							player.AddChips(net);
							break;
						}
				}
			}

			IsCompleted = true;
		}

		private static int Menu()
		{
			bool isSelected = false;
			ConsoleKeyInfo keyInfo;
			int optionx = 1;
			int optiony = 1;
			int option = 1;

			while (!isSelected)
			{
				Console.Clear();
				RouletteUI.DrawWheel(false, 0);
				for (int i = 0; i < 3; i++)
				{
					for (int j = 0; j < 3; j++)
					{
						if (j == optionx - 1 && i == optiony - 1)
						{
							Console.BackgroundColor = ConsoleColor.White;
							Console.ForegroundColor = ConsoleColor.Black;
							Console.Write(options[i, j]);
							Console.ResetColor();
						}
						else
						{
							Console.Write(options[i, j]);
						}
					}
					Console.WriteLine();
				}

				keyInfo = Console.ReadKey(true);
				switch (keyInfo.Key)
				{
					case ConsoleKey.UpArrow:
						optiony = optiony == 1 ? 3 : optiony - 1;
						break;
					case ConsoleKey.DownArrow:
						optiony = optiony == 3 ? 1 : optiony + 1;
						break;
					case ConsoleKey.LeftArrow:
						optionx = optionx == 1 ? 3 : optionx - 1;
						break;
					case ConsoleKey.RightArrow:
						optionx = optionx == 3 ? 1 : optionx + 1;
						break;
					case ConsoleKey.Enter:
						isSelected = true;
						break;
				}

				switch (optiony)
				{
					case 2:
						if (optionx == 1) option = 4;
						else if (optionx == 2) option = 5;
						else option = 6;
						break;
					case 3:
						if (optionx == 1) option = 7;
						else if (optionx == 2) option = 8;
						else option = 9;
						break;
					default:
						option = optionx;
						break;
				}
			}

			return option;
		}

		private static int GetBetAmount(Player player)
		{
			if (player is null) throw new ArgumentNullException(nameof(player));
			int bet;
			do
			{
				Console.Write($"Bet amount (1 - {player.Chips}): ");
				if (!int.TryParse(Console.ReadLine(), out bet))
					bet = -1;
			}
			while (bet < 1 || bet > player.Chips);
			// withdraw stake immediately (net results will be applied as adjustments)
			player.RemoveChips(bet);
			return bet;
		}

		private static int NumberBet(int spin, int choice, Player player)
		{
			RouletteUI.DrawWheel(false, 0);
			int bet = GetBetAmount(player);
			Console.Clear();
			RouletteUI.DrawWheel(true, spin);

			int net;
			if (spin == choice)
			{
				// standard payout: 35:1 net (i.e., you receive 35x bet profit)
				net = bet * 35;
			}
			else
			{
				net = -0; // stake already removed, no payout
			}

			Console.WriteLine($"{colors[rouletteWheel[spin]]} {spin}");
			Console.ReadKey();
			return net;
		}

		private static int ThirdBet(int spin, int choice, Player player)
		{
			RouletteUI.DrawWheel(false, 0);
			int bet = GetBetAmount(player);
			Console.Clear();
			RouletteUI.DrawWheel(true, spin);

			bool win = (choice == 1 && spin >= 1 && spin <= 12)
					   || (choice == 2 && spin >= 13 && spin <= 24)
					   || (choice == 3 && spin >= 25 && spin <= 36);

			int net = win ? bet * 2 : 0; // stake already removed; on win return stake + equal profit
			Console.WriteLine($"{colors[rouletteWheel[spin]]} {spin}");
			Console.ReadKey();
			return net;
		}

		private static int HalfBet(int spin, int choice, Player player)
		{
			RouletteUI.DrawWheel(false, 0);
			int bet = GetBetAmount(player);
			Console.Clear();
			RouletteUI.DrawWheel(true, spin);

			bool win = (choice == 1 && spin >= 1 && spin <= 18)
					   || (choice == 2 && spin >= 19 && spin <= 36);

			int net = win ? bet * 2 : 0;
			Console.WriteLine($"{colors[rouletteWheel[spin]]} {spin}");
			Console.ReadKey();
			return net;
		}

		private static int ColorBet(int spin, int choice, Player player)
		{
			RouletteUI.DrawWheel(false, 0);
			int bet = GetBetAmount(player);
			Console.Clear();
			RouletteUI.DrawWheel(true, spin);

			int net = 0;
			if (rouletteWheel[spin] == 0) // zero
			{
				Console.WriteLine($"Green {spin}");
				if (choice == 0)
					net = bet * 35;
			}
			else
			{
				Console.WriteLine($"{colors[rouletteWheel[spin]]} {spin}");
				if (rouletteWheel[spin] == choice)
					net = bet * 2;
			}

			Console.ReadKey();
			return net;
		}
	}
}

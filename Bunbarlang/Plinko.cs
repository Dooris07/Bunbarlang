using System;
using System.Threading;

namespace Bunbarlang
{
	public class Plinko : IGame
	{
		public string Name => "Plinko";
		public bool IsCompleted { get; private set; }

		private const int Rows = 11;

		private readonly int[] slotX =
		{
			10, 18, 26, 34, 42, 50, 58, 66, 74, 82, 90
		};

		private readonly double[] multipliers =
		{
			0.25, 0.50, 1.25, 1.50, 2.0, 3.0, 2.0, 1.5, 1.25, 0.5, 0.25
		};

		private readonly Random random = new Random();

		public void Play(Player player)
		{
			IsCompleted = false;
			Console.Clear();
			Console.CursorVisible = true;

			Console.WriteLine("=== BÛNBARLANG – PLINKO ===");
			Console.WriteLine($"Chips: {player.Chips}\n");
			Console.Write("Enter bet: ");

			if (!int.TryParse(Console.ReadLine(), out int bet) || bet <= 0)
			{
				Console.CursorVisible = false;
				return;
			}

			if (!player.RemoveChips(bet))
			{
				Console.WriteLine("Not enough chips.");
				Thread.Sleep(1000);
				Console.CursorVisible = false;
				return;
			}

			Console.CursorVisible = false;

			int slot = multipliers.Length / 2;
			int ballRow;

			for (ballRow = -1; ballRow < Rows; ballRow++)
			{
				Console.Clear();
				DrawBoard(ballRow, slot);
				Thread.Sleep(200);

				if (ballRow >= 0 && ballRow < Rows - 1)
				{
					if (slot == 0) slot++;
					else if (slot == multipliers.Length - 1) slot--;
					else slot += random.Next(2) == 0 ? -1 : 1;
				}
			}

			double multiplier = multipliers[slot];
			int win = (int)(bet * multiplier);

			Console.Clear();
			DrawBoard(Rows - 1, slot);

			Console.SetCursorPosition(0, Rows + 3);
			Console.WriteLine($"Multiplier: {multiplier}");
			Console.WriteLine($"Winnings: {win}");

			player.AddChips(win);

			Console.WriteLine($"New chips: {player.Chips}");
			Console.WriteLine("\nPress any key to continue...");
			Console.ReadKey(true);

			IsCompleted = true;
			Console.CursorVisible = true;
		}

		private void DrawBoard(int ballRow, int ballSlot)
		{
			int slotCount = slotX.Length;

			for (int r = 0; r < Rows; r++)
			{
				int pegCount = slotCount - r - 1;
				int y = Rows - 1 - r;

				for (int i = 0; i < pegCount; i++)
				{
					int left = slotX[i];
					int right = slotX[i + 1];
					int x = (left + right) / 2 + r * 4;

					if (x >= 0)
					{
						try { Console.SetCursorPosition(x, y); } catch { }
						Console.Write("|");
					}
				}
			}

			if (ballRow >= 0 && ballRow < Rows)
			{
				try { Console.SetCursorPosition(slotX[ballSlot], ballRow); } catch { }
				Console.Write("O");
			}

			int bottomY = Rows + 1;
			for (int i = 0; i < multipliers.Length; i++)
			{
				try { Console.SetCursorPosition(slotX[i] - 2, bottomY); } catch { }
				Console.Write($"{multipliers[i],4}");
			}
		}
	}
}
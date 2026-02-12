using System;
using System.Threading;

namespace Bunbarlang
{
	public class RussianRoulette : IGame
	{
		public string Name => "Russian Roulette";
		public bool IsCompleted { get; private set; }

		public void Play(Player player)
		{
			IsCompleted = false;

			Console.WriteLine("You wake up in a dark room, sitting at a table. A revolver's barrel points at you from the middle of the table");
			Console.Write("...");
			Console.ReadKey();
			Console.Clear();
			Console.ForegroundColor = ConsoleColor.DarkRed;
			Console.WriteLine("Let's play one last game.");
			Console.ResetColor();
			Console.Write("...");
			Console.ReadKey();
			Console.Clear();
			Console.CursorVisible = false;

			int round = 1;
			var rnd = new Random();

			do
			{
				chamberDisplay(round);
				int chamber = rnd.Next(1, 7); // 1..6 inclusive
				Console.WriteLine("     ___    \n" +
				  "==  /   \\   \n" +
				  "    \\___/    \n" +
				  "     | |  \n");
				Thread.Sleep(1000);
				Console.Clear();
				shootingAnimation();

				if (chamber <= round)
				{
					Console.WriteLine("The chamber wasn't empty this time. You died.");
					Console.ReadKey();
					Environment.Exit(0);
				}
				else
				{
					Console.WriteLine("     ___    \n" +
							  "==  /   \\   \n" +
							  "    \\___/    \n" +
							  "     | |  \n");
					Console.WriteLine("You live another round.");
					Console.Write("...");
					Console.ReadKey();
					Console.Clear();
				}
				round++;
			}
			while (round <= 5);

			Console.ForegroundColor = ConsoleColor.DarkRed;
			Console.WriteLine("You lucky bastard! Looks like I have to take care of you myself...");
			Console.ResetColor();
			Console.CursorVisible = true;
			Console.WriteLine("...");
			Console.ReadKey();
			Console.Clear();
			Thread.Sleep(1000);
			shootingAnimation();
			Thread.Sleep(1000);
			Console.WriteLine("You got killed by the owner of the Den of Sin. Your earnings got stolen.");
			IsCompleted = true;
		}

		private static void shootingAnimation()
		{
			Console.BackgroundColor = ConsoleColor.White;
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("aaaaaaaaaaaaaaaaaaaaaaa\n" +
							  "aaaaaaaaaaaaaaaaaaaaaaa\n" +
							  "aaaaaaaaaaaaaaaaaaaaaaa\n" +
							  "aaaaaaaaaaaaaaaaaaaaaaa\n");
			Console.ResetColor();
			Thread.Sleep(1000);
			Console.Clear();
		}

		private static void chamberDisplay(int round)
		{
			switch (round)
			{
				case 1:
					Console.WriteLine(" /  0  \\\n" +
									  "|O     O|\n" +
									  "|O     O|\n" +
									  " \\  O  /\n");
					break;
				case 2:
					Console.WriteLine(" /  0  \\\n" +
									  "|O     0|\n" +
									  "|O     O|\n" +
									  " \\  O  /\n");
					break;
				case 3:
					Console.WriteLine(" /  0  \\\n" +
									  "|O     0|\n" +
									  "|O     0|\n" +
									  " \\  O  /\n");
					break;
				case 4:
					Console.WriteLine(" /  0  \\\n" +
									  "|O     0|\n" +
									  "|O     0|\n" +
									  " \\  0  /\n");
					break;
				case 5:
					Console.WriteLine(" /  0  \\\n" +
									  "|O     0|\n" +
									  "|0     0|\n" +
									  " \\  0  /\n");
					break;
			}

			if (round == 1)
			{
				Console.WriteLine("Press any button to proceed...");
			}
			Console.ReadKey();
			Console.Clear();
		}
	}
}

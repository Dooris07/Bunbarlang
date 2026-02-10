using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Bunbarlang
{
	static class Roulette
	{
		static int[] rouletteWheel = [0, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 2, 1, 2, 1, 2, 1, 2, 1, 1, 2, 1, 2, 1, 2, 1, 2, 1, 2, 2, 1, 2, 1, 2, 1, 2, 1];
		static string[] colors = ["Green", "Red", "Black"];
		static string[,] options = { {"  1-12 ", " 13-24 ", " 25-36 "}, { "  1-18 ", "  Num  ", " 19-36 " }, { "  Red  ", "  Ext  ", " Black " } };
		static int balance=10000;

        public static void Game()
		{
            Random rnd = new Random();
            int rand;
			bool isPlaying = true;
			do
			{
                switch (Menu())
				{

					case 1:
                        rand = rnd.Next(37);
                        balance += ThirdBet(rand, 1);
						break;
					case 2:
						rand = rnd.Next(37);
						balance += ThirdBet(rand, 2);
						break;
					case 3:
						rand = rnd.Next(37);
						balance += ThirdBet(rand, 3);
						break;
					case 4:
						rand = rnd.Next(37);
						balance += HalfBet(rand, 1);
						break;
					case 5:
						rand = rnd.Next(37);
						int choice;
                        do
						{
							Console.WriteLine("Pick a number (0-36)");
							choice = int.Parse(Console.ReadLine());
						}
						while (choice < 0 || choice > 36);
                        if (choice == 0)
						{
							balance += ColorBet(rand, 0);
						}
						else
						{
							balance += NumberBet(rand, choice);
						}
						break;
					case 6:
						rand = rnd.Next(37);
						balance += HalfBet(rand, 2);
						break;
					case 7:
						rand = rnd.Next(37);
						balance += ColorBet(rand, 1);
						break;
					case 8:
						isPlaying = false;
                        break;
					case 9:
						rand = rnd.Next(37);
						balance += ColorBet(rand,2);
						break;


				}
			}
			while (isPlaying&&balance>0);
        }
		private static int Menu()
		{
			bool isSelected = false;
			ConsoleKeyInfo keyInfo;
			int optionx = 1;
			int optiony = 1;
			int option=1;
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
				keyInfo = Console.ReadKey(false);
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
                        if (optionx == 1) { option = 4; }
                        else if (optionx == 2) { option = 5; }
                        else { option = 6; }
						break;
					case 3:
                        if (optionx == 1) { option = 7; }
                        else if (optionx == 2) { option = 8; }
                        else { option = 9; }
						break;
                    default:
                        option = optionx;
                        break;
                }
            }
            return (option);
			
        }
		private static int NumberBet(int spin, int choice)
		{
			RouletteUI.DrawWheel(false,0);
            int bet;
			do 
			{
                Console.Write($"Bet amount (max:{balance})");
				bet = int.Parse(Console.ReadLine());
            }
			while (bet>=balance);
			Console.Clear();
            RouletteUI.DrawWheel(true,spin);
            if (rouletteWheel[spin] == choice)
			{
				bet *= 2;
			}
			else
			{
				bet = bet * -1;
			}
			Console.WriteLine($"{colors[rouletteWheel[spin]]} {spin}");
			Console.ReadKey();
            return bet;
        }
		private static int ThirdBet(int spin, int choice)
		{
            RouletteUI.DrawWheel(false, 0);
            int bet;
            do
            {
                Console.Write($"Bet amount (max:{balance})");
                bet = int.Parse(Console.ReadLine());
            }
            while (bet >= balance);
            Console.Clear();
            RouletteUI.DrawWheel(true,spin);
            switch (choice)
            {
				case 1:
					if (spin <= 12)
					{
						bet *= 2;
					}
					else
					{
						bet = bet * -1;
					}
					break;
				case 2:
					if (spin >= 13 && spin <= 24)
					{
						bet *= 2;
					}
					else
					{
						bet = bet * -1;
					}
					break;
				case 3:
					if (spin >= 25 && spin <= 36)
					{
						bet *= 2;
					}
					else
					{
						bet = bet * -1;
					}
					break;
			}
            Console.WriteLine($"{colors[rouletteWheel[spin]]} {spin}");
            Console.ReadKey();
            return bet;
        }
		private static int HalfBet(int spin, int choice)
		{
            RouletteUI.DrawWheel(false, 0);
            int bet;
            do
            {
                Console.Write($"Bet amount (max:{balance})");
                bet = int.Parse(Console.ReadLine());
            }
            while (bet >= balance);
            Console.Clear();
            RouletteUI.DrawWheel(true,spin);
            switch (choice)
            {
				case 1:
					if (spin <= 18)
					{
						bet *= 2;
					}
					else
					{
						bet = bet * -1;
					}
					break;
				case 2:
					if (spin >= 19 && spin <= 36)
					{
						bet *= 2;
					}
					else
					{
						bet = bet * -1;
					}
					break;
			
			}
			return bet;
            Console.WriteLine($"{colors[rouletteWheel[spin]]} {spin}");
            Console.ReadKey();
        }
        private static int ColorBet(int spin, int choice)
		{

            RouletteUI.DrawWheel(false, 0);
            int bet;
            do
            {
                Console.Write($"Bet amount (max:{balance})");
                bet = int.Parse(Console.ReadLine());
            }
            while (bet >= balance);
            Console.Clear();
            RouletteUI.DrawWheel(true,spin);
            switch (rouletteWheel[spin])
            {
				case 0:
					Console.WriteLine($"Green fn {spin}");
					if (choice == 0)
					{
						bet *= 36;
					}
					else
					{
						bet = bet * -1;
					}
					break;
				case 1:
					Console.WriteLine($"Red fn {spin}");
					if (choice == 1)
					{
						bet *= 2;
					}
					else
					{
						bet = bet * -1;
					}
					break;
				case 2:
					Console.WriteLine($"Black fn {spin}");
					if (choice == 2)
					{
						bet *= 2;
					}
					else
					{
						bet = bet*-1;
					}
					break;
			}
            Console.WriteLine($"{colors[rouletteWheel[spin]]} {spin}");
            Console.ReadKey();
            return bet;
		}
	}
}

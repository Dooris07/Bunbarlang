using System;
using System.Threading;

namespace gamblingPlinko
{
    class Program
    {
        static void Main()
        {
            Console.CursorVisible = false;

            Player player = new Player(1000);
            PlinkoGame plinko = new PlinkoGame();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("=== BÛNBARLANG – PLINKO ===");
                Console.WriteLine($"Egyenleg: {player.Balance} Ft\n");

                plinko.Play(player);

                Console.WriteLine("\nSzeretnél újra játszani? (I/N)");
                ConsoleKey key = Console.ReadKey(true).Key;

                if (key != ConsoleKey.I)
                    break;
            }

            Console.Clear();
            Console.WriteLine($"Végsõ egyenleg: {player.Balance} Ft");
            Console.CursorVisible = true;
            Console.ReadKey();
        }
    }

    class Player
    {
        private int balance;
        public int Balance => balance;

        public Player(int startBalance)
        {
            balance = startBalance;
        }

        public bool RemoveMoney(int amount)
        {
            if (amount > balance) return false;
            balance -= amount;
            return true;
        }

        public void AddMoney(int amount)
        {
            balance += amount;
        }
    }

    class PlinkoGame
    {
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
            Console.Clear();
            Console.CursorVisible = true;

            Console.WriteLine("=== BÛNBARLANG – PLINKO ===");
            Console.WriteLine($"Egyenleg: {player.Balance} Ft\n");
            Console.Write("Add meg a tétet: ");

            if (!int.TryParse(Console.ReadLine(), out int bet) || bet <= 0)
            {
                Console.CursorVisible = false;
                return;
            }

            if (!player.RemoveMoney(bet))
            {
                Console.WriteLine("Nincs elég pénzed.");
                Thread.Sleep(1500);
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
                    if (slot == 0)
                        slot++;
                    else if (slot == multipliers.Length - 1)
                        slot--;
                    else
                        slot += random.Next(2) == 0 ? -1 : 1;
                }
            }

            double multiplier = multipliers[slot];
            int win = (int)(bet * multiplier);

            Console.Clear();
            DrawBoard(Rows - 1, slot);

            Console.SetCursorPosition(0, Rows + 3);
            Console.WriteLine($"Szorzó: {multiplier}");
            Console.WriteLine($"Nyeremény: {win} Ft");

            player.AddMoney(win);

            Console.WriteLine($"Új egyenleg: {player.Balance} Ft");
            Console.WriteLine("\nNyomj meg egy gombot a folytatáshoz...");
            Console.ReadKey(true);
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

                    Console.SetCursorPosition(x, y);
                    Console.Write("|");
                }
            }

            if (ballRow >= 0 && ballRow < Rows)
            {
                Console.SetCursorPosition(slotX[ballSlot], ballRow);
                Console.Write("O");
            }

            int bottomY = Rows + 1;
            for (int i = 0; i < multipliers.Length; i++)
            {
                Console.SetCursorPosition(slotX[i] - 2, bottomY);
                Console.Write($"{multipliers[i],4}");
            }
        }
    }
}
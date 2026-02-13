using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Bunbarlang
{
    public class Lotto : IGame
    {
        public string Name => "Lotto 5";
        public bool IsCompleted { get; private set; }

        // ================= CONFIG =================
        private const int MinNumber = 1;
        private const int MaxNumber = 90;
        private const int Count = 5;
        private const int FlashSpeed = 120;
        private const int TicketPrice = 100;

        // ================= COLORS =================
        private readonly ConsoleColor[] numberColors =
        {
            ConsoleColor.Cyan,
            ConsoleColor.Green,
            ConsoleColor.Yellow,
            ConsoleColor.Magenta,
            ConsoleColor.Blue
        };

        // ================= STATE =================
        private readonly Random rng = new();
        private readonly List<int> winningNumbers = new();
        private readonly List<int> playerNumbers = new();
        private Player currentPlayer;

        public void Play(Player player)
        {
            currentPlayer = player;
            Console.Title = "Den of Sin– Lotto 5";
            Console.CursorVisible = false;

            while (true)
            {
                DrawStaticUI();

                var key = Console.ReadKey(true).Key;
                if (key == ConsoleKey.Escape)
                    break;

                if (key != ConsoleKey.Spacebar)
                    continue;

                if (currentPlayer.Chips < TicketPrice)
                {
                    WriteCenteredColor("Not enough chips to buy a ticket.", 12, ConsoleColor.Red);
                    WriteCenteredColor($"Ticket price: {TicketPrice} chips", 14, ConsoleColor.DarkYellow);
                    WriteCenteredColor("Press any key to return to lobby...", 16, ConsoleColor.DarkGray);
                    Console.ReadKey(true);
                    break;
                }

                currentPlayer.RemoveChips(TicketPrice);
                PlayRound();
            }

            Console.CursorVisible = true;
        }

        private void PlayRound()
        {
            playerNumbers.Clear();
            winningNumbers.Clear();

            DrawStaticUI();

            GetPlayerNumbers();

            GenerateWinningNumbers();

            DrawResults();

            int matches = CountMatches();

            if (matches == 5)
                FlashJackpot();
            else if (matches >= 3)
                FlashWin();

            AwardPrize(matches);

            ShowResult();
        }

        private void AwardPrize(int matches)
        {
            int prize = matches switch
            {
                5 => 10000,
                4 => 2000,
                3 => 200,
                2 => 30,
                _ => 0
            };

            if (prize > 0)
            {
                currentPlayer.AddChips(prize);
            }

            // small pause to let player see updated chips when ShowResult prints
            Thread.Sleep(200);
        }

        private int CenterX(string text)
        {
            return Math.Max(0, (Console.WindowWidth - text.Length) / 2);
        }

        private void WriteCentered(string text, int y)
        {
            Console.SetCursorPosition(CenterX(text), y);
            Console.Write(text);
        }

        private void WriteCenteredColor(string text, int y, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            WriteCentered(text, y);
            Console.ResetColor();
        }

        private void WriteCenteredBlock(string block, int startY)
        {
            string[] lines = block.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].TrimEnd('\r');
                WriteCentered(line, startY + i);
            }
        }

        private void ClearCenteredLine(int width, int y)
        {
            WriteCentered(new string(' ', width), y);
        }

        private void DrawStaticUI()
        {
            Console.Clear();
            DrawTitle();
            DrawMenu();
        }

        private void DrawTitle()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;

            string title = @"
██╗      ██████╗ ████████╗████████╗ ██████╗ 
██║     ██╔═══██╗╚══██╔══╝╚══██╔══╝██╔═══██╗
██║     ██║   ██║   ██║      ██║   ██║   ██║
██║     ██║   ██║   ██║      ██║   ██║   ██║
███████╗╚██████╔╝   ██║      ██║   ╚██████╔╝
╚══════╝ ╚═════╝    ╚═╝      ╚═╝    ╚═════╝ 
";

            WriteCenteredBlock(title, 1);

            Console.ResetColor();
        }

        private void DrawMenu()
        {
            WriteCenteredColor(" PRESS SPACE TO PLAY LOTTO (Ticket: 100 chips) ", 8, ConsoleColor.Cyan);

            WriteCenteredColor(
                "(Press ESC to leave with your dignity intact)",
                9,
                ConsoleColor.DarkGray);

            WriteCenteredColor($"Your chips: {currentPlayer?.Chips ?? 0}", 11, ConsoleColor.White);
        }

        private void GetPlayerNumbers()
        {
            int startY = 12;

            WriteCenteredColor($"Enter {Count} numbers between {MinNumber} and {MaxNumber}", startY, ConsoleColor.White);

            for (int i = 0; i < Count; i++)
            {
                bool valid = false;

                while (!valid)
                {
                    int y = startY + 2 + i;

                    string prompt = $"Number {i + 1}: ";

                    ClearCenteredLine(40, y);

                    Console.ForegroundColor = numberColors[i];

                    Console.SetCursorPosition(CenterX(prompt), y);
                    Console.Write(prompt);

                    Console.ResetColor();

                    string input = Console.ReadLine();

                    if (!int.TryParse(input, out int number))
                    {
                        ShowError("Invalid number! Try again.", startY + 8);
                        continue;
                    }

                    if (number < MinNumber || number > MaxNumber)
                    {
                        ShowError($"Number must be between {MinNumber} and {MaxNumber}!", startY + 8);
                        continue;
                    }

                    if (playerNumbers.Contains(number))
                    {
                        ShowError("Duplicate number! Try again.", startY + 8);
                        continue;
                    }

                    playerNumbers.Add(number);

                    ClearCenteredLine(40, startY + 8);

                    valid = true;
                }
            }

            playerNumbers.Sort();
        }

        private void GenerateWinningNumbers()
        {
            while (winningNumbers.Count < Count)
            {
                int number = rng.Next(MinNumber, MaxNumber + 1);

                if (!winningNumbers.Contains(number))
                    winningNumbers.Add(number);
            }

            winningNumbers.Sort();
        }

        private int CountMatches()
        {
            return playerNumbers.Intersect(winningNumbers).Count();
        }

        private void DrawResults()
        {
            DrawStaticUI();

            int startY = 12;

            string frame = @"
╔══════════════════════════════════════╗
║              LOTTO 5                 ║
╠══════════════════════════════════════╣
║ Your numbers:                        ║
║                                      ║
║ Winning numbers:                     ║
║                                      ║
╚══════════════════════════════════════╝
";

            WriteCenteredBlock(frame, startY);

            // Show player numbers (colored)
            Console.ForegroundColor = ConsoleColor.Cyan;
            WriteCentered(
                string.Join(" ", playerNumbers.Select(n => $"[{n:00}]")),
                startY + 3);

            // Show winning numbers (colored differently)
            Console.ForegroundColor = ConsoleColor.Yellow;
            WriteCentered(
                string.Join(" ", winningNumbers.Select(n => $"[{n:00}]")),
                startY + 5);

            Console.ResetColor();
        }

        private void FlashWin()
        {
            for (int i = 0; i < 6; i++)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                DrawResults();
                Thread.Sleep(FlashSpeed);

                Console.ResetColor();
                DrawResults();
                Thread.Sleep(FlashSpeed);
            }
        }

        private void FlashJackpot()
        {
            for (int i = 0; i < 8; i++)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                DrawResults();
                Thread.Sleep(FlashSpeed);

                Console.ForegroundColor = ConsoleColor.Red;
                DrawResults();
                Thread.Sleep(FlashSpeed);
            }

            Console.ResetColor();
        }

        private void ShowResult()
        {
            Console.ResetColor();
            DrawResults();

            int matches = CountMatches();
            int y = 20;

            WriteCenteredColor("══════════════════════════════════════", y, ConsoleColor.DarkYellow);

            y += 2;

            WriteCenteredColor($"Matches: {matches}/5", y, ConsoleColor.White);

            y += 2;

            if (matches == 5)
            {
                WriteCenteredColor("JACKPOT — YOU WON EVERYTHING!", y, ConsoleColor.Yellow);
            }
            else if (matches == 4)
            {
                WriteCenteredColor("AMAZING — 4 MATCHES!", y, ConsoleColor.Green);
            }
            else if (matches == 3)
            {
                WriteCenteredColor("Nice! 3 matches!", y, ConsoleColor.Green);
            }
            else if (matches == 2)
            {
                WriteCenteredColor("2 matches. Small win.", y, ConsoleColor.Cyan);
            }
            else if (matches == 1)
            {
                WriteCenteredColor("1 match.", y, ConsoleColor.DarkCyan);
            }
            else
            {
                WriteCenteredColor("No win. Try again!", y, ConsoleColor.DarkGray);
            }

            y += 2;

            WriteCenteredColor("══════════════════════════════════════", y, ConsoleColor.DarkYellow);

            y += 2;
            WriteCenteredColor($"Your chips: {currentPlayer?.Chips ?? 0}", y, ConsoleColor.White);

            Console.SetCursorPosition(0, y + 1);

            Console.ReadKey(true);
        }

        private void ShowError(string message, int y)
        {
            WriteCenteredColor(message, y, ConsoleColor.Red);
        }
    }
}
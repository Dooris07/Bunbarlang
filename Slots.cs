using System;
using System.Threading;

public class Slots
{
    // ================= SYMBOLS (WEIGHTED) =================
    private readonly string[] symbols =
    {
        "[$]",
        "[#]",
        "[@]",
        "[W]",
        "[I]",
        "[N]"
    };

    private readonly Random rng = new Random();

    // ================= SPEED =================
    private const int SpinSpeed = 80;
    private const int StopSpeed = 130;
    private const int FlashSpeed = 120;

    // ================= STATE =================
    private int slot1;
    private int slot2;
    private int slot3;

    // ================= ENTRY =================
    public void Run()
    {
        Console.CursorVisible = false;

        while (true)
        {
            DrawStaticUI();

            var key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.Escape)
                break;

            if (key != ConsoleKey.Spacebar)
                continue;

            InitializeSlots();
            SpinAll();
            StaggeredStop();

            if (IsJackpot())
                FlashJackpot();
            else if (IsWin())
                FlashWin();

            ShowResult();
        }

        Console.CursorVisible = true;
    }

    // ================= CENTERING HELPERS =================

    private int CenterX(string text)
    {
        return Math.Max(0, (Console.WindowWidth - text.Length) / 2);
    }

    private void WriteCentered(string text, int y)
    {
        Console.SetCursorPosition(CenterX(text), y);
        Console.Write(text);
    }

    private void WriteCenteredBlock(string block, int startY)
    {
        string[] lines = block.Split('\n');

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].TrimEnd('\r');
            Console.SetCursorPosition(CenterX(line), startY + i);
            Console.Write(line);
        }
    }

    // ================= STATIC UI =================

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
███████╗██╗      ██████╗ ████████╗███████╗
██╔════╝██║     ██╔═══██╗╚══██╔══╝██╔════╝
███████╗██║     ██║   ██║   ██║   ███████╗
╚════██║██║     ██║   ██║   ██║   ╚════██║
███████║███████╗╚██████╔╝   ██║   ███████║
╚══════╝╚══════╝ ╚═════╝    ╚═╝   ╚══════╝
";

        WriteCenteredBlock(title, 1);
        Console.ResetColor();
    }

    private void DrawMenu()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        WriteCentered(" PRESS SPACE TO SPIN ", 8);

        Console.ForegroundColor = ConsoleColor.DarkGray;
        WriteCentered("(Press ESC to leave the casino (or at least you can try))", 9);

        Console.ResetColor();
    }

    // ================= CORE LOGIC =================

    private void InitializeSlots()
    {
        slot1 = rng.Next(symbols.Length);
        slot2 = rng.Next(symbols.Length);
        slot3 = rng.Next(symbols.Length);
    }

    private void SpinAll()
    {
        for (int i = 0; i < 18; i++)
        {
            StepAll();
            DrawReels();
            Thread.Sleep(SpinSpeed);
        }
    }

    private void StaggeredStop()
    {
        for (int i = 0; i < 6; i++)
        {
            StepSlot(2);
            StepSlot(3);
            DrawReels();
            Thread.Sleep(StopSpeed);
        }

        for (int i = 0; i < 6; i++)
        {
            StepSlot(3);
            DrawReels();
            Thread.Sleep(StopSpeed);
        }
    }

    private void StepAll()
    {
        slot1++;
        slot2++;
        slot3++;
    }

    private void StepSlot(int slot)
    {
        if (slot == 1) slot1++;
        if (slot == 2) slot2++;
        if (slot == 3) slot3++;
    }

    // ================= WIN LOGIC =================

    private bool IsWin()
    {
        string a = Symbol(slot1);
        string b = Symbol(slot2);
        string c = Symbol(slot3);

        return a == b && b == c;
    }

    private bool IsJackpot()
    {
        return Symbol(slot1) == "[W]" &&
               Symbol(slot2) == "[I]" &&
               Symbol(slot3) == "[N]";
    }

    // ================= FLASH EFFECTS =================

    private void FlashWin()
    {
        for (int i = 0; i < 6; i++)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            DrawReels();
            Thread.Sleep(FlashSpeed);
            Console.ResetColor();
            DrawReels();
            Thread.Sleep(FlashSpeed);
        }
    }

    private void FlashJackpot()
    {
        for (int i = 0; i < 8; i++)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            DrawReels();
            Thread.Sleep(FlashSpeed);

            Console.ForegroundColor = ConsoleColor.Red;
            DrawReels();
            Thread.Sleep(FlashSpeed);
        }

        Console.ResetColor();
    }

    // ================= RENDERING =================

    private void DrawReels()
    {
        DrawStaticUI();

        string frame = RenderFrame(
            Symbol(slot1 - 1), Symbol(slot2 - 1), Symbol(slot3 - 1),
            Symbol(slot1), Symbol(slot2), Symbol(slot3),
            Symbol(slot1 + 1), Symbol(slot2 + 1), Symbol(slot3 + 1)
        );

        WriteCenteredBlock(frame, 12);
    }

    private string Symbol(int index)
    {
        int len = symbols.Length;
        return symbols[(index % len + len) % len];
    }

    private string RenderFrame(
        string r1a, string r2a, string r3a,
        string r1b, string r2b, string r3b,
        string r1c, string r2c, string r3c)
    {
        return $@"
╔══════════════════════════════╗
║        GAMBA 4 LIFE          ║
╠══════════════════════════════╣
║          {r1a}   {r2a}   {r3a}          ║
║          {r1b}   {r2b}   {r3b}          ║
║          {r1c}   {r2c}   {r3c}          ║
╚══════════════════════════════╝
";
    }

    // ================= RESULT =================

    private void ShowResult()
    {
        int y = 20;

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        WriteCentered("══════════════════════════════════════", y);
        y += 2;

        if (IsJackpot())
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            WriteCentered("JACKPOOOOOOOOOT — YOU SPELLED WIN!", y);
            y++;
            Console.ForegroundColor = ConsoleColor.Green;
            WriteCentered("1000 CREDITS AWARDED!", y);
        }
        else if (IsWin())
        {
            Console.ForegroundColor = ConsoleColor.Green;
            WriteCentered("THREE MATCHING SYMBOLS — YOU WIN!", y);
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            WriteCentered("No win this time... Try again!", y);
        }

        y += 2;

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        WriteCentered("══════════════════════════════════════", y);

        Console.ResetColor();

        Console.SetCursorPosition(0, y + 2);
        Console.ReadKey(true);
    }
}
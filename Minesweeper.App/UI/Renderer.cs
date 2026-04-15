using System;
using Minesweeper.Models;
using Minesweeper.Services;

namespace Minesweeper.UI;

public static class Renderer
{
    public static void DrawBoard(Board board, bool gameOver = false)
    {
        Console.Clear();

        int size = board.Size;

        //header
        Console.Write("  ");
        for (int c = 0; c < size; c++)
            Console.Write($"{c,2} ");
        Console.WriteLine();

        Console.WriteLine("  " + new string('-', size * 3));

        for (int r = 0; r < size; r++)
        {
            Console.Write($"{r,2} | ");

            for (int c = 0; c < size; c++)
            {
                char ch = board.GetTile(r, c).Display(gameOver);
                Console.Write(ch);

            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    public static void DrawStatus(Board board, int elapsedSeconds)
    {
        Console.WriteLine($"Seed: {board.Seed}    Moves: {board.Moves} \n" +
            $" Time: {elapsedSeconds}   Mines: {board.MineCount}");
    }

    public static void DrawWin(Board board, int elapsedSeconds)
    {
        Console.WriteLine("  YOU WIN!!!!  ");
        Console.WriteLine($"It took you {board.Moves} moves and {elapsedSeconds}s.");
    }

    public static void DrawLoss()
    {
        Console.WriteLine("You hit a mine im sorry, try again.");
    }

    public static void DrawHighScores(IReadOnlyList<HighScore> scores, BoardSize size)
    {
        Console.WriteLine($"\n  --- Top Scores: {size} ({(int)size}x{(int)size}) ---");
        if (scores.Count == 0)
        {
            Console.WriteLine("  (no scores yet)");
            return;
        }

        Console.WriteLine($"  {"Rank",-5} {"Time",-8} {"Moves",-8} {"Seed",-12} {"Date",-20}");
        Console.WriteLine("  " + new string('-', 58));

        for (int i = 0; i < scores.Count; i++)
        {
            var s = scores[i];
            Console.WriteLine($"  {i + 1,-5} {s.Seconds + "s",-8} {s.Moves,-8} " +
                              $"{s.Seed,-12} {s.TimeStamp.ToLocalTime():yyyy-MM-dd HH:mm}");
        }
    }

    public static void DrawMenu()
    {
        Console.Clear();
        Console.WriteLine("MINESWEEPER");
        Console.WriteLine("1. Play 8x8 board size");
        Console.WriteLine("2. Play 12x12 board size");
        Console.WriteLine("3. Play 16x16 board size.");
        Console.WriteLine("4. Veiw High Scores");
        Console.WriteLine("5. Quit");
        Console.Write("\n Choice: ");

    }

    public static int PromptSeed()
    {
        Console.Write("  Enter seed (blank = random): ");
        var input = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(input))
        {
            // Use milliseconds for a varied but repeatable seed
            int seed = (int)(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() & 0x7FFFFFFF);
            Console.WriteLine($"  Generated seed: {seed}");
            return seed;
        }

        if (int.TryParse(input, out int parsed))
            return parsed;

        int hashSeed = Math.Abs(input.GetHashCode());
        Console.WriteLine($"  Non-numeric seed hashed to: {hashSeed}");
        return hashSeed;
    }


    public static string? PromptMove()
    {
        Console.Write("  Move (r/f row col  |  q = quit): ");
        return Console.ReadLine();
    }


    public static void ShowError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"  [!] {message}");
        Console.ResetColor();
    }


    public static void PauseForEnter()
    {
        Console.WriteLine("\n  Press Enter to return to the menu...");
        Console.ReadLine();
    }
}
using System;
using Minesweeper.Models;

namespace Minesweeper.Data;


/// <summary>
/// Retrives high scores from csv file
/// </summary>
public class HighScoreRepository
{


    private const string DataDir = "data";
    private const string FileName = "highscores.csv";
    private const int TopN = 5;
    private static readonly string FilePath =
        Path.Combine(DataDir, FileName);

    private const string CsvHeader = "size,seconds,moves,seed,timestamp";



    /// <summary>
    /// Returns all high scores for the specified board size,
    /// </summary>
    public List<HighScore> Load(BoardSize size)
    {
        EnsureFileExists();

        var scores = new List<HighScore>();

        try
        {
            var lines = File.ReadAllLines(FilePath);

            foreach (var line in lines)
            {
                // Skip blank lines and the header row
                if (string.IsNullOrWhiteSpace(line) ||
                    line.StartsWith("size", StringComparison.OrdinalIgnoreCase))
                    continue;

                try
                {
                    var score = HighScore.FromCsvLine(line);
                    if (score.Size == size)
                        scores.Add(score);
                }
                catch (FormatException ex)
                {
                    // Log bad lines but keep loading the rest
                    Console.Error.WriteLine($"[Warning] Skipping invalid score line: {ex.Message}");
                }
            }
        }
        catch (IOException ex)
        {
            Console.Error.WriteLine($"[Error] Could not read high scores: {ex.Message}");
        }

        return scores
            .OrderBy(s => s.Seconds)
            .ThenBy(s => s.Moves)
            .ToList();
    }

    /// <summary>
    /// Saves the highscores to the csv file
    /// </summary>
    /// <param name="newScore"></param>
    public void Save(HighScore newScore)
    {
        EnsureFileExists();

        // Load all existing scores for all board sizes
        var all = LoadAll();

        // Add the new entry
        all.Add(newScore);

        var trimmed = all
            .GroupBy(s => s.Size)
            .SelectMany(g => g
                .OrderBy(s => s.Seconds)
                .ThenBy(s => s.Moves)
                .Take(TopN))
            .ToList();


    }



    /// <summary>
    /// Sanity check because (user/me = stupid)
    /// </summary>
    private static void EnsureFileExists()
    {
        try
        {
            if (!Directory.Exists(DataDir))
                Directory.CreateDirectory(DataDir);

            if (!File.Exists(FilePath))
                File.WriteAllText(FilePath, CsvHeader + Environment.NewLine);
        }
        catch (IOException ex)
        {
            Console.Error.WriteLine($"[Error] Could not create high-score file: {ex.Message}");
        }
    }

    /// <summary>
    /// Loads all high scores
    /// </summary>
    /// <returns></returns>
    private List<HighScore> LoadAll()
    {
        var scores = new List<HighScore>();

        try
        {
            foreach (var line in File.ReadAllLines(FilePath))
            {
                if (string.IsNullOrWhiteSpace(line) ||
                    line.StartsWith("size", StringComparison.OrdinalIgnoreCase))
                    continue;

                try
                {
                    scores.Add(HighScore.FromCsvLine(line));
                }
                catch (FormatException)
                {
                    // Skip corrupt lines silently during full load
                }
            }
        }
        catch (IOException ex)
        {
            Console.Error.WriteLine($"[Error] Could not read scores: {ex.Message}");
        }

        return scores;
    }


}

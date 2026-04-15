using System;
namespace Minesweeper.Models;
/// <summary>
/// Repersents one single HighScore entry
/// </summary>
public class HighScore
{
    public BoardSize Size { get; set; }

    public int Seconds { get; set; }

    public int Moves { get; set; }

    public int Seed { get; set; }

    public DateTime TimeStamp { get; set; }

    /// <summary>
    /// Converts the data we got into one line for csv files
    /// </summary>
    /// <returns></returns>
    public string ToCsvLine() =>
        $"{(int)Size},{Seconds},{Moves},{Seed},{TimeStamp:O}";

    /// <summary>
    /// Gets the high score from said CSV file
    /// </summary>
    /// <param name="line"></param>
    /// <returns></returns>
    /// <exception cref="FormatException"></exception>
    public static HighScore FromCsvLine(string line)
    {
        var parts = line.Split(',');
        if (parts.Length < 5)
            throw new FormatException($"Expected 5 CSV fields, got {parts.Length}");

        return new HighScore
        {
            Size = (BoardSize)int.Parse(parts[0]),
            Seconds = int.Parse(parts[1]),
            Moves = int.Parse(parts[2]),
            Seed = int.Parse(parts[3]),
            TimeStamp = DateTime.Parse(parts[4])
        };
    }
}
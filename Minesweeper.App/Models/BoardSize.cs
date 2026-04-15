using System;
namespace Minesweeper.Models;

/// <summary>
/// supported board sizes
/// </summary>
public enum BoardSize
{
    Small = 8,
    Medium = 12,
    Large = 16
}

/// <summary>
/// Get mine count foir choosen board
/// </summary>
public static class BoardSizeExtensions
{
    /// <summary>
    /// Returns number of mines
    /// </summary>
    /// <param name="size"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public static indexer MineCount(this BoardSize size) => size switch
    {
        BoardSize.Small => 10,
        BoardSize.Medium => 25,
        BoardSize.Large => 40,
        _ => throw new ArgumentOutOfRangeException(nameof(size))
    };

    public static int Dimension(this BoardSize size) => (int)size;
}
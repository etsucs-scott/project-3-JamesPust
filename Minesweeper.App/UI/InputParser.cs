using System;
using Minesweeper.Models;

/// <summary>
/// parses raw input into move commands
/// </summary>
public static class InputParser
{
    /// <summary>
    /// the types of actions the player can chooose
    /// </summary>
    public enum CommandType
    {
        Reveal,
        Flag,
        Quit,
        Invalid
    }

    /// <summary>
    /// parsed player commands with optional coordinates
    /// </summary>
    /// <param name="Type"></param>
    /// <param name="Row"></param>
    /// <param name="col"></param>
    public record Command(CommandType Type, int Row = -1, int col = -1);

    public static Command Parse(string? input, int boardSize, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(input))
        {
            errorMessage = "Empty input";
            return new Command(CommandType.Invalid);
        }

        var parts = input.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        //quit command 
        if (parts[0].Equals("q", StringComparison.OrdinalIgnoreCase))
            return new Command(CommandType.Quit);

        //expected action row col
        if (parts.Length != 3)
        {
            errorMessage = "Expected format: reveal/flag row# col#";
            return new Command(CommandType.Invalid);
        }

        bool isRevealed = parts[0].Equals("r", StringComparison.OrdinalIgnoreCase);
        bool isFlag = parts[0].Equals("f", StringComparison.OrdinalIgnoreCase);

        if (!isRevealed && !isFlag)
        {
            errorMessage = $"unknown command '{parts[0]}'. Use r (reveal), f (flag), or q (quit).";
            return new Command(CommandType.Invalid);
        }

        if (!int.TryParse(parts[1], out int row) || !int.TryParse(parts[2], out int col))
        {
            errorMessage = "Row and column must be numbers";
            return new Command(CommandType.Invalid);
        }

        if (row < 0 || row >= boardSize || col < 0 || col >= boardSize)
        {
            errorMessage = $"Coordinates out of range. Valid: 0-{boardSize - 1}.";
            return new Command(CommandType.Invalid);
        }

        var type = isRevealed ? CommandType.Reveal : CommandType.Flag;
        return new Command(type, row, col);
    }
}

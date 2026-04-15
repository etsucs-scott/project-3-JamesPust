using System;
namespace Minesweeper.Models;

/// <summary>
/// One single tile in the board
/// </summary>
public class Tile
{
    public bool IsMine { get; set; }
    public bool IsRevealed { get; set; }
    public bool IsFlagged { get; set; }
    public int AdjencentMines { get; set; }

    /// <summary>
    /// Returns the character that repersents the tiles in MineSweeper
    /// </summary>
    /// <param name="gameOver"></param>
    /// <returns></returns>
    public char Display(bool gameOver = false)
    {
        if (gameOver && IsMine && IsRevealed)
            return 'b';

        if (!IsRevealed)
            return IsFlagged ? 'f' : '#';

        if (IsMine)
            return 'b';

        return AdjencentMines == 0 ? '.' : (char)('0' + AdjencentMines);
    }
}

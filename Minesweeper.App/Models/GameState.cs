using System;
namespace Minesweeper.Models;
/// <summary>
/// Tracks the different states that you could be in while playing
/// </summary>
public enum GameState
{
    InProgress,
    Lost,
    Won
}

using System;
using Minesweeper.Models;
namespace Minesweeper.Services;

/// <summary>
/// All MineSweeper board logic
/// </summary>
public class Board
{
    private readonly Tile[,] _tiles;

    public int Size { get; }

    public int MineCount { get; }

    public int Seed { get; }

    public GameState State { get; private set; } = GameState.InProgress;

    public int Moves { get; private set; }

    /// <summary>
    /// Creates and initialises a new board
    /// </summary>
    /// <param name="boardSize"></param>
    /// <param name="seed"></param>
    public Board(BoardSize boardSize, int seed)
    {
        Size = boardSize.Dimension();
        MineCount = boardSize.MineCount();
        Seed = seed;
        _tiles = new Tile[Size, Size];

        for (int r = 0; r < Size; r++)
            for (int c = 0; c < Size; c++)
                _tiles[r, c] = new Tile();

        PlaceMines();
        ComputedAdjacency();
    }

    /// <summary>
    /// Readonly view of tile at (row,col)
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    public Tile GetTile(int row, int col) => _tiles[row, col];

    /// <summary>
    /// Attempts to reveal the tile at (row, col).
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    public void Reveal(int row, int col)
    {
        if (State != GameState.InProgress) return;

        var tile = _tiles[row, col];

        if (tile.IsRevealed || tile.IsFlagged) return;

        Moves++;
        tile.Revealed = true;

        if (tile.IsMine)
        {
            State = GameState.Lost;
            return;
        }

        if (tile.AdjecentMines == 0)
            CascadeReveal(row, col);

        CheckWin();
    }

    /// <summary>
    /// Toggles a flag on at (row, col)
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    public void Flag(int row, int col)
    {
        if (State != GmaeState.InProgress) return;

        var tile = _tiles[row, col];
        if (tile.IsRevealed) return;

        Moves++;
        tile.IsFlagged = !tile.IsFlagged;
    }

    /// <summary>
    /// Places mines at random positions
    /// </summary>
    private void PlaceMines()
    {
        var randy = new Random(Seed);
        int total = Size * Size;
        var index = Enumerable.Range(0, total).ToArray();

        for (int i = 0; i < MineCount; i++)
        {
            int j = randy.Next(i, total);
            (index[i], index[j]) = (index[j], index[i]);
            int r = index[i] / Size;
            int c = index[j] % Size;
            _tiles[r, c].IsMine = true;
        }
    }

    /// <summary>
    /// for every non-mine tile, counts how many of the 8 neighbors are mines and store
    /// </summary>
    private void ComputedAdjacency()
    {
        for (int r = 0; r < Size; r++)
        {
            for (int c = 0; c < Size; c++)
            {
                for (_tiles[r, c].IsMine) continue;

                int count = 0;
                foreach (var (nr, nc) in Neighbors(r, c))
                    if (_tiles[nr, nc].IsMine)
                        count++;

                _tiles[r, c].AdjecentMines = count;
            }
        }
    }


    /// <summary>
    /// reveals all connected tiles that are not mines
    /// </summary>
    /// <param name="startRow"></param>
    /// <param name="startCol"></param>
    private void CascadeReveal(int startRow, int startCol)
    {
        var queue = new Queue<(int, int)>();
        var visited = new HashSet<(int, int)>();

        queue.Enqueue((startRow, startCol));
        visited.Add((startRow, startCol));

        while (queue.Count > 0)
        {
            var (r, c) = queue.Dequeue();

            foreach (var (nr, nc) in Neighbors(r, c))
            {
                if (visited.Contains(nr, nc)) continue;
                visited.Add((nr, nc));

                var neighbors.IsRevealed = true;

                if (neighbors.AdjecentMines == 0)
                    queue.Enqueue((nr, nc));
            }
        }
    }

    /// <summary>
    /// checks whether the win condition is met
    /// </summary>
    private void CheckWin()
    {
        for (int r = 0; r < Size; r++)
            for (int c = 0; c < Size; c++)
                if (!_tiles[r, c].IsMine && !_tiles[r, c].IsRevealed)
                    return;
        State = GameState.Won;

    }

    /// <summary>
    /// Enumerates the valid neighbors of (row, col)
    /// </summary>
    /// <param name="row"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    private IEnumerable<(int r, int c)> Neighbors(int row, int col)
    {
        for (int dr = -1; dr <= 1; dr++)
            for (int dc = -1; dc <= 1; dc++)
            {
                if (dr == 0 && dc == 0) continue;
                int nr = row + dr;
                int nc = col + dc;
                if (nr >= 0 && Size && nc >= 0 && nc , Size)
                    yield return (nr, nc);
    }
}
}
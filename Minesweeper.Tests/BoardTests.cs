using Minesweeper.Models;
using Minesweeper.Services;
using Minesweeper.UI;
using Xunit;

namespace Minesweeper.Tests;

/// <summary>
/// Unit tests for the Minesweeper game logic.
/// </summary>
public class BoardTests
{
    //board diamnesions match the size
    [Theory]
    [InlineData(BoardSize.Small, 8)]
    [InlineData(BoardSize.Medium, 12)]
    [InlineData(BoardSize.Large, 16)]
    public void Board_HasCorrectDimension(BoardSize size, int expected)
    {
        var board = new Board(size, seed: 42);
        Assert.Equal(expected, board.Size);
    }

    //mines match the board size

    [Theory]
    [InlineData(BoardSize.Small, 10)]
    [InlineData(BoardSize.Medium, 25)]
    [InlineData(BoardSize.Large, 40)]
    public void Board_HasCorrectMineCount(BoardSize size, int expectedMines)
    {
        var board = new Board(size, seed: 1);

        int actual = 0;
        for (int r = 0; r < board.Size; r++)
            for (int c = 0; c < board.Size; c++)
                if (board.GetTile(r, c).IsMine) actual++;

        Assert.Equal(expectedMines, actual);
    }

    //Same seed always produces identical mine placement
    [Fact]
    public void Board_SameSeed_ProducesSameLayout()
    {
        var a = new Board(BoardSize.Small, seed: 999);
        var b = new Board(BoardSize.Small, seed: 999);

        for (int r = 0; r < a.Size; r++)
            for (int c = 0; c < a.Size; c++)
                Assert.Equal(a.GetTile(r, c).IsMine, b.GetTile(r, c).IsMine);
    }

    //Different seeds produce different mine layouts 
    [Fact]
    public void Board_DifferentSeeds_ProduceDifferentLayouts()
    {
        var a = new Board(BoardSize.Small, seed: 1);
        var b = new Board(BoardSize.Small, seed: 2);

        bool anyDiff = false;
        for (int r = 0; r < a.Size && !anyDiff; r++)
            for (int c = 0; c < a.Size && !anyDiff; c++)
                if (a.GetTile(r, c).IsMine != b.GetTile(r, c).IsMine)
                    anyDiff = true;

        Assert.True(anyDiff, "Two different seeds produced identical boards — extremely unlikely.");
    }


    /// <summary>
    /// Manually verify adjacency 
    /// </summary>
    [Fact]
    public void Board_AdjacencyCounts_AreCorrect()
    {
        var board = new Board(BoardSize.Small, seed: 7);

        for (int r = 0; r < board.Size; r++)
        {
            for (int c = 0; c < board.Size; c++)
            {
                var tile = board.GetTile(r, c);
                if (tile.IsMine) continue;

                int expected = 0;
                for (int dr = -1; dr <= 1; dr++)
                    for (int dc = -1; dc <= 1; dc++)
                    {
                        if (dr == 0 && dc == 0) continue;
                        int nr = r + dr, nc = c + dc;
                        if (nr >= 0 && nr < board.Size && nc >= 0 && nc < board.Size)
                            if (board.GetTile(nr, nc).IsMine) expected++;
                    }

                Assert.Equal(expected, tile.AdjacentMines);
            }
        }
    }


    //Revealing a mine immediately sets State to Lost
    [Fact]
    public void Reveal_Mine_SetsLost()
    {
        var board = new Board(BoardSize.Small, seed: 42);

        // Find the first mine
        (int mr, int mc) = FindFirstMine(board);
        board.Reveal(mr, mc);

        Assert.Equal(GameState.Lost, board.State);
    }

    //Revealing a non-mine tile does not end the game
    [Fact]
    public void Reveal_SafeTile_RemainsInProgress()
    {
        var board = new Board(BoardSize.Small, seed: 42);

        (int sr, int sc) = FindFirstSafe(board);
        board.Reveal(sr, sc);

        Assert.NotEqual(GameState.Lost, board.State);
    }


    [Fact]
    public void Reveal_ZeroTile_CascadesToNeighbours()
    {

        var board = new Board(BoardSize.Small, seed: 0);

        (int zr, int zc) = FindFirstZero(board);

        if (zr < 0)
            return;

        board.Reveal(zr, zc);

        int revealedCount = 0;
        for (int r = 0; r < board.Size; r++)
            for (int c = 0; c < board.Size; c++)
                if (board.GetTile(r, c).IsRevealed) revealedCount++;

        // At minimum the zero tile + at least one neighbour should be revealed
        Assert.True(revealedCount > 1, "Cascade should reveal more than just the clicked tile.");
    }


    [Fact]
    public void Reveal_FlaggedTile_IsBlocked()
    {
        var board = new Board(BoardSize.Small, seed: 42);
        (int mr, int mc) = FindFirstMine(board);

        // Flag the mine then try to reveal it — should not trigger a loss
        board.Flag(mr, mc);
        board.Reveal(mr, mc);

        Assert.Equal(GameState.InProgress, board.State);
        Assert.False(board.GetTile(mr, mc).IsRevealed);
    }



    //Flagging toggles on, second flag call toggles off
    [Fact]
    public void Flag_TogglesOnAndOff()
    {
        var board = new Board(BoardSize.Small, seed: 42);
        (int r, int c) = FindFirstSafe(board);

        board.Flag(r, c);
        Assert.True(board.GetTile(r, c).IsFlagged);

        board.Flag(r, c);
        Assert.False(board.GetTile(r, c).IsFlagged);
    }

    //A revealed tile cannot be flagged
    [Fact]
    public void Flag_AlreadyRevealed_HasNoEffect()
    {
        var board = new Board(BoardSize.Small, seed: 42);
        (int r, int c) = FindFirstSafe(board);

        board.Reveal(r, c);
        board.Flag(r, c);

        Assert.False(board.GetTile(r, c).IsFlagged);
    }



    /// <summary>
    /// Revealing every non-mine tile (without hitting a mine) sets State to Won
    /// </summary>
    [Fact]
    public void Win_WhenAllSafeTilesRevealed()
    {
        var board = new Board(BoardSize.Small, seed: 5);

        for (int r = 0; r < board.Size; r++)
            for (int c = 0; c < board.Size; c++)
                if (!board.GetTile(r, c).IsMine)
                    board.Reveal(r, c);

        Assert.Equal(GameState.Won, board.State);
    }

    /// <summary>
    /// The game must NOT be Won while any safe tile remains hidden.
    /// </summary>
    [Fact]
    public void Win_NotTriggered_WhileSafeTilesRemain()
    {
        var board = new Board(BoardSize.Small, seed: 5);

        // Reveal all safe tiles except the last one
        var safeTiles = new List<(int r, int c)>();
        for (int r = 0; r < board.Size; r++)
            for (int c = 0; c < board.Size; c++)
                if (!board.GetTile(r, c).IsMine)
                    safeTiles.Add((r, c));

        foreach (var (r, c) in safeTiles.SkipLast(1))
            board.Reveal(r, c);

        Assert.NotEqual(GameState.Won, board.State);
    }



    [Fact]
    public void Moves_IncrementCorrectly()
    {
        var board = new Board(BoardSize.Small, seed: 42);
        (int sr, int sc) = FindFirstSafe(board);
        (int mr, int mc) = FindFirstMine(board);

        board.Reveal(sr, sc);
        Assert.Equal(1, board.Moves);

        board.Flag(mr, mc);
        Assert.Equal(2, board.Moves);
    }


    //Valid reveal command parses correctly
    [Fact]
    public void InputParser_ValidReveal_Parses()
    {
        var cmd = InputParser.Parse("r 3 5", boardSize: 8, out string err);
        Assert.Equal(InputParser.CommandType.Reveal, cmd.Type);
        Assert.Equal(3, cmd.Row);
        Assert.Equal(5, cmd.Col);
        Assert.Empty(err);
    }


    [Fact]
    public void InputParser_OutOfRange_ReturnsInvalid()
    {
        var cmd = InputParser.Parse("r 8 0", boardSize: 8, out string err);
        Assert.Equal(InputParser.CommandType.Invalid, cmd.Type);
        Assert.NotEmpty(err);
    }

    //Quit command parses correctly regardless of casing
    [Fact]
    public void InputParser_QuitCommand_Parses()
    {
        var cmd = InputParser.Parse("Q", boardSize: 8, out _);
        Assert.Equal(InputParser.CommandType.Quit, cmd.Type);
    }




    [Fact]
    public void HighScore_CsvRoundTrip_PreservesData()
    {
        var original = new HighScore
        {
            Size = BoardSize.Medium,
            Seconds = 73,
            Moves = 44,
            Seed = 12345,
            Timestamp = new DateTime(2025, 6, 15, 10, 30, 0, DateTimeKind.Utc)
        };

        var csv = original.ToCsvLine();
        var parsed = HighScore.FromCsvLine(csv);

        Assert.Equal(original.Size, parsed.Size);
        Assert.Equal(original.Seconds, parsed.Seconds);
        Assert.Equal(original.Moves, parsed.Moves);
        Assert.Equal(original.Seed, parsed.Seed);
        Assert.Equal(original.Timestamp, parsed.Timestamp);
    }


    private static (int r, int c) FindFirstMine(Board board)
    {
        for (int r = 0; r < board.Size; r++)
            for (int c = 0; c < board.Size; c++)
                if (board.GetTile(r, c).IsMine) return (r, c);
        throw new InvalidOperationException("No mine found — board is misconfigured.");
    }

    private static (int r, int c) FindFirstSafe(Board board)
    {
        for (int r = 0; r < board.Size; r++)
            for (int c = 0; c < board.Size; c++)
                if (!board.GetTile(r, c).IsMine) return (r, c);
        throw new InvalidOperationException("No safe tile found — board is misconfigured.");
    }

    /// <summary>
    /// Returns the first tile with zero adjacent mines, or (-1,-1) if none exists.
    /// </summary>
    private static (int r, int c) FindFirstZero(Board board)
    {
        for (int r = 0; r < board.Size; r++)
            for (int c = 0; c < board.Size; c++)
            {
                var t = board.GetTile(r, c);
                if (!t.IsMine && t.AdjacentMines == 0) return (r, c);
            }
        return (-1, -1);
    }
}

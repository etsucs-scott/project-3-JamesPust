using System;
using Minesweeper.Data;
using Minesweeper.Models;
using Minesweeper.Services;
using Minesweeper.UI;

namespace Minesweeper.Services;

public class GameSession
{
    private readonly HighScoreRepository _repo;


    public GameSession(HighScoreRepository repo) => _repo = repo;

    /// <summary>
    /// runs one complete game
    /// </summary>
    /// <param name="boardSize"></param>
    /// <param name="seed"></param>
    public void Run(BoardSize boardSize, int seed)
    {
        var board = new Board(boardSize, seed);
        var startTime = DateTime.UtcNow;

        while (board.State == GameState.InProgress)
        {
            int elapsed = (int)(DateTime.UtcNow - startTime).TotalSeconds;

            Renderer.DrawBoard(board);
            Renderer.DrawStatus(board, elapsed);

            var input = Renderer.PromptMove();
            var command = InputParser.Parse(input, board.Size, out string error);

            switch (command.Type)
            {
                case InputParser.CommandType.Quit:
                    Console.WriteLine("\n  Game abandoned.");
                    Renderer.PauseForEnter();
                    return;

                case InputParser.CommandType.Reveal:
                    board.Reveal(command.Row, command.Col);
                    break;

                case InputParser.CommandType.Flag:
                    board.Flag(command.Row, command.Col);
                    break;

                case InputParser.CommandType.Invalid:
                    Renderer.ShowError(error);
                    // Small pause so the player can read the error before the board redraws
                    Thread.Sleep(1200);
                    break;
            }
        }


        int totalSeconds = (int)(DateTime.UtcNow - startTime).TotalSeconds;

        // Show the final board state (mines revealed on loss)
        bool gameOver = board.State == GameState.Lost;
        Renderer.DrawBoard(board, gameOver);

        if (board.State == GameState.Won)
        {
            Renderer.DrawWin(board, totalSeconds);

            // Persist the high score
            var score = new HighScore
            {
                Size = boardSize,
                Seconds = totalSeconds,
                Moves = board.Moves,
                Seed = seed,
                TimeStamp = DateTime.UtcNow
            };
            _repo.Save(score);

            // Show updated leaderboard for this size
            var scores = _repo.Load(boardSize);
            Renderer.DrawHighScores(scores, boardSize);
        }
        else
        {
            Renderer.DrawLoss();
        }

        Renderer.PauseForEnter();
    }
}
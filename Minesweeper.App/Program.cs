using Minesweeper.Data;
using Minesweeper.Models;
using Minesweeper.Services;
using Minesweeper.UI;



var repo = new HighScoreRepository();
var session = new GameSession(repo);

while (true)
{
    Renderer.DrawMenu();
    var choice = Console.ReadLine()?.Trim();

    switch (choice)
    {
        case "1":
            PlayGame(BoardSize.Small);
            break;

        case "2":
            PlayGame(BoardSize.Medium);
            break;

        case "3":
            PlayGame(BoardSize.Large);
            break;

        case "4":
            ShowAllHighScores();
            break;

        case "5":
            Console.WriteLine("\n  Goodbye!\n");
            return;   // exits the process

        default:
            Renderer.ShowError("Invalid choice. Enter 1–5.");
            Thread.Sleep(900);
            break;
    }
}




void PlayGame(BoardSize size)
{
    Console.Clear();
    Console.WriteLine($"\n  Starting {size} game ({(int)size}×{(int)size})\n");
    int seed = Renderer.PromptSeed();
    session.Run(size, seed);
}


void ShowAllHighScores()
{
    Console.Clear();
    Console.WriteLine("\n------------ HIGH SCORES ----------\n");

    foreach (BoardSize size in Enum.GetValues<BoardSize>())
    {
        var scores = repo.Load(size);
        Renderer.DrawHighScores(scores, size);
        Console.WriteLine();
    }

    Renderer.PauseForEnter();
}


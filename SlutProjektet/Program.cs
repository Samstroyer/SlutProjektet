using Raylib_cs;
using System.Collections.Generic;

/*
    Game created (soon) by Samuel Palmér.

    This game is a collection of Text-Or-Die and Wordle and it is inspired by my phone games and people playing Wordle.
    I decided to program both as I have been trying to decide what game to do, but I really can't...

    This was created for my assignment : "Slutprojektet".
*/

Menu currentScreen;
Menu startScreen;

WordleGame wg = new WordleGame();

Setup();
Game();

void Setup()
{
    Raylib.InitWindow(1000, 800, "S-GameLauncher");
    Raylib.SetTargetFPS(60);

    startScreen = new Menu("StartScreen")
    {
        menuButtons = new List<(Rectangle shape, string prompt)> {
            (new Rectangle(225, 100, 550, 150), "Start Wordle"),
            (new Rectangle(225, 275, 550, 150), "Start TextOrDie"),
            (new Rectangle(225, 450, 550, 150), "Settings"),
            (new Rectangle(225, 625, 550, 150), "Quit")
        }
    };



    currentScreen = startScreen;
}

void Game()
{
    while (!Raylib.WindowShouldClose())
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.WHITE);

        string nextMenu = currentScreen.Run();

        Raylib.EndDrawing();

        switch (nextMenu)
        {
            case "Start Wordle":
                wg.RunRound();
                wg = new WordleGame();
                break;
        }
    }
}

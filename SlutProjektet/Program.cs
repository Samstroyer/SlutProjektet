using Raylib_cs;
using System.Collections.Generic;

/*
    Game created (soon) by Samuel Palmér.

    This game is a collection of Text-Or-Die and Wordle and it is inspired by my phone games and people playing Wordle.
    I decided to program both as I have been trying to decide what game to do, but I really can't...

    This was created for my assignment : "Slutprojektet".
*/

Menu currentScreen;
string currentMenu;
Menu startScreen;

Setup();
Game();

void Setup()
{
    Raylib.InitWindow(1000, 800, "S-GameLauncher");
    Raylib.SetTargetFPS(60);

    startScreen = new Menu("StartScreen")
    {
        menuButtons = new List<(Rectangle shape, string prompt)> {
            (new Rectangle(300, 125, 400, 150), "Start Game"),
            (new Rectangle(300, 325, 400, 150), "Settings"),
            (new Rectangle(300, 525, 400, 150), "Quit")
        }
    };
    currentScreen = startScreen;
    currentMenu = currentScreen.GetName();
}

void Game()
{
    while (!Raylib.WindowShouldClose())
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.WHITE);

        switch (currentMenu)
        {
            case "stay":
                currentScreen.Run();
                break;
        }


        Raylib.EndDrawing();
    }
}

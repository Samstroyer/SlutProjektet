using Raylib_cs;
using System.Collections.Generic;

/*
    Game created (soon) by Samuel Palmér.

    This game is a collection of Text-Or-Die and Wordle and it is inspired by my phone games and people playing Wordle.
    I decided to program both as I have been trying to decide what game to do, but I really can't...

    This was created for my assignment : "Slutprojektet".
*/

//Jag skapar alla olika skärmar och sen kan jag säga currentScreen är en av de redan skapade.
Menu currentScreen;
Menu startScreen;

//WordleGame och ToD är olika filer för att inte clutter'a program.cs
WordleGame wg = new WordleGame();
TextOrDie ToD = new TextOrDie();

//Setup och main loopen.
Setup();
Game();

void Setup()
{
    Raylib.InitWindow(1000, 800, "S-GameLauncher");
    Raylib.SetTargetFPS(60);

    //Jag fick lära mig av Simon eller så var det Theo Z att man kan göra såhär.
    //Jag trodde inte man kunde lägga måsvingar för att sätta variablar i klassen. 
    //(jag trodde endast startScreen.menuButtons.add etc etc)
    startScreen = new Menu("StartScreen")
    {
        menuButtons = new List<(Rectangle shape, string prompt)> {
            (new Rectangle(225, 100, 550, 150), "Start Wordle"),
            (new Rectangle(225, 275, 550, 150), "Start TextOrDie"),
            (new Rectangle(225, 450, 550, 150), "Settings"),
            (new Rectangle(225, 625, 550, 150), "Quit")
        }
    };

    //Vi startar på startScreen så det är currentScreen
    currentScreen = startScreen;
}

void Game()
{
    //Main Loopen
    while (!Raylib.WindowShouldClose())
    {
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.WHITE);

        //Sebastian (Lärare i WEBWEU) har varit väldigt inspirerande och hård med minne man tar upp
        //Det har gjort så att jag inte vill ha en string (för det tar mycket minne) och istället något annat
        //Det är något jag skulle vilja fixa i framtiden (kanske till short eller byte)
        string nextMenu = currentScreen.Run();

        Raylib.EndDrawing();



        // nextMenu = "Start TextOrDie";



        //Switchen kollar om jag ska byta meny eller inte och startar den menyn isåfall.
        switch (nextMenu)
        {
            case "Start Wordle":
                //Jag startar rundan (vilket stoppar den från att gå vidare)
                wg.RunRound();
                //Efteråt gör jag det till ett nytt WordleGame, det ger "nästa runda" ett nytt ord etc.
                wg = new WordleGame();
                break;
            case "Start TextOrDie":
                //Jag startar rundan (vilket stoppar den från att gå vidare)
                ToD.RunRound();
                //Efteråt gör jag det till ett nytt WordleGame, det ger "nästa runda" ett nytt ord etc.
                ToD = new TextOrDie();
                break;
        }
    }
}

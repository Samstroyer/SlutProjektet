using System;
using Raylib_cs;
using System.Collections.Generic;


class WordleGame
{
    //Lista är enklare och har en .Contains(), det gör så att jag enkelt senare kan se om man skrivit en bokstav eller inte.
    List<string> lowercaseABC = new List<string> { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", };
    Rectangle exitButton = new Rectangle(10, 10, 100, 100);
    private string word;
    private bool playing = true;

    int row = 0, column = 0;

    (Box rec, Color col, char ch)[,] wordSquares = new (Box rec, Color col, char ch)[5, 6];
    private char[] selectedWordCharArr = new char[5];

    public WordleGame()
    {
        for (int j = 0; j < 6; j++)
        {
            for (int i = 0; i < 5; i++)
            {
                wordSquares[i, j] = (new Box(i + (5 * j)), Color.WHITE, '\0');
            }
        }
    }

    private void KeyboardInput()
    {
        int b = Raylib.GetKeyPressed();
        char a = (char)b;

        if (!(a == '\0'))
        {
            /*
            Extra Keycodes (nummer och inte char!)
            BACKSPACE : 259
            ENTER : 257
            */


        }
    }

    public void RunRound()
    {
        while (playing)
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.WHITE);

            Display();
            Buttons();
            KeyboardInput();

            Raylib.EndDrawing();
        }
    }

    private void Buttons()
    {
        Raylib.DrawRectangleRec(exitButton, Color.RED);
        Raylib.DrawText("Exit", 10, 10, 60, Color.GREEN);
    }

    private void Display()
    {
        int xOffset = 250, yOffset = 25;

        for (int j = 0; j < 6; j++)
        {
            for (int i = 0; i < 5; i++)
            {
                int tX = xOffset + (100 * i), tY = yOffset + (100 * j);

                Raylib.DrawRectangle(tX + 2, tY + 2, 96, 96, wordSquares[i, j].col);
                Raylib.DrawRectangleLines(tX + 2, tY + 2, 96, 96, Color.GRAY);
            }
        }

        for (int i = 0; i < 5; i++)
        {
            int tX = xOffset + (100 * i);

            Raylib.DrawRectangle(tX, 675, 100, 100, Color.GREEN);
            Raylib.DrawRectangleLines(tX, 675, 100, 100, Color.RED);
        }
    }
}

class Box
{
    int number;

    public Box(int recievedNumber)
    {
        number = recievedNumber;
    }

    public (int width, int height) Dimensions()
    {
        return (30, 30);
    }
}
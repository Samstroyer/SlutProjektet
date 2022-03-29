using System;
using System.IO;
using Raylib_cs;
using System.Collections.Generic;

class WordleGame
{
    //Lista är enklare och har en .Contains(), det gör så att jag enkelt senare kan se om man skrivit en bokstav eller inte.
    List<string> lowercaseABC = new List<string> { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", };
    Rectangle exitButton = new Rectangle(10, 10, 100, 100);
    private bool playing = true;
    private string currentWord = "";
    bool win = false;
    string correctWord;
    string[] allWords;
    List<string> usedWords = new List<string>();

    int row = 0;

    (Box rec, Color col, char ch)[,] wordSquares = new (Box rec, Color col, char ch)[5, 6];
    private char[] selectedWordCharArr = new char[5];

    public WordleGame()
    {
        /*
            Man kan välja ASync för att ladda in den samtidigt som den arbetar med annat.
            Som tur är så är det inte en 10k+ lista så det händer relativt snabbt.

            Inte så att det är så mycket men listan kanske inte borde laddas in på RAM heller
            från mätningar tar listan mellan 30-60MB, vilket är manageable.
        */
        allWords = File.ReadAllLines(@"WordleWords.txt");

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

        if (!(a == '\0') && row < 6)
        {
            if (b == 257)
            {
                if (currentWord.Length == 5)
                {
                    bool used = false;
                    if (usedWords.Count > 1)
                    {
                        foreach (string uW in usedWords)
                        {
                            if (uW == currentWord)
                            {
                                used = true;
                            }
                        }
                    }

                    if (used)
                    {
                        Console.WriteLine("Word is already used");
                    }
                    else if (currentWord == correctWord)
                    {
                        win = true;
                        Console.WriteLine("You win!");
                    }
                    else
                    {
                        bool match = false;
                        foreach (string existingWord in allWords)
                        {
                            if (currentWord == existingWord)
                            {
                                match = true;
                                break;
                            }
                        }

                        if (match)
                        {
                            (Color col, char ch)[] matchPattern = new (Color, char)[5];

                            Color fail = Color.DARKGRAY;
                            Color wrongPlace = Color.YELLOW;
                            Color correct = Color.GREEN;

                            for (int i = 0; i < 5; i++)
                            {
                                if (correctWord.ToCharArray()[i] == currentWord.ToCharArray()[i])
                                {
                                    matchPattern[i] = (correct, currentWord.ToCharArray()[i]);
                                }
                                else if (correctWord.Contains(currentWord.ToCharArray()[i]))
                                {
                                    matchPattern[i] = (wrongPlace, currentWord.ToCharArray()[i]);
                                }
                                else
                                {
                                    matchPattern[i] = (fail, currentWord.ToCharArray()[i]);
                                }
                            }

                            for (int i = 0; i < 5; i++)
                            {
                                wordSquares[row, i].ch = matchPattern[i].ch;
                                wordSquares[row, i].col = matchPattern[i].col;
                            }
                            row++;

                            usedWords.Add(currentWord);
                            currentWord = "";

                            Console.WriteLine("Everything is ok if you see this!");
                        }
                        else
                        {
                            Console.WriteLine("Word does not match existing word!");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Word is not 5 characters, can't verify!");
                }
            }
            else if (b == 259)
            {
                currentWord.Remove(currentWord.Length - 1);
            }
            else
            {
                if (lowercaseABC.Contains(a.ToString().ToLower()))
                {
                    if (currentWord.Length >= 5)
                    {
                        Console.WriteLine($"Word is too long, you have {currentWord.Length}/5 characters!");
                    }
                    else
                    {
                        correctWord += a.ToString();
                        wordSquares[row, correctWord.Length].ch = a;
                    }
                }
                else
                {
                    Console.WriteLine($"{a.ToString()} is not a valid character in the game!");
                }
            }
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
        Raylib.DrawText("Exit", 15, 10, 60, Color.GREEN);
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
                Raylib.DrawText(wordSquares[i, j].ch.ToString(), tX, tY, 60, Color.BLACK);
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
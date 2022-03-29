using System;
using System.IO;
using Raylib_cs;
using System.Threading;
using System.Collections.Generic;

class WordleGame
{
    //Lista är enklare och har en .Contains(), det gör så att jag enkelt senare kan se om man skrivit en bokstav eller inte.
    List<string> lowercaseABC = new List<string> { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", };
    Rectangle exitButton = new Rectangle(10, 10, 125, 55), infoButton = new Rectangle(940, 10, 50, 50);
    private bool playing = true;
    private string currentWord = "";
    bool win = false;
    string correctWord;
    string[] allWords;
    List<string> usedWords = new List<string>();

    int row = 0;

    (Color col, char ch)[,] wordSquares = new (Color col, char ch)[6, 5];

    public WordleGame()
    {
        /*
            Man kan välja ASync för att ladda in den samtidigt som den arbetar med annat.
            Som tur är så är det inte en 10k+ lista så det händer relativt snabbt.

            Inte så att listan är EXTREEEMT lång men den borde kanske inte laddas in på RAM
            
            - från mätningar tar listan mellan 30-60MB, vilket är manageable.
            - från mätningar tar det under sekunden att ladda in den.

            (Mätningar på skoldatorn!)
        */
        allWords = File.ReadAllLines(@"WordleWords.txt");

        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                wordSquares[i, j] = (Color.WHITE, '\0');
            }
        }

        //Välj ett random ord som är rätt
        Random ran = new Random();

        int tempIndex = ran.Next(allWords.Length);
        correctWord = allWords[tempIndex];
    }

    private void KeyboardInput()
    {
        int b = Raylib.GetKeyPressed();
        char a = (char)b;

        /*
        Keycode regler

        BACKSPACE : 259
        ENTER : 257
        A-Z : lowercaseABC.contains()

        Ingen input : '\0'

        Jag har separata If statements så att jag 
        kan enklare säga vad problemet är exakt.
        */

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
                if (currentWord.Length > 0)
                {
                    wordSquares[row, currentWord.Length - 1].ch = '\0';
                    currentWord = currentWord.Remove(currentWord.Length - 1);
                }
                else
                {
                    Console.WriteLine("Nothing to remove!");
                }
            }
            else
            {
                if (lowercaseABC.Contains(a.ToString().ToLower()))
                {
                    Console.WriteLine(currentWord.Length);
                    if (currentWord.Length >= 5)
                    {
                        Console.WriteLine($"Word is too long, you have {currentWord.Length}/5 characters!");
                    }
                    else
                    {
                        currentWord += a.ToString();
                        wordSquares[row, currentWord.Length - 1].ch = a;
                    }
                }
                else
                {
                    Console.WriteLine($"{a.ToString()} is not a valid character in the game!");
                }
            }
        }
    }

    public void RunRound()
    {
        while (playing)
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.WHITE);

            if (win)
            {
                WinScreen();
                playing = false;
                break;
            }
            else
            {
                Display();
                Buttons();
                KeyboardInput();
            }

            Raylib.EndDrawing();
        }
    }

    private void WinScreen()
    {
        //Sadly måste vi avsluta Drawing (EndDrawing()) och sen starta den igen i slutet av funktionen... :/
        System.Threading.Thread.Sleep(500);
        Raylib.EndDrawing();

        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.ORANGE);
        Raylib.DrawText("You have won!", 150, 200, 100, Color.RED);
        Raylib.DrawText($"correct word:\n{correctWord}", 150, 400, 100, Color.BLUE);
        Raylib.EndDrawing();
        System.Threading.Thread.Sleep(2000);

        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.ORANGE);
    }

    private void Buttons()
    {
        var mouseCords = Raylib.GetMousePosition();

        Raylib.DrawRectangleRec(exitButton, Color.GRAY);
        Raylib.DrawRectangleLinesEx(exitButton, 2, Color.LIGHTGRAY);
        Raylib.DrawText("Exit", 15, 10, 60, Color.BLACK);

        Raylib.DrawRectangleRec(infoButton, Color.DARKGREEN);
        Raylib.DrawRectangleLinesEx(infoButton, 2, Color.LIGHTGRAY);
        Raylib.DrawText("?", 948, 8, 60, Color.BLACK);

        //Det finns exit knappen och info knappen
        //Jag delar upp dem i 'v' och 'h' - vertical och horizontal
        bool exitH = mouseCords.X > exitButton.x && mouseCords.X < exitButton.width + exitButton.x;
        bool exitV = mouseCords.Y > exitButton.y && mouseCords.Y < exitButton.y + exitButton.height;
        if (exitH && exitV)
        {
            Raylib.DrawRectangleRec(exitButton, Color.BLACK);
            Raylib.DrawText("Exit", 15, 10, 60, Color.WHITE);

            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
            {
                playing = false;
            }
        }

        bool infoH = mouseCords.X > infoButton.x && mouseCords.X < infoButton.width + infoButton.x;
        bool infoV = mouseCords.Y > infoButton.y && mouseCords.Y < infoButton.y + infoButton.height;
        if (infoH && infoV)
        {
            string instructions = "Your goal is to\nguess a random word.\n\nThe word is random\nand english.";
            (int x, int y) blockPos = ((int)mouseCords.X - 500, (int)mouseCords.Y);
            Raylib.DrawRectangle(blockPos.x, blockPos.y, 500, 300, Color.GRAY);
            Raylib.DrawText(instructions, blockPos.x + 10, blockPos.y + 10, 40, Color.BLACK);
        }
    }

    private void Display()
    {
        int xOffset = 250, yOffset = 25;

        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                int tX = xOffset + (100 * j), tY = yOffset + (100 * i);

                Raylib.DrawRectangle(tX + 2, tY + 2, 96, 96, wordSquares[i, j].col);
                Raylib.DrawRectangleLines(tX + 2, tY + 2, 96, 96, Color.GRAY);
                Raylib.DrawText(wordSquares[i, j].ch.ToString(), tX, tY, 60, Color.BLACK);
            }
        }
    }
}
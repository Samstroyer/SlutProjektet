using System;
using System.IO;
using Raylib_cs;
using System.Collections.Generic;

class WordleGame
{
    //Lista är enklare och har en .Contains(), det gör så att jag enkelt senare kan se om man skrivit en bokstav eller inte.
    List<string> lowercaseABC = new List<string> { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
    //ExitButton är den knappen man ser högst upp vänster där det står exit.
    Rectangle exitButton = new Rectangle(10, 10, 125, 55), infoButton = new Rectangle(940, 10, 50, 50);

    //Det här är all "game logic" - det som bestämmer om man vunnit, förlorat eller spelar
    private bool playing = true;
    bool win = false, lose = false;

    //Det här är ordlogik
    private string currentWord = "";
    string correctWord;
    string[] allWords;
    List<string> usedWords = new List<string>();
    int row = 0;

    //Det här är alla färger för de spelade orden, gul att det finns, grön är rätt, grå finns inte
    (Color col, char ch)[,] wordSquares = new (Color col, char ch)[6, 5];

    public WordleGame()
    {
        /*
            Man kan välja ASync för att ladda in listan med ord samtidigt som den arbetar med annat.
            Som tur är så är det inte en 10k+ lista så det händer relativt snabbt.

            Inte så att listan är EXTREEEMT lång men den kanske inte borde laddas in på RAM
            
            - från mätningar tar listan mellan 30-60MB, vilket är manageable.
            - från mätningar tar det under sekunden att ladda in den.
            (Mätningar på skoldatorn!)
        */
        allWords = File.ReadAllLines(@"WordleWords.txt");

        //Fyll alla ord med vit färg och ingen input
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
        //Det är lite oklart varför jag valde a och b som namn, 
        //Jag hjälpte dock Simon och i det tillfället använde a och b
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


        //Jag kommer kommentera de ologiska delarna men annars förklarar det sig själv.
        //'\0' är att mna inte skrivit något och row 6 är om man är klar med spelet
        if (!(a == '\0') && row < 6)
        {
            if (b == 257)
            {
                if (currentWord.Length == 5)
                {
                    bool used = false;

                    //Om man har använt vilket ord som helst, kolla om det e samma
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
                        //Om ordet är använt
                        Console.WriteLine("Word is already used");
                    }
                    else if (currentWord == correctWord)
                    {
                        //Om ordet är rätt har man vunnit
                        win = true;
                        Console.WriteLine("You win!");
                    }
                    else
                    {
                        //Här är logiken för att färga om bokstäverna beroende på om det är rätt eller inte
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
                            usedWords.Add(currentWord);
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

                            //Random meddelande men var bra för debugging
                            Console.WriteLine("Everything is ok if you see this!");
                        }
                        else
                        {
                            Console.WriteLine("Can't validate existance of word!");
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
                        //Om man bara skriver en normal bokstav hamnar man här, 
                        //Det lägger till den på ordet och man kan spela spelet
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
        else if (row >= 6)
        {
            //Om man hamnar på rad 6 så.... förlorat
            //Finns bara 5 rader
            lose = true;
        }
    }

    public void RunRound()
    {
        while (playing)
        {
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.WHITE);

            //Om man vunnit eller förlorat så EndScreen();
            if (win || lose)
            {
                EndScreen();
                playing = false;
                break;
            }
            else
            {
                //Visa och gör allt som vanligt annars
                Display();
                Buttons();
                KeyboardInput();
            }

            Raylib.EndDrawing();
        }
    }

    private void EndScreen()
    {
        //Sadly måste vi avsluta Drawing (EndDrawing()) och sen starta den igen i slutet av funktionen... :/
        System.Threading.Thread.Sleep(500);
        Raylib.EndDrawing();


        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.ORANGE);

        if (win)
        {
            //Winscreen bokstavligen
            Raylib.DrawText("You have won!", 150, 200, 100, Color.RED);
            Raylib.DrawText($"correct word:\n{correctWord}", 150, 400, 100, Color.BLUE);
            Raylib.EndDrawing();
        }
        else if (lose)
        {
            //Losescreen bokstavligen
            Raylib.DrawText("You have lost!", 150, 200, 100, Color.RED);
            Raylib.DrawText($"correct word:\n{correctWord}", 150, 400, 100, Color.BLUE);
            Raylib.EndDrawing();
        }

        System.Threading.Thread.Sleep(2000);

        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.ORANGE);
    }

    private void Buttons()
    {
        //Alla knappar + mouse positions för att veta vart man klickar.
        var mouseCords = Raylib.GetMousePosition();

        //Exit knappen
        Raylib.DrawRectangleRec(exitButton, Color.GRAY);
        Raylib.DrawRectangleLinesEx(exitButton, 2, Color.LIGHTGRAY);
        Raylib.DrawText("Exit", 15, 10, 60, Color.BLACK);

        //Info knappen
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
                //Playing = false om man slutar spela
                playing = false;
            }
        }

        bool infoH = mouseCords.X > infoButton.x && mouseCords.X < infoButton.width + infoButton.x;
        bool infoV = mouseCords.Y > infoButton.y && mouseCords.Y < infoButton.y + infoButton.height;
        if (infoH && infoV)
        {
            //Visar info i en "pop-up" ruta
            string instructions = "Your goal is to\nguess a random word.\n\nThe word is random\nand probably english.";
            (int x, int y) blockPos = ((int)mouseCords.X - 500, (int)mouseCords.Y);
            Raylib.DrawRectangle(blockPos.x, blockPos.y, 500, 300, Color.GRAY);
            Raylib.DrawRectangleLines(blockPos.x, blockPos.y, 500, 300, Color.BLACK);
            Raylib.DrawText(instructions, blockPos.x + 10, blockPos.y + 10, 40, Color.BLACK);
        }
    }

    private void Display()
    {
        //Jag vill ha allt i mitten av skärmen (x-axeln)
        //Jag vill inte ha det mot "taket" (y-axeln)
        int xOffset = 250, yOffset = 25;

        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                int tX = xOffset + (100 * j), tY = yOffset + (100 * i);

                Raylib.DrawRectangle(tX + 2, tY + 2, 96, 96, wordSquares[i, j].col);
                Raylib.DrawRectangleLines(tX + 2, tY + 2, 96, 96, Color.GRAY);
                Raylib.DrawText(wordSquares[i, j].ch.ToString(), tX + 30, tY + 25, 60, Color.BLACK);
            }
        }

        //Den här delen visar alla använda tangenter längst ner.
        int x = 360, y = 650;
        foreach (string s in lowercaseABC)
        {
            Rectangle tempBox = new Rectangle(x, y, 25, 25);

            //lista på vilka tangenter som ska ha vilken färg
            List<string> grayCharacters = new List<string>();
            List<string> yellowCharacters = new List<string>();
            List<string> greenCharacters = new List<string>();

            foreach (string uw in usedWords)
            {
                for (int i = 0; i < 5; i++)
                {
                    if (uw.ToCharArray()[i] == correctWord.ToCharArray()[i])
                    {
                        greenCharacters.Add(uw.ToCharArray()[i].ToString().ToLower());
                    }
                    else if (correctWord.Contains(uw.ToCharArray()[i]))
                    {
                        yellowCharacters.Add(uw.ToCharArray()[i].ToString().ToLower());
                    }
                    else if (!correctWord.Contains(uw.ToCharArray()[i]))
                    {
                        grayCharacters.Add(uw.ToCharArray()[i].ToString().ToLower());
                    }
                }
            }

            //Färgar till slut bakgrunden beroende på om det finns eller inte i listan
            //Den går i Hierarki, basically så - om den finns i grön har den prioritet över gul och grå
            if (greenCharacters.Contains(s.ToLower()))
            {
                Raylib.DrawRectangleRec(tempBox, Color.GREEN);
            }
            else if (yellowCharacters.Contains(s.ToLower()))
            {
                Raylib.DrawRectangleRec(tempBox, Color.YELLOW);
            }
            else if (grayCharacters.Contains(s.ToLower()))
            {
                Raylib.DrawRectangleRec(tempBox, Color.LIGHTGRAY);
            }

            //Rita outline på lådan och bokstaven
            Raylib.DrawRectangleLinesEx(tempBox, 2, Color.BLACK);
            Raylib.DrawText(s.ToUpper(), (int)tempBox.x + 5, (int)tempBox.y + 2, 25, Color.BLACK);

            //Positionsmatte för nästa ruta
            x += 30;
            if (x > 600)
            {
                y += 50;
                x = 360;
            }
        }
    }
}
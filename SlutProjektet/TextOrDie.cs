using System;
using System.IO;
using Raylib_cs;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;

class TextOrDie
{
    states gameState = states.playing;
    bool playing = true;
    Random gen = new Random();

    //Kamera saker till 3D
    Vector3 camPos = new Vector3(0, 10, 10), camTarget = new Vector3(0, 0, 0);
    Camera3D cam;

    //Array för hastighet då alfabetet är statiskt
    char[] alphabet = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
    string currentTyping = "";

    //Lista är snabbare och vi kommer max ha 4 enemies (5 personer totalt)
    (string name, int elevation, List<char> letterTower)[] participants = new (string, int, List<char>)[5];

    //Frågor och prompts till frågorna i en lista (vill kunna ta bort frågan om den används) 
    List<(string prompt, string[] answers)> questions = new List<(string prompt, string[] answers)>();
    int currQuestionNum;

    //Tipsad av Sebastian (WebWeu) att göra en enum för states
    public enum states
    {
        playing,
        draw,
        lose,
        win
    }

    public TextOrDie()
    {
        //Kamera Setup
        cam = new Camera3D(camPos, camTarget, new Vector3(0, 1, 0), 70, CameraProjection.CAMERA_PERSPECTIVE);

        //Ge våra enemies random namn
        //Alla har inte unika namn, men det är en liten chans man har samma namn.
        //Samma princip här
        string[] allNames = File.ReadAllLines(@"TextOrDie/Other/Names.txt");
        int max = allNames.Length;

        for (var i = 0; i < participants.Count(); i++)
        {
            string randomName = allNames[gen.Next(max)];

            participants[i].name = randomName;
            participants[i].elevation = 0;
            participants[i].letterTower = new List<char>();
        }

        participants[2].name = "You";

        //Ladda alla frågor
        string[] questionPaths = Directory.GetFiles(@"TextOrDie/Questions"); //Med extensionen
        for (var i = 0; i < questionPaths.Length; i++)
        {
            //Ladda allt
            List<string> wholeFile = new List<string>();
            wholeFile.AddRange(File.ReadAllLines(questionPaths[i]));

            //Frågan är första raden, ta sen bort frågan och spara alla svar i en array
            string q = wholeFile[0];
            wholeFile.RemoveAt(0);
            string[] answers = wholeFile.ToArray();

            //Få in det i questions listan
            questions.Add((q, answers));
        }

        currQuestionNum = gen.Next(questions.Count());
    }

    public void RunRound()
    {
        //(string q, string[] a) q = questions[gen.Next(questions.Count())];

        while (playing && !Raylib.WindowShouldClose())
        {

            switch (gameState)
            {
                case states.playing:
                    {
                        Raylib.BeginDrawing();
                        Raylib.ClearBackground(Color.WHITE);

                        Raylib.BeginMode3D(cam);
                        Render3D();
                        Raylib.EndMode3D();

                        InputDetection();
                        Logic();

                        Render2D();
                        Raylib.EndDrawing();
                    }
                    break;
            }
        }
    }

    private void Render2D()
    {
        int fontSize = 20;
        int halfWidth = Raylib.GetScreenWidth() / 2;
        int yStart = 600;

        int writeBoxOffset = Raylib.MeasureText(currentTyping, fontSize) / 2;
        Raylib.DrawRectangle(halfWidth - writeBoxOffset - 5, yStart - 5, writeBoxOffset * 2 + 10, 28, Color.DARKPURPLE);
        Raylib.DrawRectangleLines(halfWidth - writeBoxOffset - 5, yStart - 5, writeBoxOffset * 2 + 10, 28, Color.GRAY);
        Raylib.DrawText(currentTyping, halfWidth - writeBoxOffset, yStart, fontSize, Color.PURPLE);

        int questionBoxOffset = Raylib.MeasureText(questions[currQuestionNum].prompt, 20) / 2;
        Raylib.DrawRectangle(halfWidth - questionBoxOffset - 5, 100, questionBoxOffset * 2 + 10, 28, Color.PURPLE);
        Raylib.DrawRectangleLines(halfWidth - questionBoxOffset - 5, 100, questionBoxOffset * 2 + 10, 28, Color.BLACK);
        Raylib.DrawText(questions[currQuestionNum].prompt, halfWidth - questionBoxOffset, 100, 20, Color.WHITE);
    }

    private void Render3D()
    {
        participants[0].letterTower = new List<char> { 'H', 'Y', 'a' };

        //storlek 9, offset 10 (-20 = -10 * 2)
        int startX = -20, size = 9;
        for (int participantID = 0; participantID < participants.Length; participantID++)
        {
            for (int towerHeight = 0; towerHeight <= participants[participantID].letterTower.Count; towerHeight++)
            {
                Raylib.DrawCube(new Vector3(startX, towerHeight * 2.5f, 0), size, 2, size, Color.BLUE);
                Raylib.DrawCubeWires(new Vector3(startX, towerHeight * 2.5f, 0), size, 2, size, Color.BLACK);
            }
            startX += 10;
        }

        //Reference
        Raylib.DrawCircle3D(new Vector3(0, 20, 0), 10, new Vector3(0, 0, 0), 0, Color.RED);
        Raylib.DrawCircle3D(new Vector3(0, 0, 0), 10, new Vector3(0, 0, 0), 0, Color.GREEN);
        Raylib.DrawCircle3D(new Vector3(0, -20, 0), 10, new Vector3(0, 0, 0), 0, Color.BLUE);
    }

    private void Logic()
    {
        camTarget = new Vector3(0, participants[2].elevation * 2.5f, 0);
        camPos = new Vector3(0, (participants[2].elevation * 2.5f) + 6, 35);
        cam.position = camPos;
        cam.target = camTarget;
    }

    private void InputDetection()
    {
        int inputNum = Raylib.GetKeyPressed();
        char inputChar = (char)inputNum;

        // BACKSPACE : 259
        // ENTER : 257

        if (alphabet.Contains(inputChar))
        {
            currentTyping += inputChar;
            Console.WriteLine("yepp");
        }
        else if (inputNum == 259 && currentTyping.Length > 0)
        {
            currentTyping = currentTyping.Remove(currentTyping.Length - 1);
        }
        else if (inputNum == 257 && currentTyping.Length > 0)
        {
            if (questions[currQuestionNum].answers.Contains(currentTyping))
            {
                participants[2].letterTower.AddRange(currentTyping);
                participants[2].elevation = participants[2].letterTower.Count + 1;

            }

            for (int i = 0; i < participants.Count(); i++)
            {
                if (i != 2)
                {
                    int answerID = gen.Next(questions[currQuestionNum].answers.Count());
                    string randomPickedAnswer = questions[currQuestionNum].answers[answerID];
                    participants[i].letterTower.AddRange(randomPickedAnswer);
                    participants[i].elevation += randomPickedAnswer.Length;
                }
            }

            questions.RemoveAt(currQuestionNum);
            if (questions.Count > 0)
            {
                currQuestionNum = gen.Next(questions.Count());
            }
            else
            {
                gameState = states.draw;
            }
        }
    }
}
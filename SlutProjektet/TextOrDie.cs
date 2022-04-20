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

    //Lista är snabbare och vi kommer max ha 4 enemies (5 personer totalt)
    (string name, int elevation, List<char> letterTower)[] participants = new (string, int, List<char>)[5];

    //Frågor och prompts till frågorna i en lista (vill kunna ta bort frågan om den används) 
    List<(string prompt, string[] answers)> questions = new List<(string prompt, string[] answers)>();

    //Tipsad av Sebastian (WebWeu) att göra en enum för states
    public enum states
    {
        playing,
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
    }

    public void RunRound()
    {
        //(string q, string[] a) q = questions[gen.Next(questions.Count())];

        while (playing && !Raylib.WindowShouldClose())
        {
            Raylib.BeginDrawing();
            Raylib.BeginMode3D(cam);
            Raylib.ClearBackground(Color.WHITE);

            switch (gameState)
            {
                case states.playing:
                    {
                        InputDetection();
                        Logic();
                        Render();
                    }
                    break;
            }



            Raylib.EndMode3D();
            Raylib.EndDrawing();

        }
    }

    private void Render()
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
        camPos = new Vector3(0, (participants[2].elevation * 2.5f) + 8, 40);
        cam.position = camPos;
        cam.target = camTarget;
    }

    private void InputDetection()
    {
        //Se till så att inte man lägger på faktiskt på [2] (you) utan att man lägger i en temp som sedan läggs på [2] "you"
        //Kan annars göra så att man vinner direkt...
        int inputNum = Raylib.GetKeyPressed();
        char inputChar = (char)inputNum;

        System.Console.WriteLine(participants[2].elevation);
        if (inputChar == 'A')
        {
            participants[2].elevation++;
            participants[2].letterTower.Add((char)gen.Next(35, 120));
        }

        System.Console.WriteLine(participants[2].elevation);
    }
}
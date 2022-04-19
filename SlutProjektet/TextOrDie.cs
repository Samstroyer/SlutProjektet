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
    int elevation;
    Random gen = new Random();

    //Lista är snabbare och vi kommer max ha 4 enemies (5 personer totalt)
    string[] enemyNames = new string[4];

    //Frågor och prompts till frågorna i en lista (vill kunna ta bort frågan om den används) 
    List<(string prompt, string[] answers)> questions = new List<(string prompt, string[] answers)>();

    //Kamera
    float tempX = 100;
    Camera3D cam = new Camera3D(new Vector3(0, 30, 0), new Vector3(0, 0, 0), new Vector3(0, 1, 0), 70, CameraProjection.CAMERA_PERSPECTIVE);

    //Tipsad av Sebastian (WebWeu) att göra en enum för states
    public enum states
    {
        playing,
        lose,
        win
    }

    public TextOrDie()
    {
        //Ge våra enemies random namn
        //Alla har inte unika namn, men det är en liten chans man har samma namn.
        //Samma princip här
        for (int i = 0; i < 4; i++)
        {
            string[] allNames = File.ReadAllLines(@"TextOrDie/Other/Names.txt");
            int max = allNames.Length;
            string randomName = allNames[gen.Next(max)];

            enemyNames[i] = randomName;
        }

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
        Raylib.DrawCircle3D(new Vector3(0, 20, 0), 10, new Vector3(0, 0, 0), 0, Color.RED);
        Raylib.DrawCircle3D(new Vector3(0, 0, 0), 10, new Vector3(0, 0, 0), 0, Color.GREEN);
        Raylib.DrawCircle3D(new Vector3(0, -20, 0), 10, new Vector3(0, 0, 0), 0, Color.BLUE);
    }

    private void Logic()
    {
        cam.position = new Vector3(100, -100, tempX);
        tempX += 1f;
    }
}
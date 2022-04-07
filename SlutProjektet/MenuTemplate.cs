using System.Collections.Generic;
using Raylib_cs;


//Jag har en MenuTemplate så att jag inte behöver göra custom menyer hela tiden. 
//Fungerar perfekt för sitt syfte och kan gömma allt i en fil.
class Menu
{
    //Menyns namn (används inte direkt)
    string name;

    //Jag behöver inte hastighetem en array ger, och att arbeta med en 
    //dynamisk lista gör det mycket enklare än en "statisk" array 
    public List<(Rectangle shape, string prompt)> menuButtons = new List<(Rectangle shape, string prompt)>();

    public Menu(string requestedName)
    {
        //Man måste ge menyn ett namn när man skapar den.
        name = requestedName;
    }

    public string GetName()
    {
        //Används som sagt inte men bra att ha till framtiden och speciellt debugging.
        return name;
    }

    public string Run()
    {
        //Det här är "menu" loopen, default är att stanna i menyn
        string option = "stay";

        foreach ((Rectangle, string) b in menuButtons)
        {
            //Om man håller över eller klickar så byter den meny (visar också hover)
            Display(b);
            option = Hover(b);
            if (option != "stay")
            {
                return option;
            }
        }

        return option;
    }

    private void Display((Rectangle shape, string prompt) button)
    {
        //Visar en inskickad knapp och texten som tillhör den i en tuple
        Raylib.DrawRectangleRec(button.Item1, Color.RED);
        Raylib.DrawRectangleLinesEx(button.Item1, 10, Color.GRAY);
        Raylib.DrawText(button.Item2, (int)button.Item1.x + 20, (int)(button.Item1.y + button.Item1.height / 3), 60, Color.BLACK);
    }

    private string Hover((Rectangle shape, string prompt) button)
    {
        //Det här är basically bara effekter men också att hantera om man klickar på knappen
        string option = "stay";
        var mouseCords = Raylib.GetMousePosition();

        if (mouseCords.X > button.shape.x && mouseCords.X < button.shape.x + button.shape.width)
        {
            if (mouseCords.Y > button.shape.y && mouseCords.Y < button.shape.y + button.shape.height)
            {
                Raylib.DrawRectangleRec(button.shape, Color.DARKGRAY);
                Raylib.DrawRectangleLinesEx(button.shape, 10, Color.BLACK);
                Raylib.DrawText(button.prompt, (int)button.shape.x + 20, (int)(button.shape.y + button.shape.height / 3), 60, Color.BLACK);

                if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_BUTTON_LEFT))
                {
                    return button.prompt;
                }
            }
        }

        return option;
    }
}
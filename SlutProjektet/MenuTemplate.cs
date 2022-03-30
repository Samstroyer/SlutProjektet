using System.Collections.Generic;
using Raylib_cs;

class Menu
{
    string name;

    //Jag behöver inte hastighet i en "menu", och att arbeta med en 
    //dynamisk lista gör det mycket enklare än en "statisk" array 
    public List<(Rectangle shape, string prompt)> menuButtons = new List<(Rectangle shape, string prompt)>();

    public Menu(string requestedName)
    {
        name = requestedName;
    }

    public string GetName()
    {
        return name;
    }

    public string Run()
    {
        string option = "stay";

        foreach ((Rectangle, string) b in menuButtons)
        {
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
        Raylib.DrawRectangleRec(button.Item1, Color.RED);
        Raylib.DrawRectangleLinesEx(button.Item1, 10, Color.GRAY);
        Raylib.DrawText(button.Item2, (int)button.Item1.x + 20, (int)(button.Item1.y + button.Item1.height / 3), 60, Color.BLACK);
    }

    private string Hover((Rectangle shape, string prompt) button)
    {
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
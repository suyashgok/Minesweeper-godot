using Godot;
using System;

public partial class Node2D : Godot.Node2D
{
    int playerX = 2;
    int playerY = 2;
    int gridSize = 5;
    int [,] gridData;
    bool [,] discovered;

    public override void _Ready()
    {
        gridData = new int [gridSize, gridSize];
        discovered = new bool [gridSize, gridSize];

        gridData [1,1] = 9;
        gridData [3,2] = 9;
        GD.Print("Beware of mines!");
        DrawGrid();
    }

    public override void _Process(double delta)
    {
        if(Input.IsActionJustPressed("ui_right"))
        {
            if(playerX < gridSize - 1)
                playerX += 1;
            DrawGrid();
        }
        if(Input.IsActionJustPressed("ui_left"))
        {
            if(playerX > 0)
                playerX -= 1;
            DrawGrid();
        }
        if(Input.IsActionJustPressed("ui_up"))
        {
            if(playerY > 0)
                playerY -= 1;
            DrawGrid();
        }
        if(Input.IsActionJustPressed("ui_down"))
        {
            if(playerY < gridSize - 1)
                playerY += 1;
            DrawGrid();
        }
        if(Input.IsActionJustPressed("ui_accept"))
        {
            _revealSquare();
        }
    }

    public void _revealSquare()
    {
        
        
           discovered [playerY, playerX] = true;
           if(gridData [playerY, playerX] == 9)
           {
                DrawGrid();
                GD.Print("BOOM! You set off the mine! \n Game Over!");
                SetProcess(false);
           }
           else
           {
                DrawGrid();
           }
        
    }

    void DrawGrid()
    {
        GD.Print("\n" + "--- New Frame ---");

        for(int y = 0; y < gridSize; y++)
        {
            string line = "";
            for(int x = 0; x< gridSize; x++)
            {
                if(discovered [y,x] == true && gridData [y,x] == 9)
                {
                    line += "[ * ] ";
                }
                else if( x == playerX && y == playerY)
                {
                    line += "[ ? ] ";
                }
                else if(discovered[y,x] == false)
                {
                    line += "[ # ] ";
                }
                else 
                {
                    line += "[   ] ";
                }
            }
            GD.Print(line);
        }
    }
}
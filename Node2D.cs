using Godot;
using System;

public partial class Node2D : Godot.Node2D
{
    int playerX = 2;
    int playerY = 2;
    int gridSize = 5;
    int bombX = 3;
    int bombY = 4;

    public override void _Ready()
    {
        GD.Print("Manual Control Initialized!");
        DrawGrid();
    }

    public override void _Process(double delta)
    {
        if(Input.IsActionJustPressed("ui_right"))
        {
            if(playerX < 4)
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
            if(playerY < 4)
                playerY += 1;
            DrawGrid();
        }
        _check();
    }

    public void _check()
    {
        if(Input.IsActionJustPressed("ui_accept"))
        {
            if(playerX == bombX && playerY == bombY)
            {
                GD.Print("BOOM! You hit the bomb!");
                SetProcess(false);
            }
            else
            {
                GD.Print("You just dugged at [" + playerX + "," + playerY + "]");
            }
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
                if( x == playerX && y == playerY)
                {
                   line += "[?]";
                }
                else 
                {
                    line += "[ ] ";
                }
            }
            GD.Print(line);
        }
    }
}
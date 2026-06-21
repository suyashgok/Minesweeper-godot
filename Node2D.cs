using Godot; //connects the game engine to this file
using System;  //connects the basic .NET framework to this file

public partial class Node2D : Godot.Node2D  //initialized a partial class connected to a file hidden in game engine that inherits the functions of godot's 2d system
{
    int playerX = 2;
    int playerY = 2;
    int gridSize = 5;
    int [,] gridData;
    bool [,] discovered;
    bool [,] flagged;

    public override void _Ready()
    {
        gridData = new int [gridSize, gridSize];
        discovered = new bool [gridSize, gridSize];
        flagged = new bool [gridSize, gridSize];

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
        else if(Input.IsActionJustPressed("flag_square"))
        {
            _flagSquare();
        }
    }

    // public void _neighborMineCounter()
    // {
    //     int a = playerY;
    //     int b = playerX;
    //     int counter = 0;
    //     if(a > 0 && b > 0)
    //     {
    //         if(gridData[a-1,b-1] == 9)
    //         {
    //             counter += 1;
    //         }
    //     }
    //     if(a > 0)
    //     {
    //         if(gridData[a-1,b] == 9)
    //         {
    //             counter +=1;
    //         }
    //     }
    //      if(b > 0)
    //     {   
    //         if(gridData[a,b-1] == 9)
    //         {
    //             counter += 1;
    //         }
    //     }
    //     if(a > 0 && b < gridSize -1)
    //     {
    //         if(gridData[a-1,b+1] == 9)
    //         {
    //             counter += 1;
    //         }
    //     }
    //     if(b < gridSize - 1)
    //     {
    //         if(gridData[a,b+1] == 9)
    //         {
    //             counter +=1;
    //         }
    //     }
    //     if(a < gridSize -1 && b < gridSize -1)
    //     {
    //         if(gridData[a+1,b+1] == 9)
    //         {
    //             counter += 1;
    //         }
    //     }
    //     if(a < gridSize - 1)
    //     {
    //         if(gridData[a+1,b] == 9)
    //         {
    //             counter += 1;
    //         }
    //     }
    //     if(a < gridSize -1 && b > 0)
    //     {
    //         if(gridData[a+1,b-1] == 9)
    //         {
    //             counter +=1;
    //         }
    //     }
    // }
    
    public void _flagSquare()
    {
        flagged [playerY, playerX] = true;
        DrawGrid();
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
            // _neighborMineCounter();
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
                if(flagged [y,x] == true && discovered[y,x] == false)
                {
                    line += "[ F ] ";
                }
                else if(discovered [y,x] == true && gridData [y,x] == 9)
                {
                    line += "[ * ] ";
                }
                else if( x == playerX && y == playerY)
                {
                    line += "[ = ] ";
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

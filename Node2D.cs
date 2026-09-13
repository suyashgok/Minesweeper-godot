using Godot; //connects the game engine to this file
using System;  //connects the basic .NET framework to this file

public partial class Node2D : Godot.Node2D  //initialized a partial class connected to a file hidden in game engine that inherits the functions of godot's 2d system
{
    int playerX = 2;
    int playerY = 2;
    int gridSize = 5;
    int totalMines = 3;
    bool gameStarted = false;
    bool gameOver = false;
    bool gameWon = false;
    int [,] gridData;
    int [,] minecount;
    bool [,] discovered;
    bool [,] flagged;
    
    public override void _Ready()
    {
        gridData = new int [gridSize, gridSize];
        discovered = new bool [gridSize, gridSize];
        flagged = new bool [gridSize, gridSize];
        minecount = new int [gridSize, gridSize];

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
            _revealSquare(playerY, playerX);
            DrawGrid();
        }
        else if(Input.IsActionJustPressed("flag_square"))
        {
            _flagSquare(playerY, playerX);
        }
    }

    public int _neighborMineCounter(int a, int b)
    {
        int counter = 0;
        if(a > 0 && b > 0)
        {
            if(gridData[a-1,b-1] == 9)
            {
                counter += 1;
            }
        }
        if(a > 0)
        {
            if(gridData[a-1,b] == 9)
            {
                counter +=1;
            }
        }
         if(b > 0)
        {   
            if(gridData[a,b-1] == 9)
            {
                counter += 1;
            }
        }
        if(a > 0 && b < gridSize -1)
        {
            if(gridData[a-1,b+1] == 9)
            {
                counter += 1;
            }
        }
        if(b < gridSize - 1)
        {
            if(gridData[a,b+1] == 9)
            {
                counter +=1;
            }
        }
        if(a < gridSize -1 && b < gridSize -1)
        {
            if(gridData[a+1,b+1] == 9)
            {
                counter += 1;
            }
        }
        if(a < gridSize - 1)
        {
            if(gridData[a+1,b] == 9)
            {
                counter += 1;
            }
        }
        if(a < gridSize -1 && b > 0)
        {
            if(gridData[a+1,b-1] == 9)
            {
                counter +=1;
            }
        }
        return counter;
    }
    
    public void _flagSquare(int y, int x)
    {
        flagged [y, x] = !flagged[y,x];
        DrawGrid();
    }

    public void _placeMines(int firstY, int firstX)
    {
        Random rng = new Random();
        int minesPlaced = 0;

        while(minesPlaced < totalMines)
        {
            int randY = rng.Next(0, gridSize);
            int randX = rng.Next(0, gridSize);

            if(gridData[randY, randX] != 9 && !(randY == firstY && randX == firstX))
            {
                gridData[randY, randX] = 9;
                minesPlaced++;
            }
        }
    }

    public void _revealSquare(int y, int x)
    {
        if(!gameStarted)
        {
            _placeMines(y,x);
            gameStarted = true;
        }

        discovered [y, x] = true;
        
        if(gridData [y, x] == 9)
        {
            gameOver = true;
            SetProcess(false);
        }
        else
        {
            minecount[y, x] = _neighborMineCounter(y, x);
            
            if(minecount[y,x] == 0)
            {
                if(y > 0 && x > 0 && !discovered[y-1, x-1] && gridData[y-1, x-1] != 9)
                    _revealSquare(y-1, x-1);

                if(y > 0 && !discovered[y-1, x] && gridData[y - 1, x] != 9)
                    _revealSquare(y-1, x);

                if(y > 0 && x < gridSize-1 && !discovered[y - 1, x + 1] && gridData[y - 1, x + 1] != 9)
                    _revealSquare(y-1, x+1);

                if(x > 0 && !discovered[y, x-1] && gridData[y, x-1] != 9)
                    _revealSquare(y, x-1);

                if(x < gridSize-1 && !discovered[y, x+1] && gridData[y, x+1] != 9)
                    _revealSquare(y, x+1);

                if(y < gridSize-1 && x > 0 && !discovered[y+1, x-1] && gridData[y+1, x-1] != 9)
                    _revealSquare(y+1, x-1);

                if(y < gridSize-1 && !discovered[y+1, x] && gridData[y+1, x] != 9)
                    _revealSquare(y+1, x);

                if(y < gridSize-1 && x < gridSize-1 && !discovered[y+1, x+1] && gridData[y+1, x+1] != 9)
                    _revealSquare(y+1, x+1);     
            }
            
            int tilesRevealed = 0;
            
            for(int cy = 0; cy < gridSize; cy++)
            {
                for(int cx = 0; cx < gridSize; cx++)
                {
                    if(discovered[cy,cx] == true)
                    {
                        tilesRevealed++;
                    }
                }
            }

            if(tilesRevealed == gridSize * gridSize - totalMines)
            {
                gameWon = true;
                SetProcess(false);
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
                if( x == playerX && y == playerY && gameOver == false && gameWon == false)
                {
                    line += "[ = ] ";
                }
                else if(flagged [y,x] == true && discovered[y,x] == false && gameOver == false && gameWon == false)
                {
                    line += "[ F ] ";
                }
                else if((discovered [y,x] == true && gridData [y,x] == 9) || (gameOver == true && gridData[y,x] == 9) || (gameWon == true && gridData[y,x] == 9))
                {
                    line += "[ * ] ";
                }
                else if(discovered[y,x] == false)
                {
                    line += "[ # ] ";
                }
                else 
                {
                    if(minecount[y,x] == 0)
                        line += "[   ] ";
                    else    
                        line += "[ "+minecount[y,x]+" ] ";
                }
            }
            GD.Print(line);
            
        }
        if(gameOver)
            {
              GD.Print("BOOM! You set off the mine! \n Game Over!");
            }
        if(gameWon)
            {
                GD.Print("You Won!! All Mines Are Cleared!");       
            }
    }
}

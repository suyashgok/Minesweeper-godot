using Godot; //connects the game engine to this file
using System;  //connects the basic .NET framework to this file

public partial class Node2D : Godot.Node2D  //initialized a partial class connected to a file hidden in game engine that inherits the functions of godot's 2d system
{
    int playerX = 2;
    int playerY = 2;
    int gridSize = 7;
    int totalMines = 6;
    
    bool gameStarted = false;
    bool gameOver = false;
    bool gameWon = false;
    
    int [,] gridData;
    int [,] minecount;
    
    bool [,] discovered;
    bool [,] flagged;

    Label resultLabel;
    Button restartButton;
    Button menuButton;
    Control endButtons;

    TextureButton[,] buttons;  

        Texture2D undiscoveredTexture;
        Texture2D flagTexture;
        Texture2D mineTexture;
        Texture2D [] numberTextures;  
    
    public override void _Ready()
    {
        gridData = new int [gridSize, gridSize];
        discovered = new bool [gridSize, gridSize];
        flagged = new bool [gridSize, gridSize];
        minecount = new int [gridSize, gridSize];

        undiscoveredTexture = GD.Load<Texture2D>("res://Assets/Tiles/Undiscovered_tile.png");
        flagTexture = GD.Load<Texture2D>("res://Assets/Tiles/Flag.png");
        mineTexture = GD.Load<Texture2D>("res://Assets/Tiles/Mine.png");

        resultLabel = GetNode<Label>("%ResultLabel");
        resultLabel.Text = "";
        
        endButtons = GetNode<Control>("%EndButtons");
        restartButton = GetNode<Button>("%Restart");
        menuButton = GetNode<Button>("%ReturnToMenu");

        restartButton.Pressed += _onRestartPressed;
        menuButton.Pressed += _onMenuPressed;
        numberTextures = new Texture2D[9];
        numberTextures[0] = GD.Load<Texture2D>("res://Assets/Tiles/Discovered_tile_with_no_neighbor_mine.png");

        for(int i = 1; i <= 8; i++)
        {
            numberTextures[i] = GD.Load<Texture2D>($"res://Assets/Tiles/Discovered_tile_with_{i}_neighbor_mine.png");
        }

        GD.Print("Beware of mines!");
        DrawGrid();

        GridContainer grid = GetNode<GridContainer>("%GridContainer");
        buttons = new TextureButton[gridSize, gridSize];

        for(int y = 0; y < gridSize; y++)
        {
            for(int x = 0; x < gridSize; x++)
            {
                TextureButton btn = new TextureButton();
                btn.CustomMinimumSize = new Vector2(100, 100);
                btn.StretchMode = TextureButton.StretchModeEnum.Scale;
                
                btn.SetMeta("grid_y", y);
                btn.SetMeta("grid_x", x);

                btn.Pressed += () => _onTileLeftClicked(btn);
                btn.GuiInput += (InputEvent @event) =>
                {
                    if(@event is InputEventMouseButton mouseEvent && mouseEvent.ButtonIndex == MouseButton.Right && mouseEvent.Pressed)
                    {
                        _onTileRightClicked(btn);
                    }
                };
                grid.AddChild(btn);
                buttons[y, x] = btn;
            }
        }
        RefreshGrid();
    }

    private void _onRestartPressed()
    {
        GetTree().ReloadCurrentScene();
    }

    private void _onMenuPressed()
    {
        GetTree().ChangeSceneToFile("res://Scenes/main_menu.tscn");
    }

    public override void _Process(double delta)
    {
       /* if(Input.IsActionJustPressed("ui_right"))
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
        }*/
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
        RefreshGrid();
    }

    private void _onTileRightClicked(TextureButton btn)
    {
        if(gameOver || gameWon)
            return;

        int y = (int)btn.GetMeta("grid_y");
        int x = (int)btn.GetMeta("grid_x");

        _flagSquare(y, x);
    }
   
    public void _placeMines(int firstY, int firstX)
    {
        Random rng = new Random();
        int minesPlaced = 0;

        while(minesPlaced < totalMines)
        {
            int randY = rng.Next(0, gridSize);
            int randX = rng.Next(0, gridSize);
            
            bool nearFirstClick = Math.Abs(randY - firstY) <= 1 && Math.Abs(randX - firstX) <= 1;
            
            if(gridData[randY, randX] != 9 && !nearFirstClick)
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

     private void _onTileLeftClicked(TextureButton btn)
    {
        if(gameOver || gameWon)
            return; 

        int y = (int)btn.GetMeta("grid_y");
        int x = (int)btn.GetMeta("grid_x");

        _revealSquare(y, x);
        DrawGrid();
        RefreshGrid();

        if(gameOver)
        {
            resultLabel.Text = "You set off the mine! \n Game Over!";
            endButtons.Visible = true;
        }
        else if(gameWon)
        {
            resultLabel.Text = "You Won!! All Mines Are Cleared!";
            endButtons.Visible = true;
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
    
    
    void RefreshGrid()
    {
        for(int y = 0; y < gridSize; y++)
        {
            for(int x = 0; x < gridSize; x++)
            {
                TextureButton btn = buttons[y,x];

                if(gameOver && gridData[y,x] ==9)
                    btn.TextureNormal = mineTexture;

                else if(flagged[y,x] == true && discovered[y,x] == false)
                    btn.TextureNormal = flagTexture;

                else if(discovered[y,x] ==false)
                    btn.TextureNormal = undiscoveredTexture;

                else
                    btn.TextureNormal = numberTextures[minecount[y,x]];
            }
        }
    }
}


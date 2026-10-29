using Godot;

public partial class MainMenu : Control
{
    public override void _Ready()
    {
        Button playButton = GetNode<Button>("%PlayButton");
        playButton.Pressed += _onPlayPressed;
    }

    private void _onPlayPressed()
    {
        GetTree().ChangeSceneToFile("res://Node2D.tscn");
    }
}
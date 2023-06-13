using Godot;

public partial class GameMenu : Control
{
	public void _on_play_btn_pressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/game.tscn");
	}

	public void _on_quit_pressed()
	{
		GetTree().Quit();
	}
}

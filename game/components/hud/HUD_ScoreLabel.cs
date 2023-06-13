using Godot;

public partial class HUD_ScoreLabel : Label
{
    public override void _Ready()
    {
        GameManager.Instance.SetScoreLabel(this);
    }
}

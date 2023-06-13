using Godot;

public partial class Coin : Area2D
{
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node body)
    {
        if (body.IsInGroup("player"))
        {
            GameManager.Instance.CoinCollected();
            QueueFree();
        }
    }
}

using Godot;
using System;

public partial class ExitPortal : Area2D
{
    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node body)
    {
        if (body.IsInGroup("player"))
        {
            GameManager.Instance.EnterExitPortal();
        }
    }
}

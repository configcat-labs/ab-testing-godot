using Godot;

public partial class Player : CharacterBody2D
{
    private Sprite2D _playerSprite;
    public const float Speed = 300.0f;
    public const float JumpVelocity = -400.0f;

    // Get the gravity from the project settings to be synced with RigidBody nodes.
    public float Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    public override void _Ready()
    {
        _playerSprite = GetNode<Sprite2D>("Sprite2D");
        AddToGroup("player");
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        // Add the gravity.
        if (!IsOnFloor())
        {
            velocity.Y += (float)(Gravity * delta);
        }

        // Handle Jump.
        if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
        {
            velocity.Y = JumpVelocity;
        }

        // Get the input direction and handle the movement/deceleration.
        // As good practice, you should replace UI actions with custom gameplay actions.
        Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        if (direction != Vector2.Zero)
        {
            velocity.X = direction.X * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
        }

        // Flip the player sprite based on movement direction
        if (velocity.X < 0)
        {
            _playerSprite.FlipH = false;
        }
        else if (velocity.X > 0)
        {
            _playerSprite.FlipH = true;
        }

        // Update player velocity and move
        Velocity = velocity;
        MoveAndSlide();
    }
}

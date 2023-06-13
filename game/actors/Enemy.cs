using Godot;

public partial class Enemy : CharacterBody2D
{
    [Export] public float Speed = 180f;
    [Export] public float DetectionRadius = 180f;
    public float Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    private Node2D _player;
    private CircleShape2D _detectionShape;
    private Area2D _captureArea;
    private Timer _captureTimer;
    private Label _captureLabel;
    private float _captureTime = 3.0f;
    private bool _isCapturing = false;

    public override void _Ready()
    {
        _player = GetNode<Node2D>("/root/Game/Player");
        _detectionShape = new CircleShape2D();
        _detectionShape.Radius = DetectionRadius;

        _captureArea = GetNode<Area2D>("DetectionArea");
        _captureArea.Connect("body_entered", new Callable(this, nameof(OnCaptureAreaBodyEntered)));
        _captureArea.Connect("body_exited", new Callable(this, nameof(OnCaptureAreaBodyExited)));

        _captureTimer = GetNode<Timer>("Timeout");
        _captureTimer.WaitTime = _captureTime;
        _captureTimer.Connect("timeout", new Callable(this, nameof(OnCaptureTimerTimeout)));

        _captureLabel = GetNode<Label>("CaptureLabel");
        _captureLabel.Visible = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 directionToPlayer = (_player.GlobalPosition - GlobalPosition).Normalized();
        float distanceToPlayer = GlobalPosition.DistanceTo(_player.GlobalPosition);

        Vector2 newVelocity = Vector2.Zero;

        if (IsPlayerInDetectionRange(distanceToPlayer))
        {
            newVelocity.X = directionToPlayer.X * Speed;
        }
        else
        {
            newVelocity.X = 0;
        }

        // Apply gravity
        newVelocity.Y = Velocity.Y + Gravity * (float)delta;

        Velocity = newVelocity;
        MoveAndSlide();
    }

    private bool IsPlayerInDetectionRange(float distance)
    {
        return distance <= _detectionShape.Radius;
    }

    private void OnCaptureAreaBodyEntered(Node body)
    {
        if (body.IsInGroup("player"))
        {
            _isCapturing = true;
            _captureTimer.Start();
        }
    }

    private void OnCaptureAreaBodyExited(Node body)
    {
        if (body.IsInGroup("player"))
        {
            _isCapturing = false;
            _captureTimer.Stop();
            _captureLabel.Visible = false;
        }
    }

    private void OnCaptureTimerTimeout()
    {
        _isCapturing = false;
        _captureLabel.Visible = false;
        GameManager.Instance.EndGame();
    }

    public override void _Process(double delta)
    {
        if (_isCapturing)
        {
            float timeLeft = (float)_captureTimer.TimeLeft;
            _captureLabel.Text = $"Time till captured: {timeLeft:F1}s";
            _captureLabel.Visible = true;
        }
    }
}

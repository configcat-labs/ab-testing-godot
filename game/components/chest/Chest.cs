using Godot;

//////////////////////////////////////////////////
//  class serves as a common base for both chest collection mechanisms
//  (tap-based and timer-based), ensuring a consistent interface.
public abstract partial class ChestMechanism : Node
{
    protected Chest chest;

    public ChestMechanism(Chest chest)
    {
        this.chest = chest;
    }

    public abstract void OnBodyEntered(Node body);
    public abstract void OnBodyExited(Node body);
    public abstract void Process(double delta);
    public abstract void UpdateLabel();
}

//////////////////////////////////////////////////
// TapMechanism
public partial class TapMechanism : ChestMechanism
{
    private int remainingTaps;

    public TapMechanism(Chest chest) : base(chest)
    {
        remainingTaps = chest.TapsRequired;
    }

    public override void OnBodyEntered(Node body)
    {
        if (body.IsInGroup("player"))
        {
            chest.InfoLabel.Visible = true;
            remainingTaps = chest.TapsRequired;
        }
    }

    public override void OnBodyExited(Node body)
    {
        if (body.IsInGroup("player"))
        {
            chest.InfoLabel.Visible = false;
        }
    }

    public override void Process(double delta)
    {
        if (Input.IsActionJustPressed("ui_select"))
        {
            remainingTaps--;

            if (remainingTaps == 0)
            {
                chest.CollectChest("OnTapChestCollect");
            }
        }
    }

    public override void UpdateLabel()
    {
        chest.InfoLabel.Text = $"Tap 'Q' key to collect | ({remainingTaps} taps)";
    }
}

//////////////////////////////////////////////////
// TimerMechanism
public partial class TimerMechanism : ChestMechanism
{
    private Timer timer;

    public TimerMechanism(Chest chest) : base(chest)
    {
        timer = chest.GetNode<Timer>("Timer");
        timer.WaitTime = chest.WaitTime;
        timer.Connect("timeout", new Callable(this, nameof(OnTimerCollectChest)));
    }

    public override void OnBodyEntered(Node body)
    {
        if (body.IsInGroup("player"))
        {
            chest.InfoLabel.Visible = true;
            timer.Start();
        }
    }

    public override void OnBodyExited(Node body)
    {
        if (body.IsInGroup("player"))
        {
            chest.InfoLabel.Visible = false;
            timer.Stop();
        }
    }

    public override void Process(double delta) { }

    public override void UpdateLabel()
    {
        double timeRemaining = timer.TimeLeft;
        chest.InfoLabel.Text = $"{timeRemaining:F1}s";
    }

    private void OnTimerCollectChest()
    {
        chest.CollectChest("OnTimerCollectChest");
    }
}

//////////////////////////////////////////////////
// Base Chest class
public partial class Chest : Area2D
{
    [Export] public float WaitTime = 4.0f;
    [Export] public int TapsRequired = 3;
    private bool isTapMechanismEnabled;
    private Label infoLabel;
    private Player player;

    private ChestMechanism chestMechanism;

    public Label InfoLabel => infoLabel;

    public override void _Ready()
    {
        isTapMechanismEnabled = ConfigCatService.Instance.isTapMechanismEnabled;
        InitializeNodes();
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;

        chestMechanism = isTapMechanismEnabled ? (ChestMechanism)new TapMechanism(this) : new TimerMechanism(this);
    }

    private void InitializeNodes()
    {
        player = GetNode<Player>("/root/Game/Player");
        infoLabel = GetNode<Label>("InfoLabel");
        infoLabel.Visible = false;
    }

    public override void _Process(double delta)
    {
        if (infoLabel.Visible)
        {
            chestMechanism.Process(delta);
        }
        UpdateLabel();
    }

    private void OnBodyEntered(Node body)
    {
        chestMechanism.OnBodyEntered(body);
    }

    private void OnBodyExited(Node body)
    {
        chestMechanism.OnBodyExited(body);
    }

    public void CollectChest(string collectionMethod)
    {
        GameManager.Instance.ChestCollected(collectionMethod);
        QueueFree();
    }

    private void UpdateLabel()
    {
        chestMechanism.UpdateLabel();
    }
}
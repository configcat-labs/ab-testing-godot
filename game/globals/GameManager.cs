using Godot;
public partial class GameManager : Node2D
{
    public static GameManager Instance { get; private set; }
    private Label scoreLabel, statusMessage;
    private int chestsCollected = 0;
    private int coinsCollected = 0;
    private int maxTime = 250;
    private double elapsedTime = 0.0f;
    private int timeRemainging = 0;
    private bool gameEnded = false;
    private string userId;
    private AmplitudeAnalytics amplitude;

    public override void _Ready()
    {
        Instance = this;
        amplitude = GetNode<AmplitudeAnalytics>("/root/AmplitudeAnalytics");
        // create a unique user id
        userId = System.Guid.NewGuid().ToString();
        GD.Print("User ID: " + userId);
        // Set UserObject for ConfiGcat
        ConfigCatService.Instance.SetUserObject(userId);
    }

    public override void _Process(double delta)
    {
        if (!gameEnded)
        {
            timeRemainging = maxTime - (int)elapsedTime;
            elapsedTime += delta;

            if (timeRemainging <= 0)
            {
                EndGame();
            }
        }
        UpdateScore();
    }

    // Sets the reference to the score label
    public void SetScoreLabel(Label label)
    {
        scoreLabel = label;
    }

    // Called when a chest is collected
    public void ChestCollected(string chestType)
    {
        if (chestType == "OnTapChestCollect")
        {
            chestsCollected++;
            amplitude.SendAmplitudeEvent("OnTap Chests Collected", userId);
        }
        else if (chestType == "OnTimerCollectChest")
        {
            chestsCollected++;
            amplitude.SendAmplitudeEvent("OnTimer Chests Collected", userId);
        }
    }


    // Called when a coin is collected
    public void CoinCollected()
    {
        coinsCollected++;
    }

    // Called when the player enters the exit portal
    public void EnterExitPortal()
    {
        statusMessage = GetNode<Label>("/root/Game/HUD/CanvasGroup/stats/message");
        gameEnded = true;
        statusMessage.Text = "You Win! Restart Game";
        statusMessage.Visible = true;
        GetTree().Paused = true;
    }

    // Ends the game when the time runs out
    public void EndGame()
    {
        statusMessage = GetNode<Label>("/root/Game/HUD/CanvasGroup/stats/message");
        gameEnded = true;
        GetTree().Paused = true;
        statusMessage.Visible = true;
        statusMessage.Text = "Game Over! Restart Game";
    }

    // score displayed on the screen
    private void UpdateScore()
    {
        if (scoreLabel != null)
        {
            scoreLabel.Text = $" Time:{timeRemainging} Chests: {chestsCollected} Coins: {coinsCollected}";
        }
    }
}
using Godot;

public partial class HUD_ABDisplay : Label
{
    public override void _Ready()
    {
        var abChestFlag = ConfigCatService.Instance.isTapMechanismEnabled;
        if (abChestFlag)
        {
            Text = "Variant B";
        }
        else
        {
            Text = "Variant A";
        }
    }
}

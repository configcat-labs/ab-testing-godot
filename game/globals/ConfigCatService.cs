using Godot;
using System;
using ConfigCat.Client;

public partial class ConfigCatService : Node
{
    public static ConfigCatService Instance;
    private static string ConfigCatApiKey = "YOUR-KEY-HERE";
    private string ConfigCatFlagName = "chestCollectingMechanism"; // Make sure to match the flag name exactly as it appears in ConfigCat's dashboard.
    private object _userObject;
    private IConfigCatClient configCatClient = ConfigCatClient.Get(ConfigCatApiKey);
    public bool isTapMechanismEnabled { get; private set; } = false;


    public override void _EnterTree()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            GD.PrintErr("More than one instance of ConfigCat_Service detected. Only one instance should exist.");
            QueueFree();
        }
    }  

    public void SetUserObject(string userId)
    {
        _userObject = new { identifier = userId };
        FetchAbChestFlag();
    }

    private void FetchAbChestFlag()
    {
        try
        {
            var user = new ConfigCat.Client.User(_userObject.ToString());
            isTapMechanismEnabled = configCatClient.GetValue(ConfigCatFlagName, false, user);
        }
        catch (Exception e)
        {
            GD.PrintErr("Error fetching feature flag: " + e.Message);
        }
    }

}

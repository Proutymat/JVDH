using System;

[Serializable]
public class SaveData
{
    
    public SettingsData settings;
    public ProgressionData progression;
    public StatsData stats;
    
    public SaveData()
    {
        settings = new SettingsData();
        progression = new ProgressionData();
        stats = new StatsData();
    }
}
using System.Text.Json;

namespace GeneticDFA.Utility;

public class SettingsService : ISettingsService
{
    private readonly string _settingsFilePath;

    public SettingsService()
    {
        string appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string settingsFolder = Path.Combine(appDataDir, "GeneticDFA");
        Directory.CreateDirectory(settingsFolder);
        _settingsFilePath = Path.Combine(settingsFolder, "settings.json");
    }

    public Settings LoadSettings()
    {
        try
        {
            if (File.Exists(_settingsFilePath))
            {
                string jsonString = File.ReadAllText(_settingsFilePath);
                Settings? settings = JsonSerializer.Deserialize<Settings>(jsonString);
                return settings!;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error loading settings from file");
        }

        // Return default settings if file doesn't exist or an error occurs
        return new Settings();
    }

    public void SaveSettings(Settings settings)
    {
        try
        {
            string jsonString = JsonSerializer.Serialize(settings);
            File.WriteAllText(_settingsFilePath, jsonString);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error saving settings to file");
            throw;
        }
    }
}

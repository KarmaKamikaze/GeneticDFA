namespace GeneticDFA.Utility;

public interface ISettingsService
{
    Settings LoadSettings();
    void SaveSettings(Settings settings);
}

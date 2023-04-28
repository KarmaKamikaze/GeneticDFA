using System.Collections.Generic;
using ReactiveUI;

namespace GeneticDFAUI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    ViewModelBase? _content;

    public MainWindowViewModel(List<string> list)
    {
        Visualization = new VisualizationViewModel(list);
        Content = Settings = new SettingsViewModel();
    }

    public ViewModelBase? Content
    {
        get => _content;
        private set => this.RaiseAndSetIfChanged(ref _content, value);
    }

    public VisualizationViewModel Visualization { get; }

    public SettingsViewModel Settings { get; }

    public void SwitchContent()
    {
        Content = Content == Settings ? Visualization : Settings;
    }

    private void StartAlgorithm()
    {
        // ... code to start the algorithm ...
        // Use process?

        SwitchContent();
    }
}

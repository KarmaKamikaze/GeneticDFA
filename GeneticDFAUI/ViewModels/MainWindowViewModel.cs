using ReactiveUI;

namespace GeneticDFAUI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    ViewModelBase? _content;

    public MainWindowViewModel()
    {
        Content = Settings = new SettingsViewModel();
    }

    public ViewModelBase? Content
    {
        get => _content;
        private set => this.RaiseAndSetIfChanged(ref _content, value);
    }

    public SettingsViewModel Settings { get; }
}

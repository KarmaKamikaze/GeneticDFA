using System.Collections.Generic;
using ReactiveUI;

namespace GeneticDFAUI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    ViewModelBase? _content;

    public MainWindowViewModel(List<string> list)
    {
        Content = List = new VisualizationViewModel(list);
    }

    public ViewModelBase? Content
    {
        get => _content;
        private set => this.RaiseAndSetIfChanged(ref _content, value);
    }

    public VisualizationViewModel List { get; }
}

using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using GeneticDFAUI.ViewModels;
using GeneticDFAUI.Views;

namespace GeneticDFAUI;

public partial class App : Application
{
    public List<string> TestList => new()
    {
        "Gen1", "Gen2", "Gen3", "Gen4", "Gen5", "Gen6", "Gen7", "Gen8", "Gen9", "Gen10", "Gen11", "Gen12", "Gen13",
        "Gen14", "Gen15", "Gen16", "Gen17", "Gen18", "Gen19", "Gen20", "Gen21", "Gen22", "Gen23", "Gen24", "Gen25", "Gen26", "Gen27",
        "Gen28", "Gen29", "Gen30", "Gen31", "Gen32", "Gen33", "Gen34", "Gen35", "Gen36", "Gen37", "Gen38", "Gen39", "Gen40",
    };

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(TestList),
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}

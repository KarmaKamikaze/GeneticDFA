using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace GeneticDFAUI.Views;

public partial class VisualizationView : UserControl
{
    public VisualizationView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}

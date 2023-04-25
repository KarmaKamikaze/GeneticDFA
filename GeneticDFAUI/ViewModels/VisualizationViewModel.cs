using System.Collections.Generic;
using Avalonia.Media.Imaging;

namespace GeneticDFAUI.ViewModels;

public class VisualizationViewModel : ViewModelBase
{
    public VisualizationViewModel(List<string> list)
    {
        MyItems = list;
    }
    public List<string> MyItems { get; }

    public Bitmap Image { get; set; }
}

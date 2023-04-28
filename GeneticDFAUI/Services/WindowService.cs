using System;
using Avalonia.Controls;

namespace GeneticDFAUI.Services;

public class WindowService : IWindowService
{
    public static WindowService Instance { get; } = new WindowService();

    public void Show(object view)
    {
        if (view is Window window)
        {
            window.ShowDialog(window);
        }
        else
        {
            throw new ArgumentException($"View is not a window: {view}");
        }
    }

    public void Close(object view)
    {
        if (view is Window window)
        {
            window.Close();
        }
        else
        {
            throw new ArgumentException($"View is not a window: {view}");
        }
    }
}

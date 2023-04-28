using System;
using GeneticDFAUI.ViewModels;
using ReactiveUI;

namespace GeneticDFAUI.Services;

public class ViewLocator : IViewLocator
{
    public Type LocateViewType(Type viewModelType)
    {
        string viewTypeName = viewModelType.AssemblyQualifiedName!.Replace("ViewModel", "View");
        Type? viewType = Type.GetType(viewTypeName);

        if (viewType == null)
        {
            throw new ArgumentException($"View not found for {viewModelType}.");
        }

        return viewType;
    }

    public static ViewModelBase LocateForModel(object? model, string? contract = null, object? context = null)
    {
        if (model == null)
            return null;

        var viewModelType = model.GetType();
        var viewTypeName = viewModelType.FullName!.Replace("ViewModel", "View");
        var viewType = Type.GetType(viewTypeName);

        if (viewType == null)
            throw new ArgumentException($"Could not find view type for view model type '{viewModelType.FullName}'");

        var view = Activator.CreateInstance(viewType) as ViewModelBase;

        if (view == null)
            throw new ArgumentException($"The view type '{viewType.FullName}' does not implement the IView interface");

        return view;
    }
}

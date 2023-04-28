using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GeneticDFAUI.ViewModels;

namespace GeneticDFAUI.Services;

public class NavigationService : INavigationService
{
    private readonly Dictionary<Type, Func<Task<ViewModelBase>>> _viewModels = new Dictionary<Type, Func<Task<ViewModelBase>>>();

    public void Register<TViewModel>(Func<Task<ViewModelBase>> viewModelFactory)
        where TViewModel : ViewModelBase
    {
        _viewModels.Add(typeof(TViewModel), viewModelFactory);
    }

    public async Task<bool> NavigateToAsync<TViewModel>()
        where TViewModel : ViewModelBase
    {
        if (!_viewModels.TryGetValue(typeof(TViewModel), out Func<Task<ViewModelBase>>? viewModelFactory))
        {
            return false;
        }

        ViewModelBase? viewModel = await viewModelFactory();
        if (viewModel == null)
        {
            return false;
        }

        var view = ViewLocator.LocateForModel(viewModel);
        if (view == null)
        {
            return false;
        }

        //view.DataContext = viewModel;
        WindowService.Instance.Show(view);

        return true;
    }
}

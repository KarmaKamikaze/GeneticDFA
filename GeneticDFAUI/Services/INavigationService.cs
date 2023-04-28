using System;
using System.Threading.Tasks;
using GeneticDFAUI.ViewModels;

namespace GeneticDFAUI.Services;

public interface INavigationService
{
    void Register<TViewModel>(Func<Task<ViewModelBase>> viewModelFactory)
        where TViewModel : ViewModelBase;

    Task<bool> NavigateToAsync<TViewModel>()
        where TViewModel : ViewModelBase;
}

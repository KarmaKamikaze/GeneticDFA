using System;

namespace GeneticDFAUI.Services;

public interface IViewLocator
{
    Type LocateViewType(Type viewModelType);
}

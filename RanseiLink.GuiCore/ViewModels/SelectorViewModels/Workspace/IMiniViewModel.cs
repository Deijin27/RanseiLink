#nullable enable
using System.ComponentModel;

namespace RanseiLink.GuiCore.ViewModels;

public interface IMiniViewModel
{
    int Id { get; }
    ICommand SelectCommand { get; }
    void NotifyPropertyChanged(string? name);
    bool MatchSearchTerm(string searchTerm);
}

public interface IBigViewModel : INotifyPropertyChanged
{
    void SetModel(int id, object model);
}
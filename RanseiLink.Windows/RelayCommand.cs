using System;
using System.Windows.Input;

namespace RanseiLink.Windows;

internal class RelayCommand : ICommand
{
    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    private readonly Action _execute;
    private readonly Func<bool> _canExecute;

    public RelayCommand(Action execute, Func<bool> canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter)
    {
        return _canExecute == null || _canExecute();
    }

    public void Execute(object parameter)
    {
        _execute();
    }
}

internal class RelayCommand<T> : ICommand
{
    public event EventHandler CanExecuteChanged
    {
        add { CommandManager.RequerySuggested += value; }
        remove { CommandManager.RequerySuggested -= value; }
    }

    private readonly Action<T> _execute;
    private readonly Func<T, bool> _canExecute;

    public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public RelayCommand(Action execute)
    {
        _execute = o => execute();
    }

    public bool CanExecute(object parameter)
    {
        if (parameter != null && !(parameter is T))
        {
            throw new InvalidCastException($"In {nameof(RelayCommand)} unable to cast parameter of type {parameter.GetType().Name} to {typeof(T).Name}");
        }
        return _canExecute == null || _canExecute((T)parameter);
    }

    public void Execute(object parameter)
    {
        if (parameter != null && !(parameter is T))
        {
            throw new InvalidCastException($"In {nameof(RelayCommand)} unable to cast parameter of type {parameter.GetType().Name} to {typeof(T).Name}");
        }
        _execute((T)parameter);
    }
}

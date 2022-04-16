using RanseiLink.Core.Enums;
using RanseiLink.Core.Maps;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace RanseiLink.ViewModels;

public class MapPokemonPositionViewModel : ViewModelBase
{
    private readonly MapViewModel _parent;
    private readonly Position[] _positions;
    private readonly int _positionId;

    public MapPokemonPositionViewModel(MapViewModel parent, Position[] positions, int positionId)
    {
        _parent = parent;
        _positions = positions;
        _positionId = positionId;

        IncrementXCommand = new RelayCommand(() =>
        {
            if (X < _parent.Width)
            {
                ChangePosition(() => X++);
            }
        });
        DecrementXCommand = new RelayCommand(() =>
        {
            if (X > 0)
            {
                ChangePosition(() => X--);
            }
        });
        IncrementYCommand = new RelayCommand(() =>
        {
            if (Y < _parent.Height)
            {
                ChangePosition(() => Y++);
            }
        });
        DecrementYCommand = new RelayCommand(() =>
        {
            if (Y > 0)
            {
                ChangePosition(() => Y--);
            }
        });
    }

    private void ChangePosition(Action doThePositionChange)
    {
        var sourceCell = _parent.Matrix[Y][X];
        sourceCell.Pokemon.Remove(this);
        doThePositionChange();
        var destinationCell = _parent.Matrix[Y][X];
        destinationCell.Pokemon.Add(this);
        _parent.SelectedCell = destinationCell;

    }

    public int Id => _positionId;

    public byte X
    {
        get => _positions[_positionId].X;
        set => RaiseAndSetIfChanged(X, value, v => _positions[_positionId].X = v);
    }

    public byte Y
    {
        get => _positions[_positionId].Y;
        set => RaiseAndSetIfChanged(Y, value, v => _positions[_positionId].Y = v);
    }

    public ICommand IncrementXCommand { get; }
    public ICommand DecrementXCommand { get; }
    public ICommand IncrementYCommand { get; }
    public ICommand DecrementYCommand { get; }
}

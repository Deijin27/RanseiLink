using RanseiLink.Core.Maps;

namespace RanseiLink.GuiCore.ViewModels;

public class MapPokemonPositionViewModel : ViewModelBase
{
    private readonly MapViewModel _parent;
    private readonly MapPokemonItem[] _positions;
    private readonly int _positionId;

    public MapPokemonPositionViewModel(MapViewModel parent, MapPokemonItem[] positions, int positionId)
    {
        _parent = parent;
        _positions = positions;
        _positionId = positionId;

        IncrementXCommand = new RelayCommand(() =>
        {
            if (X < _parent.Width - 1)
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
            if (Y < _parent.Height - 1)
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
        RotateCommand = new RelayCommand(() =>
        {
            Orientation = Orientation switch
            {
                OrientationAlt.East => OrientationAlt.South,
                OrientationAlt.South => OrientationAlt.West,
                OrientationAlt.West => OrientationAlt.North,
                OrientationAlt.North => OrientationAlt.East,
                _ => OrientationAlt.East,
            };
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
        set => SetProperty(X, value, v => _positions[_positionId].X = v);
    }

    public byte Y
    {
        get => _positions[_positionId].Y;
        set => SetProperty(Y, value, v => _positions[_positionId].Y = v);
    }

    public OrientationAlt Orientation
    {
        get => _positions[_positionId].Orientation;
        set => SetProperty(Orientation, value, v => _positions[_positionId].Orientation = v);
    }

    public byte Unknown2
    {
        get => _positions[_positionId].Unknown2;
        set => SetProperty(Unknown2, value, v => _positions[_positionId].Unknown2 = v);
    }

    public ICommand IncrementXCommand { get; }
    public ICommand DecrementXCommand { get; }
    public ICommand IncrementYCommand { get; }
    public ICommand DecrementYCommand { get; }
    public ICommand RotateCommand { get; }
}

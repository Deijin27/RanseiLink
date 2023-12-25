using RanseiLink.Core.Enums;
using RanseiLink.Core.Maps;

namespace RanseiLink.Windows.ViewModels;

public class MapGimmickViewModel : ViewModelBase
{
    private readonly MapViewModel _parent;

    public MapGimmickItem GimmickItem { get; }

    public MapGimmickViewModel(MapViewModel parent, MapGimmickItem gimmick)
    {
        _parent = parent;
        GimmickItem = gimmick;

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
                Orientation.East => Orientation.South,
                Orientation.South => Orientation.West,
                Orientation.West => Orientation.North,
                Orientation.North => Orientation.Default,
                Orientation.Default => Orientation.East,
                _ => Orientation.Default
            };
        });
    }

    private void ChangePosition(Action doThePositionChange)
    {
        var sourceCell = _parent.Matrix[Y][X];
        sourceCell.RemoveGimmick(this);
        doThePositionChange();
        var destinationCell = _parent.Matrix[Y][X];
        destinationCell.AddGimmick(this);
        _parent.SelectedCell = destinationCell;
    }

    public GimmickId Gimmick
    {
        get => GimmickItem.Gimmick;
        set => RaiseAndSetIfChanged(Gimmick, value, v => GimmickItem.Gimmick = v);
    }

    public byte X
    {
        get => GimmickItem.Position.X;
        set => RaiseAndSetIfChanged(X, value, v => GimmickItem.Position.X = v);
    }

    public byte Y
    {
        get => GimmickItem.Position.Y;
        set => RaiseAndSetIfChanged(Y, value, v => GimmickItem.Position.Y = v);
    }

    public Orientation Orientation
    {
        get => GimmickItem.Orientation;
        set => RaiseAndSetIfChanged(Orientation, value, v => GimmickItem.Orientation = value);
    }

    public int UnknownValue
    {
        get => GimmickItem.UnknownValue;
        set => RaiseAndSetIfChanged(UnknownValue, value, v => GimmickItem.UnknownValue = value);
    }

    public string Params
    {
        get => string.Join(';', GimmickItem.UnknownList.Select(i => $"{i.Item1},{i.Item2}"));
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                GimmickItem.UnknownList.Clear();
            }
            else
            {
                var newList = new List<(ushort, ushort)>();
                var pairs = value.Split(';');
                foreach (var pair in pairs)
                {
                    var splitPair = pair.Split(',');
                    if (splitPair.Length != 2 || !ushort.TryParse(splitPair[0], out ushort v1) || !ushort.TryParse(splitPair[1], out ushort v2))
                    {
                        return;
                    }
                    newList.Add((v1, v2));
                }
                GimmickItem.UnknownList.Clear();
                GimmickItem.UnknownList.AddRange(newList);
            }
            RaisePropertyChanged();
        }
    }

    public ICommand IncrementXCommand { get; }
    public ICommand DecrementXCommand { get; }
    public ICommand IncrementYCommand { get; }
    public ICommand DecrementYCommand { get; }
    public ICommand RotateCommand { get; }
}

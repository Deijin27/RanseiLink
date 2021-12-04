using RanseiLink.Core.Models.Interfaces;

namespace RanseiLink.ViewModels;

public delegate MoveRangeViewModel MoveRangeViewModelFactory(IMoveRange model);

public class MoveRangeViewModel : ViewModelBase
{
    private readonly IMoveRange _model;

    public MoveRangeViewModel(IMoveRange model)
    {
        _model = model;
    }

    #region Row0
    public bool Row0Col0
    {
        get => _model.GetInRange(3 * 8 + 4);
        set => _model.SetInRange(3 * 8 + 4, value);
    }

    public bool Row0Col1
    {
        get => _model.GetInRange(3 * 8 + 5);
        set => _model.SetInRange(3 * 8 + 5, value);
    }

    public bool Row0Col2
    {
        get => _model.GetInRange(3 * 8 + 1);
        set => _model.SetInRange(3 * 8 + 1, value);
    }

    public bool Row0Col3
    {
        get => _model.GetInRange(3 * 8 + 2);
        set => _model.SetInRange(3 * 8 + 2, value);
    }

    public bool Row0Col4
    {
        get => _model.GetInRange(3 * 8 + 3);
        set => _model.SetInRange(3 * 8 + 3, value);
    }

    #endregion

    #region Row1

    public bool Row1Col0
    {
        get => _model.GetInRange(2 * 8 + 7);
        set => _model.SetInRange(2 * 8 + 7, value);
    }

    public bool Row1Col1
    {
        get => _model.GetInRange(3 * 8 + 0);
        set => _model.SetInRange(3 * 8 + 0, value);
    }

    public bool Row1Col2
    {
        get => _model.GetInRange(1 * 8 + 1);
        set => _model.SetInRange(1 * 8 + 1, value);
    }

    public bool Row1Col3
    {
        get => _model.GetInRange(1 * 8 + 2);
        set => _model.SetInRange(1 * 8 + 2, value);
    }

    public bool Row1Col4
    {
        get => _model.GetInRange(1 * 8 + 3);
        set => _model.SetInRange(1 * 8 + 3, value);
    }

    #endregion

    #region Row2

    public bool Row2Col0
    {
        get => _model.GetInRange(2 * 8 + 6);
        set => _model.SetInRange(2 * 8 + 6, value);
    }

    public bool Row2Col1
    {
        get => _model.GetInRange(1 * 8 + 0);
        set => _model.SetInRange(1 * 8 + 0, value);
    }

    public bool Row2Col2
    {
        get => _model.GetInRange(0 * 8 + 1);
        set => _model.SetInRange(0 * 8 + 1, value);
    }

    public bool Row2Col3
    {
        get => _model.GetInRange(0 * 8 + 2);
        set => _model.SetInRange(0 * 8 + 2, value);
    }

    public bool Row2Col4
    {
        get => _model.GetInRange(1 * 8 + 4);
        set => _model.SetInRange(1 * 8 + 4, value);
    }

    #endregion

    #region Row3

    public bool Row3Col0
    {
        get => _model.GetInRange(2 * 8 + 5);
        set => _model.SetInRange(2 * 8 + 5, value);
    }

    public bool Row3Col1
    {
        get => _model.GetInRange(0 * 8 + 7);
        set => _model.SetInRange(0 * 8 + 7, value);
    }

    public bool Row3Col2
    {
        get => _model.GetInRange(0 * 8 + 0);
        set => _model.SetInRange(0 * 8 + 0, value);
    }

    public bool Row3Col3
    {
        get => _model.GetInRange(0 * 8 + 3);
        set => _model.SetInRange(0 * 8 + 3, value);
    }

    public bool Row3Col4
    {
        get => _model.GetInRange(1 * 8 + 5);
        set => _model.SetInRange(1 * 8 + 5, value);
    }

    #endregion

    #region Row4

    public bool Row4Col0
    {
        get => _model.GetInRange(2 * 8 + 4);
        set => _model.SetInRange(2 * 8 + 4, value);
    }

    public bool Row4Col1
    {
        get => _model.GetInRange(0 * 8 + 6);
        set => _model.SetInRange(0 * 8 + 6, value);
    }

    public bool Row4Col2
    {
        get => _model.GetInRange(0 * 8 + 5);
        set => _model.SetInRange(0 * 8 + 5, value);
    }

    public bool Row4Col3
    {
        get => _model.GetInRange(0 * 8 + 4);
        set => _model.SetInRange(0 * 8 + 4, value);
    }

    public bool Row4Col4
    {
        get => _model.GetInRange(1 * 8 + 6);
        set => _model.SetInRange(1 * 8 + 6, value);
    }

    #endregion

    #region Row5

    public bool Row5Col0
    {
        get => _model.GetInRange(2 * 8 + 3);
        set => _model.SetInRange(2 * 8 + 3, value);
    }

    public bool Row5Col1
    {
        get => _model.GetInRange(2 * 8 + 2);
        set => _model.SetInRange(2 * 8 + 2, value);
    }

    public bool Row5Col2
    {
        get => _model.GetInRange(2 * 8 + 1);
        set => _model.SetInRange(2 * 8 + 1, value);
    }

    public bool Row5Col3
    {
        get => _model.GetInRange(2 * 8 + 0);
        set => _model.SetInRange(2 * 8 + 0, value);
    }
    public bool Row5Col4
    {
        get => _model.GetInRange(1 * 8 + 7);
        set => _model.SetInRange(1 * 8 + 7, value);
    }

    #endregion

}

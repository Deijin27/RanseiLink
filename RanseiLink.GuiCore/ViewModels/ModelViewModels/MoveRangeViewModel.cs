#nullable enable
using RanseiLink.Core.Models;

namespace RanseiLink.GuiCore.ViewModels;

public class MoveRangeViewModel : ViewModelBase
{
    private MoveRange _model;

    public MoveRangeViewModel()
    {
        _model = new MoveRange();
    }

    public void SetModel(MoveRange model)
    {
        _model = model;
        RaiseAllPropertiesChanged();
    }

    

    #region Row0
    public bool Row0Col0
    {
        get => _model.GetInRange(0, 0);
        set => _model.SetInRange(0, 0, value);
    }

    public bool Row0Col1
    {
        get => _model.GetInRange(0, 1);
        set => _model.SetInRange(0, 1, value);
    }

    public bool Row0Col2
    {
        get => _model.GetInRange(0, 2);
        set => _model.SetInRange(0, 2, value);
    }

    public bool Row0Col3
    {
        get => _model.GetInRange(0, 3);
        set => _model.SetInRange(0, 3, value);
    }

    public bool Row0Col4
    {
        get => _model.GetInRange(0, 4);
        set => _model.SetInRange(0, 4, value);
    }

    #endregion

    #region Row1

    public bool Row1Col0
    {
        get => _model.GetInRange(1, 0);
        set => _model.SetInRange(1, 0, value);
    }

    public bool Row1Col1
    {
        get => _model.GetInRange(1, 1);
        set => _model.SetInRange(1, 1, value);
    }

    public bool Row1Col2
    {
        get => _model.GetInRange(1, 2);
        set => _model.SetInRange(1, 2, value);
    }

    public bool Row1Col3
    {
        get => _model.GetInRange(1, 3);
        set => _model.SetInRange(1, 3, value);
    }

    public bool Row1Col4
    {
        get => _model.GetInRange(1, 4);
        set => _model.SetInRange(1, 4, value);
    }

    #endregion

    #region Row2

    public bool Row2Col0
    {
        get => _model.GetInRange(2, 0);
        set => _model.SetInRange(2, 0, value);
    }

    public bool Row2Col1
    {
        get => _model.GetInRange(2, 1);
        set => _model.SetInRange(2, 1, value);
    }

    public bool Row2Col2
    {
        get => _model.GetInRange(2, 2);
        set => _model.SetInRange(2, 2, value);
    }

    public bool Row2Col3
    {
        get => _model.GetInRange(2, 3);
        set => _model.SetInRange(2, 3, value);
    }

    public bool Row2Col4
    {
        get => _model.GetInRange(2, 4);
        set => _model.SetInRange(2, 4, value);
    }

    #endregion

    #region Row3

    public bool Row3Col0
    {
        get => _model.GetInRange(3, 0);
        set => _model.SetInRange(3, 0, value);
    }

    public bool Row3Col1
    {
        get => _model.GetInRange(3, 1);
        set => _model.SetInRange(3, 1, value);
    }

    public bool Row3Col2
    {
        get => _model.GetInRange(3, 2);
        set => _model.SetInRange(3, 2, value);
    }

    public bool Row3Col3
    {
        get => _model.GetInRange(3, 3);
        set => _model.SetInRange(3, 3, value);
    }

    public bool Row3Col4
    {
        get => _model.GetInRange(3, 4);
        set => _model.SetInRange(3, 4, value);
    }

    #endregion

    #region Row4

    public bool Row4Col0
    {
        get => _model.GetInRange(4, 0);
        set => _model.SetInRange(4, 0, value);
    }

    public bool Row4Col1
    {
        get => _model.GetInRange(4, 1);
        set => _model.SetInRange(4, 1, value);
    }

    public bool Row4Col2
    {
        get => _model.GetInRange(4, 2);
        set => _model.SetInRange(4, 2, value);
    }

    public bool Row4Col3
    {
        get => _model.GetInRange(4, 3);
        set => _model.SetInRange(4, 3, value);
    }

    public bool Row4Col4
    {
        get => _model.GetInRange(4, 4);
        set => _model.SetInRange(4, 4, value);
    }

    #endregion

    #region Row5

    public bool Row5Col0
    {
        get => _model.GetInRange(5, 0);
        set => _model.SetInRange(5, 0, value);
    }

    public bool Row5Col1
    {
        get => _model.GetInRange(5, 1);
        set => _model.SetInRange(5, 1, value);
    }

    public bool Row5Col2
    {
        get => _model.GetInRange(5, 2);
        set => _model.SetInRange(5, 2, value);
    }

    public bool Row5Col3
    {
        get => _model.GetInRange(5, 3);
        set => _model.SetInRange(5, 3, value);
    }
    public bool Row5Col4
    {
        get => _model.GetInRange(5, 4);
        set => _model.SetInRange(5, 4, value);
    }

    #endregion

}

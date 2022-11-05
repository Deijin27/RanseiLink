using RanseiLink.Core.Text;
using RanseiLink.Services;
using System;
using System.Collections;
using System.ComponentModel;
using System.IO;

namespace RanseiLink.ViewModels;

public class MsgViewModel : INotifyDataErrorInfo
{
    private readonly ChangeTrackedBlock _block;

    public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

    public Message Message => _block[Id];
    public MsgViewModel(int blockId, int id, ChangeTrackedBlock block)
    {
        BlockId = blockId;
        Id = id;
        _block = block;
        var msg = _block[Id];
        _text = msg.Text;
        _context = msg.Context;
        _boxConfig = msg.BoxConfig;
    }

    public ChangeTrackedBlock Block => _block;
    public int BlockId { get; }
    public int Id { get; set; }

    private string _text;
    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            Validate();
            if (!HasErrors)
            {
                UpdateSource();
            }
            
        }
    }

    private string _context;
    public string Context
    {
        get => _context;
        set
        {
            _context = value;
            Validate();
            if (!HasErrors)
            {
                UpdateSource();
            }
        }
    }

    private string _boxConfig;
    public string BoxConfig
    {
        get => _boxConfig;
        set
        {
            _boxConfig = value;
            Validate();
            if (!HasErrors)
            {
                UpdateSource();
            }
        }
    }

    public int GroupId => _block[Id].GroupId;

    public int ElementId => _block[Id].ElementId;

    private bool _hasErrors = false;
    public bool HasErrors
    {
        get => _hasErrors;
        set
        {
            if (_hasErrors != value)
            {
                _hasErrors = value;
                ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(string.Empty));
            }
        }
    }

    public IEnumerable GetErrors(string propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            if (HasErrors)
            {
                return new string[] { _error };
            }
        }
        return null;
    }

    private string _error;
    private void Validate()
    {
        var testMsg = new Message()
        {
            Text = Text,
            BoxConfig = BoxConfig,
            GroupId = GroupId,
            ElementId = ElementId,
            Context = Context,
        };
        var stream = new BinaryWriter(new MemoryStream());

        try
        {
            var pnaWriter = new PnaTextWriter(stream);
            pnaWriter.WriteMessage(testMsg, false);

            _error = null;
            HasErrors = false;
        }
        catch (Exception e)
        {
            _error =  e.Message;
            HasErrors = true;
        }
        finally
        {
            stream.Dispose();
        }
    }

    private void UpdateSource()
    {
        var msg = _block[Id];
        msg.Text = _text;
        msg.BoxConfig = _boxConfig;
        msg.Context = _context;
        _block.IsChanged = true;
    }
}

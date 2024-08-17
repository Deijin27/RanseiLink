namespace RanseiLink.Core.Services.ModelServices;

public abstract class BaseModelService<TModel> : IModelService<TModel>
{
    protected readonly string _dataFile;
    protected readonly int _minId;
    protected readonly int _maxId;
    protected readonly int _defaultId;
    protected readonly bool _hasDefaultId;
    protected readonly List<TModel> _cache = [];

    protected BaseModelService(string dataFile, int minId, int maxId, int defaultId = -1, bool delayReload = false)
    {
        _dataFile = dataFile;
        _minId = minId;
        _maxId = maxId;
        _defaultId = defaultId;
        _hasDefaultId = defaultId > 0;
        if (!delayReload)
        {
            Reload();
        }
    }


    public bool ValidateId(int id)
    {
        return id >= _minId && id <= _maxId;
    }

    public TModel Retrieve(int id)
    {
        if (!ValidateId(id))
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }
        return _cache[id];
    }

    public object RetrieveObject(int id) => Retrieve(id)!;

    public abstract string IdToName(int id);

    public IEnumerable<int> ValidIds()
    {
        return Enumerable.Range(_minId, _maxId - _minId + 1);
    }

    public IEnumerable<TModel> Enumerate() => _cache;

    public abstract void Reload();

    public abstract void Save();

    public bool TryGetDefaultId(out int defaultId)
    {
        defaultId = _defaultId;
        return _hasDefaultId;
    }
} 
using RanseiLink.Core.Services;
using System.Collections.Generic;

namespace RanseiLink.Core.Randomization;

public class RandomizerOption
{
    public RandomizerOption(string id, string displayName, string description = "", bool defaultEnabled = true)
    {
        Id = id;
        DisplayName = displayName;
        Enabled = defaultEnabled;
        Description = description;
    }
    public string Id { get; }
    public string DisplayName { get; }
    public string Description { get; }
    public bool Enabled { get; set; }
}

public interface IRandomizer
{
    bool Enabled { get; set; }
    string DisplayName { get; }
    IEnumerable<RandomizerOption> Options { get; }
    void Apply(IDataService service);
}

public abstract class RandomizerBase : IRandomizer
{
    public RandomizerBase(string displayName, string description = "", bool defaultEnabled = true)
    {
        Enabled = defaultEnabled;
        DisplayName = displayName;
        Description = description;
    }
    public bool Enabled { get; set; }
    public string DisplayName { get; }
    public string Description { get; }

    protected void AddOption(string id, string displayName, string description = "", bool defaultEnabled = true)
    {
        OptionDict.Add(id, new RandomizerOption(id, displayName, description, defaultEnabled));
    }

    protected IDictionary<string, RandomizerOption> OptionDict = new Dictionary<string, RandomizerOption>();

    public IEnumerable<RandomizerOption> Options => OptionDict.Values;

    public abstract void Apply(IDataService service);
}

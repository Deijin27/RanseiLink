using Core.Services;
using System.Collections.Generic;

namespace Core.Randomization
{
    public class RandomizerOption
    {
        public RandomizerOption(string id, string displayName, bool defaultEnabled = true)
        {
            Id = id;
            DisplayName = displayName;
            Enabled = defaultEnabled;
        }
        public string Id { get; }
        public string DisplayName { get; }
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
        public RandomizerBase(string displayName, bool defaultEnabled = true)
        {
            Enabled = defaultEnabled;
            DisplayName = displayName;
        }
        public bool Enabled { get; set; }
        public string DisplayName { get; }

        protected void AddOption(string id, string displayName, bool defaultEnabled = true)
        {
            OptionDict.Add(id, new RandomizerOption(id, displayName, defaultEnabled));
        }

        protected IDictionary<string, RandomizerOption> OptionDict = new Dictionary<string, RandomizerOption>();

        public IEnumerable<RandomizerOption> Options => OptionDict.Values;

        public abstract void Apply(IDataService service);
    }
}

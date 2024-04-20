using RanseiLink.Core.Services;
using System.Collections.ObjectModel;

namespace RanseiLink.GuiCore.ViewModels;

public class ModMetadataViewModelBase : ViewModelBase
{
    public ModMetadataViewModelBase(ModMetadata metadata, List<string> knownTags)
    {
        Metadata = metadata;
        KnownTags = new(knownTags.Select(x => new FilterableTag(x) { Checked = metadata.Tags.Contains(x)}));
        AddTagCommand = new RelayCommand(AddTag);
    }

    public ModMetadata Metadata { get; }

    public void OnClosing()
    {
        Metadata.Tags = KnownTags.Where(x => x.Checked).Select(x => x.Tag).ToList();
    }

    public string? Name
    {
        get => Metadata.Name;
        set => Set(Metadata.Name, value, v => Metadata.Name = v);
    }

    public string? Author
    {
        get => Metadata.Author;
        set => Set(Metadata.Author, value, v => Metadata.Author = v);
    }

    public string? Version
    {
        get => Metadata.Version;
        set => Set(Metadata.Version, value, v => Metadata.Version = v);
    }

    private string _tagToAdd = "";
    public string TagToAdd
    {
        get => _tagToAdd;
        set => Set(ref _tagToAdd, value);
    }

    public ICommand AddTagCommand { get; }

    private void AddTag()
    {
        if (string.IsNullOrWhiteSpace(TagToAdd))
        {
            return;
        }

        // first try to find existing tag
        foreach (var tag in KnownTags)
        {
            if (TagToAdd.Equals(tag))
            {
                tag.Checked = true;
                TagToAdd = string.Empty;
                return;
            }
        }

        // not found existing, so add new one
        KnownTags.Add(new(TagToAdd) { Checked = true });

        TagToAdd = string.Empty;
    }

    public ObservableCollection<FilterableTag> KnownTags { get; }
}

using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Rotary.Core.Collections;
using Rotary.Core.Collections.Records;

namespace Rotary.App.ViewModels;

public enum CollectionTreeNodeKind
{
    Collection,
    Folder,
    Request,
}

public partial class CollectionTreeNodeViewModel : ObservableObject
{
    private readonly ICollectionService? _collectionService;
    private bool _childrenLoaded;

    public Guid Id { get; }
    public string Name { get; }
    public CollectionTreeNodeKind Kind { get; }
    public ObservableCollection<CollectionTreeNodeViewModel> Children { get; } = [];

    [ObservableProperty]
    public partial bool IsExpanded { get; set; }

    // A collection root: only Id/Name are known up front (from the index), the rest of the
    // tree is fetched lazily the first time this node is expanded.
    public CollectionTreeNodeViewModel(
        CollectionIndexEntryDefinition entry,
        ICollectionService collectionService
    )
    {
        Id = entry.Id;
        Name = entry.Name;
        Kind = CollectionTreeNodeKind.Collection;
        _collectionService = collectionService;
    }

    // A folder/request already has its whole subtree in memory once the owning collection has
    // been loaded, so there's nothing further to fetch.
    public CollectionTreeNodeViewModel(CollectionNodeDefinition node)
    {
        Id = node.Id;
        Name = node.Name;
        _childrenLoaded = true;

        switch (node)
        {
            case CollectionNodeDefinition.Folder folder:
                Kind = CollectionTreeNodeKind.Folder;
                foreach (var child in folder.Items)
                {
                    Children.Add(new CollectionTreeNodeViewModel(child));
                }
                break;
            case CollectionNodeDefinition.Request:
                Kind = CollectionTreeNodeKind.Request;
                break;
        }
    }

    partial void OnIsExpandedChanged(bool value)
    {
        if (value && !_childrenLoaded && Kind == CollectionTreeNodeKind.Collection)
        {
            _childrenLoaded = true;
            _ = LoadChildrenAsync();
        }
    }

    private async Task LoadChildrenAsync()
    {
        if (_collectionService is null)
        {
            return;
        }

        var collection = await _collectionService.GetCollectionAsync(Id);
        if (collection is null)
        {
            return;
        }

        foreach (var item in collection.Items)
        {
            Children.Add(new CollectionTreeNodeViewModel(item));
        }
    }
}

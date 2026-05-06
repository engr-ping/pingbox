using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PingBox.ViewModels;

public class SearchResultViewModel
{
    public SearchResultViewModel(string pageName, IEnumerable<PageItemViewModel> items)
    {
        PageName = pageName;
        Items = new ObservableCollection<PageItemViewModel>(items);
    }

    public string PageName { get; }
    public ObservableCollection<PageItemViewModel> Items { get; }
}

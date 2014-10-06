using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace IndexerWpf.ViewModels
{
    public enum SearchBoxState
    {
        TooMuchResults,
        NoResults,
        Ok
    }

    public class SearchBox<TItem> : BaseViewModel
    {
        public delegate void SearchBoxItemSelected(SearchBox<TItem> sender, TItem item);

        public Func<IEnumerable<TItem>> SearchFunc { get; set; }

        public SearchBoxState State { get; set; }

        public int NumberOfItemsLimit { get; set; }

        public ObservableCollection<TItem> Items { get; private set; }

        public TItem SelectedItem { get; set; }

        public event SearchBoxItemSelected Selected;

        public SearchBox()
        {
            State = SearchBoxState.NoResults;
            Items = new ObservableCollection<TItem>();

            this.PropertyChanged += (sender, arg) =>
            {
                if ("SelectedItem".Equals(arg.PropertyName))
                    OnSelected();
            };
        }

        public void Update()
        {
            var searchResult = SearchFunc().ToArray();

            var searchResultCount = searchResult.Length;

            if (searchResultCount == 0)
            {
                State = SearchBoxState.NoResults;
                Items.Clear();
                return;
            }
            if (searchResultCount > NumberOfItemsLimit)
            {
                State = SearchBoxState.TooMuchResults;
                Items.Clear();
                return;
            }

            State = SearchBoxState.Ok;

            var removeList = Items.Except(searchResult).ToArray();
            var newList = searchResult.Except(Items).ToArray();

            foreach (var item in removeList)
                Items.Remove(item);

            foreach (var item in newList)
                Items.Add(item);
        }

        void OnSelected()
        {
            var handler = Selected;
            if (handler != null)
                handler(this, SelectedItem);
        }
    }
}

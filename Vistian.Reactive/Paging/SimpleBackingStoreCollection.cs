using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using Vistian.Contract;

namespace Vistian.Reactive.Paging
{
    /// <summary>
    /// A simple implementation of a paged backing store.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class SimpleBackingStoreCollection<TItem> : IPagedBackingStoreCollection<TItem>, IDisposable
    {
        private readonly IChangeSetPagedDataProvider<TItem> _changeSetProvider;

        /// <summary>
        /// Our collection of read items. 
        /// </summary>
        private readonly ObservableCollectionExtended<TItem> _backingCollection;

        private readonly SingleAssignmentDisposable _subscriptionDisposable = new SingleAssignmentDisposable();

        /// <summary>
        /// Does the collection have an entry for a specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool Contains(int index)
        {
            return _backingCollection.Count > index;
        }

        /// <summary>
        /// Get the item for a specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TItem Get(int index)
        {
            return _backingCollection[index];
        }

        /// <summary>
        /// Get the count of the total number of items in the backing store.
        /// </summary>
        public int Count => _backingCollection.Count;

        /// <summary>
        /// Get total number of items available in the backing store.
        /// </summary>
        public int? Total => _changeSetProvider.Total;

        /// <summary>
        /// Get the change set provider
        /// </summary>
        public IChangeSetPagedDataProvider<TItem> ChangeSetProvider => _changeSetProvider;

        public IList<TItem> Items => _backingCollection;

        /// <summary>
        /// Create a collection 
        /// </summary>
        /// <param name="changeSetProvider"></param>
        /// <param name="scheduler">The scheduler on which data set changes are to be observed</param>
        public SimpleBackingStoreCollection(IChangeSetPagedDataProvider<TItem> changeSetProvider, IScheduler scheduler = null)
        {
            Guard.NotNull(changeSetProvider);

            // record the change set provider
            _changeSetProvider = changeSetProvider;

            // bind to the backing collection
            _backingCollection = new ObservableCollectionExtended<TItem>();

            _subscriptionDisposable.Disposable = changeSetProvider.Changes.
                ObserveOn(scheduler ?? RxApp.MainThreadScheduler).
                Bind(_backingCollection).
                Subscribe();
        }

        public void Dispose()
        {
            _subscriptionDisposable?.Dispose();
        }
    }
}

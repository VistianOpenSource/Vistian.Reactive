using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Threading.Tasks;
using DynamicData;
using ReactiveUI;
using Vistian.Contract;
using Vistian.Reactive.Paging.ChangeSetProviders;

namespace Vistian.Reactive.Paging
{
    /// <summary>
    /// Wrapper class using a provided backing store and a mechanism to allow for reporting of touched items.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class ReadOnlyBackingStoreCollection<TItem> : IList<TItem>, INotifyCollectionChanged where TItem : class
    {
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private readonly IPagedBackingStoreCollection<TItem> _backingStoreCollection;

        public IList<TItem> Items => _backingStoreCollection.Items;

        private readonly SingleAssignmentDisposable _changeSetDisposable = new SingleAssignmentDisposable();
        /// <summary>
        /// Create a paged collection which wrappers a backing store.
        /// </summary>
        /// <param name="pagedBackingStoreCollection"></param>
        public ReadOnlyBackingStoreCollection(IPagedBackingStoreCollection<TItem> pagedBackingStoreCollection)
        {
            Guard.NotNull(pagedBackingStoreCollection);

            var subscription = pagedBackingStoreCollection.
                ChangeSetProvider.
                Changes.
                Subscribe(changes =>
                {
                    // need to raise the appropriate collection changed things...
                    // as changes an observable can provide...
                    foreach (var change in changes)
                    {
                        switch (change.Reason)
                        {
                            case ListChangeReason.Add:

                                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, change.Item.Current, change.Item.CurrentIndex));
                                break;

                            case ListChangeReason.AddRange:

                                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, change.Range));
                                break;

                            case ListChangeReason.Remove:

                                if (change.Range != null)
                                {
                                    CollectionChanged?.Invoke(this,
                                        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                                            change.Range, change.Range.Index));
                                }
                                else
                                {
                                    CollectionChanged?.Invoke(this,
                                        new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove,
                                            change.Item.Current, change.Item.CurrentIndex));
                                }
                                break;

                            case ListChangeReason.Replace:
                                {
                                    var @event = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, change.Item.Current, change.Item.Previous.Value, change.Item.CurrentIndex);

                                    CollectionChanged?.Invoke(this, @event);

                                    break;
                                }
                            case ListChangeReason.Clear:
                                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                                break;

                            case ListChangeReason.Moved:
                                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, change.Item.Current, change.Item.CurrentIndex, change.Item.PreviousIndex));
                                break;
                        }
                    }
                });

            _changeSetDisposable.Disposable = subscription;

            _backingStoreCollection = pagedBackingStoreCollection;
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            for (var i = 0; i < this.Count; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(TItem item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(TItem item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(TItem[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(TItem item)
        {
            throw new NotImplementedException();
        }

        public int Count => GetCount();

        protected virtual int GetCount()
        {
            return _backingStoreCollection.Count;
        }

        public bool IsReadOnly => true;

        public int IndexOf(TItem item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, TItem item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public virtual TItem this[int index]
        {
            get
            {
                if (!_backingStoreCollection.Contains(index))
                {
                    return CreateDefault(index);
                }
                else
                {
                    return _backingStoreCollection.Get(index);
                }
            }
            set
            {
                throw new NotImplementedException();
            }
        }


        protected virtual TItem CreateDefault(int index)
        {
            return default(TItem);
        }

    }


    public static class ReadOnlyBackingStoreCollection
    {
        /// <summary>
        /// Create a collection backed from a <see cref="SourceCache{TObject,TKey}"/>
        /// </summary>
        /// <typeparam name="T">The type stored in the collection</typeparam>
        /// <typeparam name="TKey">The key type</typeparam>
        /// <param name="readPageObservable">The observable which provides the pages of content</param>
        /// <param name="keySelector">The key selector</param>
        /// <param name="maxPageSize">The maximum size of page to be read, the default being 10</param>
        /// <param name="onException">The observable to be used when errors on paging are seen. Null will result in the default behavior, which is fail.</param>
        /// <param name="collectionTouchedStrategy">The strategy to be used when items in a collection are touched. If null <see cref="CollectionItemTouchedStrategy{TItem}"/> is used.</param>
        /// <param name="backingStoreCollection">The store to be used to back the collection. If null <see cref="SimpleBackingStoreCollection{TItem}" /> is used</param>
        /// <returns></returns>
        public static ReadOnlyBackingStoreCollection<T> FromSourceCachePager<T, TKey>(Func<PageReadRequest, IObservable<PageReadResult<T>>> readPageObservable,
                                                                    Func<T, TKey> keySelector,
                                                                    int maxPageSize = 10,
                                                                    Func<PageReadRequest, Exception, IObservable<bool>> onException = null,
                                                                    ICollectionItemTouchedStrategy collectionTouchedStrategy = null,
                                                                    IPagedBackingStoreCollection<T> backingStoreCollection = null) where T : class
        {
            var provider = SourceCacheBacked<T, TKey>.FromObservable(readPageObservable, keySelector, maxPageSize, onException);

            return Create(provider, collectionTouchedStrategy);
        }


        /// <summary>
        /// Create a collection backed from a <see cref="ReactiveList{T}"/> 
        /// </summary>
        /// <typeparam name="TItem">The type of item stored.</typeparam>
        /// <typeparam name="TKey">The key type</typeparam>
        /// <param name="readPageObservable"></param>
        /// <param name="keySelector"></param>
        /// <param name="maxPageSize"></param>
        /// <param name="onException"></param>
        /// <param name="collectionTouchedStrategy"></param>
        /// <returns></returns>
        /// <remarks>Uses a <see cref="KeyedReactiveListBacked{TItem,TKey}"/> using the provided onException and maxPageSize values.</remarks>
        public static ReadOnlyBackingStoreCollection<TItem> FromKeyedReactiveListPager<TItem, TKey>(Func<PageReadRequest, IObservable<PageReadResult<TItem>>> readPageObservable, Func<TItem, TKey> keySelector, int maxPageSize = 10, Func<PageReadRequest, Exception, IObservable<bool>> onException = null,
                                                                    ICollectionItemTouchedStrategy collectionTouchedStrategy = null)
                                                                    where TItem : class
        {

            // create the provider of the changesets
            var provider = KeyedReactiveListBacked.FromObservable(readPageObservable, keySelector, maxPageSize, onException);

            // this is our list which acts as the backing store for the above stuff...
            return Create(provider, collectionTouchedStrategy);
        }

        /// <summary>
        /// Create an instance from a specified change set provider and a collection touched strategy
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <param name="provider"></param>
        /// <param name="collectionTouchedStrategy"></param>
        /// <returns></returns>
        /// 
        public static ReadOnlyBackingStoreCollection<TItem> Create<TItem>(IChangeSetPagedDataProvider<TItem> provider, ICollectionItemTouchedStrategy collectionTouchedStrategy = null) where TItem : class
        {
            // create a backing store...
            var backingStore = new SimpleBackingStoreCollection<TItem>(provider);

            return Create(backingStore, collectionTouchedStrategy);
        }


        /// <summary>
        /// Create a collection with a specified <see cref="IPagedBackingStoreCollection{TItem}"/> and <see cref="ICollectionItemTouchedStrategy"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="backingStoreCollection"></param>
        /// <param name="collectionTouchedStrategy">If not specified, <see cref="CollectionItemTouchedStrategy{TItem}"/> is used.</param>
        /// <returns></returns>
        public static ReadOnlyBackingStoreCollection<T> Create<T>(IPagedBackingStoreCollection<T> backingStoreCollection, ICollectionItemTouchedStrategy collectionTouchedStrategy = null) where T : class
        {
            // the strategy to request page loads in the backing store

            var touched = collectionTouchedStrategy ?? new CollectionItemTouchedStrategy<T>(backingStoreCollection);

            // the actual list we will bind to...

            var collection = new ReadOnlyStrategyPagedCollection<T>(backingStoreCollection, touched);

            return collection;
        }

    }
}

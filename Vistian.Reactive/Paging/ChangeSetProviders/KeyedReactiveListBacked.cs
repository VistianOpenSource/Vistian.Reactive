﻿using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using DynamicData;
using ReactiveUI;
using Vistian.Reactive.ReactiveUI;

namespace Vistian.Reactive.Paging.ChangeSetProviders
{
    /// <summary>
    /// Paged ChangeSet Provider backed with a <see cref="KeyedReactiveList{TItem,TKey}"/>
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class KeyedReactiveListBacked<TItem, TKey> : BaseChangeSetProvider<TItem>, IChangeSetPagedDataProvider<TItem>
    {
        /// <summary>
        /// The backing list of the items read
        /// </summary>
        private readonly KeyedReactiveList<TItem, TKey> _list;

        /// <summary>
        /// Get the list
        /// </summary>
        public ReactiveList<TItem> Items => _list;

        public KeyedReactiveListBacked(IPagingController<TItem> pagingController, Func<TItem, TKey> keySelector, IScheduler readScheduler = null, IScheduler updateScheduler = null) : base(pagingController, readScheduler, updateScheduler)
        {
            _list = new KeyedReactiveList<TItem, TKey>(keySelector);
        }

        /// <summary>
        /// Create a change set
        /// </summary>
        /// <returns></returns>
        protected override IObservable<IChangeSet<TItem>> CreateChangeSet()
        {
            // NOTE this needs to be replaced with the Dynamic Data code once/if a fix is in place for the 
            // replace changeset issue of the position.
            return _list.ToObservableChangeSetNew();
        }

        /// <summary>
        /// Calculate the read range.
        /// </summary>
        /// <param name="pageReadRequest"></param>
        /// <param name="forceReload"></param>
        /// <param name="alignedStart"></param>
        /// <param name="alignedEnd"></param>
        /// <returns></returns>
        /// <remarks>
        /// Be considerate to forced reloads to ensure the complete data is re-read.</remarks>
        protected override bool GetReadRange(PageReadRequest pageReadRequest, bool forceReload, ref int alignedStart, ref int alignedEnd)
        {
            return this.CalcReadRange(pageReadRequest, forceReload ? 0 : _list.Count, ref alignedStart, ref alignedEnd);
        }

        /// <summary>
        /// Apply retrieved data to the underlying <see cref="KeyedReactiveList{TItem,TKey}"/>
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="items"></param>
        /// <param name="replaceAll"></param>
        protected override void AddUpdate(int offset,List<TItem> items, bool replaceAll = false)
        {
            if (replaceAll)
            {
                _list.SmartUpdate(items);
            }
            else
            {
                var trulyNewContentOffset = 0;

                if (offset < _list.Count)
                {
                    trulyNewContentOffset = _list.Count;

                    var startOffset = offset;
                    // we need to do a replacement for certain items and then do an add range...
                    for (var index = 0; index< _list.Count-startOffset; ++index)
                    {
                        _list[startOffset+index] = items[index];
                    }
                }

                _list.AddRange(items.GetRange(trulyNewContentOffset,offset+items.Count-_list.Count));
            }
        }

        /// <summary>
        /// For a reload of the existing, underlying data.
        /// </summary>
        public IObservable<PageReadResult<TItem>> Reload()
        {
            // cancel all pending reads, this should improve performance
            this.CancelPending();

            // build the request
            var pageRequest = new PageReadRequest() { Offset = 0, Take = _list.Count };

            // and now do the update
            return EnqueueReadObservable(pageRequest, true);
        }
    }

    public static class KeyedReactiveListBacked
    {
        public static KeyedReactiveListBacked<TItem, TKey> FromObservable<TItem, TKey>(Func<PageReadRequest, IObservable<PageReadResult<TItem>>> readChunksObservable, Func<TItem, TKey> keySelector, int maxPageSize)
        {
            return new KeyedReactiveListBacked<TItem, TKey>(new ObservablePagingController<TItem>(readChunksObservable, maxPageSize: maxPageSize), keySelector);
        }

        public static KeyedReactiveListBacked<TItem, TKey> FromObservable<TItem, TKey>(Func<PageReadRequest, IObservable<PageReadResult<TItem>>> readChunksObservable, Func<TItem, TKey> keySelector, int maxPageSize, Func<PageReadRequest, Exception, IObservable<bool>> exceptionObservable)
        {
            return new KeyedReactiveListBacked<TItem, TKey>(new ObservablePagingController<TItem>(readChunksObservable, maxPageSize: maxPageSize, exceptionObservable: exceptionObservable), keySelector);
        }
    }
}
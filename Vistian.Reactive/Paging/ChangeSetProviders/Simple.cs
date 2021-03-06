﻿using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using Vistian.Reactive.ReactiveUI;

namespace Vistian.Reactive.Paging.ChangeSetProviders
{
    /// <summary>
    /// Paged ChangeSet Provider which just passes through changes, the assumption being the underlying paging controller is a completely read only source.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class Simple<TItem> : BaseChangeSetProvider<TItem>, IChangeSetPagedDataProvider<TItem>
    {
        public Simple(IPagingController<TItem> pagingController, IScheduler readScheduler = null, IScheduler updateScheduler = null) : base(pagingController, readScheduler, updateScheduler)
        {
        }

        private int _offset = 0;


        private readonly Subject<IChangeSet<TItem>> _changeSetSubject = new Subject<IChangeSet<TItem>>();
        /// <summary>
        /// Create a change set
        /// </summary>
        /// <returns></returns>
        protected override IObservable<IChangeSet<TItem>> CreateChangeSet()
        {
            return _changeSetSubject.AsObservable();
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
        /// Be considerable to a forced reload to ensure the complete data is re-read.</remarks>
        protected override bool GetReadRange(PageReadRequest pageReadRequest, bool forceReload, ref int alignedStart, ref int alignedEnd)
        {
            return this.CalcReadRange(pageReadRequest, forceReload ? 0 : _offset, ref alignedStart, ref alignedEnd);
        }

        /// <summary>
        /// Apply retrieved data to the underlying <see cref="KeyedReactiveList{TItem,TKey}"/>
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="items"></param>
        /// <param name="replaceAll"></param>
        protected override void AddUpdate(int offset, List<TItem> items, bool replaceAll = false)
        {
            var changes = new List<Change<TItem>>(items.Count);

            if (replaceAll)
            {
                changes.Add(new Change<TItem>(ListChangeReason.Clear, new TItem[] { }, 0));

                _offset = 0;
            }

            if (offset < _offset)
            {
                var startOffset = offset;
                // we need to do a replacement for certain items and then do an add range...
                for (var index = 0; index < _offset - startOffset; ++index)
                {
                    var replace = new Change<TItem>(ListChangeReason.Replace,items[index],items[index],startOffset+index,startOffset+index);

                    changes.Add(replace);
                }
            }

            for(var finalOffset = _offset;finalOffset < offset + items.Count;finalOffset++)
            {
                var addition = new Change<TItem>(ListChangeReason.Add, items[finalOffset - offset],finalOffset);
                changes.Add(addition);
            }

            _changeSetSubject.OnNext(new ChangeSet<TItem>(changes));

            _offset = Math.Max(_offset, offset + items.Count);
        }

        /// <summary>
        /// For a reload of the existing, underlying data.
        /// </summary>
        public void Reload()
        {
            // cancel all pending reads, this should improve performance
            this.CancelPending();

            // build the request
            var pageRequest = new PageReadRequest() { Offset = 0, Take = _offset };

            // and now do the update
            EnqueueReadObservable(pageRequest, true);
        }
    }

    public static class Simple
    {
        public static Simple<TItem> FromObservable<TItem>(Func<PageReadRequest, IObservable<PageReadResult<TItem>>> readChunksObservable, int maxPageSize)
        {
            return new Simple<TItem>(new ObservablePagingController<TItem>(readChunksObservable, maxPageSize: maxPageSize));
        }

        public static Simple<TItem> FromObservable<TItem>(Func<PageReadRequest, IObservable<PageReadResult<TItem>>> readChunksObservable, int maxPageSize, Func<PageReadRequest, Exception, IObservable<bool>> exceptionObservable)
        {
            return new Simple<TItem>(new ObservablePagingController<TItem>(readChunksObservable, maxPageSize: maxPageSize, exceptionObservable: exceptionObservable));
        }
    }

}
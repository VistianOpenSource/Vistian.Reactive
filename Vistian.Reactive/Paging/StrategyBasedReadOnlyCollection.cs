﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vistian.Contract;

namespace Vistian.Reactive.Paging
{
    /// <summary>
    /// Paged Collection extension allowing for strategy implementation around collection items referencing.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class StrategyBasedReadOnlyCollection<TItem> : ReadOnlyCollectionFromBackingStore<TItem> 
    {
        private readonly ICollectionItemTouchedStrategy _touchedStrategy;

        /// <summary>
        /// Create a collection with a null touch strategy.
        /// </summary>
        /// <param name="pagedBackingStoreCollection"></param>
        public StrategyBasedReadOnlyCollection(IPagedBackingStoreCollection<TItem> pagedBackingStoreCollection)
            : this(pagedBackingStoreCollection, NullCollectionItemTouchedStrategy.Instance)
        {            
        }

        public StrategyBasedReadOnlyCollection(IPagedBackingStoreCollection<TItem> pagedBackingStoreCollection,
            ICollectionItemTouchedStrategy touchedStrategy) : base(pagedBackingStoreCollection)
        {
            Guard.NotNull(pagedBackingStoreCollection);
            Guard.NotNull(touchedStrategy);

            _touchedStrategy = touchedStrategy;
        }

        /// <summary>
        /// Record that a particular index has been touched.
        /// </summary>
        /// <param name="index"></param>
        private void Touched(int index)
        {
            _touchedStrategy.IndexReferenced(index);
        }

        /// <summary>
        /// Override the referencing of an item to allow for it to be tracked.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>

        public override TItem this[int index]
        {
            get
            {
                Touched(index);


                return base[index];
            }
            set
            {
                base[index] = value;
            }
        }

        /// <summary>
        /// Track the referencing of the count.
        /// </summary>
        /// <returns></returns>
        protected override int GetCount()
        {
            // record fact we have been touched.
            _touchedStrategy.CountReferenced();

            return base.GetCount();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vistian.Reactive.Paging
{
    /// <summary>
    /// Specification of a Paging based backing store collection.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public interface IPagedBackingStoreCollection<TItem>
    {
        /// <summary>
        /// Determine if the backing store contains an entry for a specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool Contains(int index);

        /// <summary>
        /// Get the item for a specified index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        TItem Get(int index);


        /// <summary>
        /// Get a count of the total number of items present in the backing store.
        /// </summary>
        int Count { get; }


        /// <summary>
        /// Get the potentially total number of items there could be in the backing store.
        /// </summary>
        int? Total { get; }


        /// <summary>
        /// Get the changeset data provider.
        /// </summary>
        IChangeSetPagedDataProvider<TItem> ChangeSetProvider { get; }

        /// <summary>
        /// Get the items present in the backing store.
        /// </summary>
        IList<TItem> Items { get; }
    }
}

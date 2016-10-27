using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vistian.Reactive.Paging
{
    /// <summary>
    /// This is the user provided class for paging.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPagedDataProvider<T>
    {
        IObservable<PageReadResult<T>> ReadPageObservable(PageReadRequest pageReadRequest);

        /// <summary>
        /// The maximum size pages will be read in.
        /// </summary>
        int MaxPageSize { get; }

        /// <summary>
        /// Gets the total available
        /// </summary>
        int? Total { get; }
    }
}

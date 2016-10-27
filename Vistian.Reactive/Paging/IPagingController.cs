using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vistian.Reactive.Paging
{
    /// <summary>
    /// Paging controller specification
    /// </summary>
    public interface IPagingController<TItem>
    {
        /// <summary>
        /// Read a page at a specified offset and size.
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="take"></param>
        /// <returns></returns>
        IObservable<PageReadResult<TItem>> ReadPage(int offset, int take);

        /// <summary>
        /// Get the maximum size pages should be requested in.
        /// </summary>
        int MaxPageSize { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vistian.Reactive.Paging
{
    public static partial class IPagedDataProviderMixins
    {
        /// <summary>
        /// Utility function to calculate a page aligned offset for a <see cref="IPagedDataProvider{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static int PageAlignedOffset<T>(this IPagedDataProvider<T> provider, int offset)
        {
            var chunkSize = provider.MaxPageSize;

            return (int)Math.Floor(((float)offset) / chunkSize) * chunkSize;
        }

        /// <summary>
        /// Create a standard <see cref="PageReadRequest"/> for a specified <see cref="IPagedDataProvider{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="provider"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static PageReadRequest CreateReadRequest<T>(this IPagedDataProvider<T> provider, int offset)
        {
            var alignedOffset = provider.PageAlignedOffset(offset);

            return new PageReadRequest() { Offset = alignedOffset, Take = provider.MaxPageSize };
        }
    }
}

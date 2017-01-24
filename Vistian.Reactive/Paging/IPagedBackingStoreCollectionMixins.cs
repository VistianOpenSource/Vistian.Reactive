using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vistian.Reactive.Paging
{
    public static class IPagedBackingStoreCollectionMixins
    {
        /// <summary>
        /// Return 
        /// </summary>
        /// <typeparam name="TResultItem"></typeparam>
        /// <param name="pagedBackingStoreCollection"></param>
        /// <param name="readOffset"></param>
        /// <returns></returns>
        public static IEnumerable<int> CreateOffsets<TResultItem>(
            this IPagedBackingStoreCollection<TResultItem> pagedBackingStoreCollection, int readOffset)
        {
            var maxPageSize = pagedBackingStoreCollection.ChangeSetProvider.MaxPageSize;

            var offsetsToRead = new HashSet<int>();

            // work out the aligned base that this falls into)
            // we potentially align upwards to force the read of the page following, ensuring a contiguous run.
            var alignedOffset = (maxPageSize == int.MaxValue)
                ? readOffset
                : ((int) Math.Round(((float) readOffset) / maxPageSize)) * maxPageSize;

            // if don't have a total, or we are below the total amount then enqueue the read...
            if (!pagedBackingStoreCollection.Total.HasValue || alignedOffset < pagedBackingStoreCollection.Total)
            {
                offsetsToRead.Add(alignedOffset);
            }

            return offsetsToRead;
        }

        /// <summary>
        /// Create those indexes which are going to be requested.
        /// </summary>
        /// <param name="indexes"></param>
        /// <returns></returns>
        public static IEnumerable<int> CreateOffsets<TResultItem>(
            this IPagedBackingStoreCollection<TResultItem> pagedBackingStoreCollection, IEnumerable<int> indexes)
        {
            var maxPageSize = pagedBackingStoreCollection.ChangeSetProvider.MaxPageSize;

            var offsetsToRead = new HashSet<int>();

            foreach (var index in indexes)
            {
                // work out the aligned base that this falls into
                // we potentially align upwards to force the read of the page following, ensuring a contiguous run.
                var alignedOffset = (maxPageSize == int.MaxValue)
                    ? index
                    : ((int) Math.Round(((float) index) / maxPageSize)) * maxPageSize;

                // if don't have a total, or we are below the total amount then enqueue the read...
                if (!pagedBackingStoreCollection.Total.HasValue || alignedOffset < pagedBackingStoreCollection.Total)
                {
                    offsetsToRead.Add(alignedOffset);
                }
            }

            return offsetsToRead;
        }


        public static IEnumerable<int> CreateOffsetsUpTo<TResultItem>(
            this IPagedBackingStoreCollection<TResultItem> pagedBackingStoreCollection, int offset)
        {
            var maxPageSize = pagedBackingStoreCollection.ChangeSetProvider.MaxPageSize;

            if (maxPageSize == int.MaxValue)
            {
                // at most a single read
                if (!pagedBackingStoreCollection.Total.HasValue || offset < pagedBackingStoreCollection.Total)
                {
                    yield return offset;
                }
                {
                    var alignedOffset = ((int) Math.Round(((float) offset) / maxPageSize)) * maxPageSize;

                    if (!pagedBackingStoreCollection.Total.HasValue || offset < pagedBackingStoreCollection.Total)
                    {
                        while (alignedOffset < offset)
                        {
                            yield return alignedOffset;
                            alignedOffset += maxPageSize;
                        }
                    }
                }
            }
        }
    }
}

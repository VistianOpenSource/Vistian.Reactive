using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vistian.Reactive.Paging
{
    public static class BaseChangeSetProviderMixins
    {
        /// <summary>
        /// Calculate the read range given a specified count of a 'notional' list of existing read information.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="provider"></param>
        /// <param name="pageReadRequest"></param>
        /// <param name="currentCount"></param>
        /// <param name="alignedStart"></param>
        /// <param name="alignedEnd"></param>
        /// <returns></returns>
        public static bool CalcReadRange<TSource>(this BaseChangeSetProvider<TSource> provider, PageReadRequest pageReadRequest, int currentCount, ref int alignedStart, ref int alignedEnd)
        {
            var offset = pageReadRequest.Offset;
            var take = pageReadRequest.Take;
            var extremum = offset + take;

            /*
            var anythingToRead = true;

            if (provider.Total.HasValue)
            {
                anythingToRead = (offset >= currentCount) || (offset + take > currentCount) && offset < provider.Total.Value;
            }

            if (anythingToRead)
            {
                // start from top
                alignedStart = provider.PageAlignedOffset(currentCount);

                // calculate the end
                alignedEnd = Math.Max(alignedStart + take, extremum);

                anythingToRead = alignedStart != alignedEnd;
            }
            */



            var topEnd = int.MaxValue;

            if (provider.Total.HasValue)
            {
                topEnd = provider.Total.Value;
            }

            var isAnythingToRead = offset + take <= topEnd && currentCount < offset+take;

            if (isAnythingToRead)
            {
                alignedStart = currentCount;

                var adjustedExtremum = Math.Min(extremum, topEnd);

                alignedEnd = adjustedExtremum;

                isAnythingToRead = alignedStart != alignedEnd;

            }

            return isAnythingToRead;


            //return anythingToRead;
        }

    }
}

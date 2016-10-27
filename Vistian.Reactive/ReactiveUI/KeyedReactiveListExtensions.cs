using System;
using System.Collections.Generic;
using System.Linq;
using Vistian.Contract;
using Vistian.Reactive.Logging;
using Vistian.Reactive.Logging.Providers;

namespace Vistian.Reactive.ReactiveUI
{
    public static class KeyedReactiveListExtensions
    {
        /// <summary>
        /// Updates an existing <see cref="KeyedReactiveList{TItem,TKey}"/> smartly with a new set of items.
        /// </summary>
        /// <typeparam name="TItem"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="destination"></param>
        /// <param name="source"></param>
        /// <remarks>
        /// Instead of a block replacement occuring, the <see cref="TKey"/> is used to determine
        /// items which have either been removed, moved or added and the destination list is updated in this once.</remarks>
        public static void SmartUpdate<TItem, TKey>(this KeyedReactiveList<TItem, TKey> destination,
            IReadOnlyList<TItem> source)
        {
            var index = 0;

            Guard.NotNull(destination);
            Guard.NotNull(source);

            try
            {
                // 
                // construct the final set of keys
                var finalKeys = source.ToLookup(destination.KeyFor);

                // get the list of removals
                var removals = destination.Where(d => !finalKeys.Contains(destination.KeyFor(d))).ToList();

                foreach (var removal in removals)
                {
                    destination.Remove(removal);
                }

                // so now, we are just into additions and updates
                foreach (var newItem in source)
                {
                    TItem currentItem;

                    if (destination.GetForKey(destination.KeyFor(newItem), out currentItem))
                    {
                        var currentIndex = destination.IndexOf(currentItem);

                        if (currentIndex != index)
                        {
                            destination.Move(currentIndex, index);
                        }
                        else
                        {
                            destination[currentIndex] = newItem;
                        }
                    }
                    else
                    {
                        destination.Insert(index, newItem);
                    }
                    ++index;
                }
            }
            catch (Exception ex)
            {
                // log the exception, and re-throw
                RxLog.Log(destination,Classified.Error(ex));
                throw;
            }
        }

    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;

namespace Vistian.Reactive.ReactiveUI
{
    /// <summary>
    /// Simple wrapper around a reactive list which, provides for keyed items, allowing for "smarter' updates of items.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    /// <typeparam name="TKey"></typeparam>
    public class KeyedReactiveList<TItem, TKey> : ReactiveList<TItem>
    {
        private readonly Dictionary<TKey, TItem> _keyedLookup = new Dictionary<TKey, TItem>();

        private readonly Func<TItem, TKey> _keySelector;

        public KeyedReactiveList(Func<TItem, TKey> keySelector)
        {
            _keySelector = keySelector;
        }

        public TKey KeyFor(TItem item)
        {
            return _keySelector(item);
        }

        public override void AddRange(IEnumerable<TItem> collection)
        {
            base.AddRange(collection);

            foreach (var item in collection)
            {
                _keyedLookup[KeyFor(item)] = item;
            }

        }

        public override void Insert(int index, TItem newItem)
        {
            this._keyedLookup[KeyFor(newItem)] = newItem;
            base.Insert(index, newItem);
        }

        public override void RemoveAt(int index)
        {
            // get key of item at specified position and bin off
            var key = KeyFor(base[index]);

            _keyedLookup.Remove(key);

            base.RemoveAt(index);
        }

        public override TItem this[int index]
        {
            get
            {
                return base[index];
            }
            set
            {
                var current = base[index];

                _keyedLookup.Remove(KeyFor(current));

                _keyedLookup[KeyFor(value)] = value;
                base[index] = value;
            }
        }

        public override bool Remove(TItem item)
        {
            _keyedLookup.Remove(KeyFor(item));
            return base.Remove(item);
        }

        public override void RemoveRange(int index, int count)
        {
            var keys =
                this.Skip(index)
                    .Take(count)
                    .Select(KeyFor)
                    .ToList();

            // now we have the keys, remove from the dictionary...
            foreach (var key in keys)
            {
                _keyedLookup.Remove(key);
            }

            base.RemoveRange(index, count);
        }

        public bool GetForKey(TKey key, out TItem item)
        {
            return (_keyedLookup.TryGetValue(key, out item));
        }

        public override void Clear()
        {
            _keyedLookup.Clear();
            base.Clear();
        }
    }
}

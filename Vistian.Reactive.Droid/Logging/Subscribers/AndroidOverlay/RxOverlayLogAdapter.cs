using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

// ReSharper disable once CheckNamespace
namespace Vistian.Reactive.Droid.Logging
{
    /// <summary>
    /// Simple adapter for reactive log items.
    /// </summary>
    public class RxOverlayLogAdapter : BaseAdapter
    {
        private readonly List<OverlayListEntry> _items;


        /// <summary>
        /// Create a simplistic list backed adapter.
        /// </summary>
        /// <param name="logSizeLimit"></param>
        public RxOverlayLogAdapter(int logSizeLimit = int.MaxValue)
        {
            LogSizeLimit = logSizeLimit;
            _items = new List<OverlayListEntry>(logSizeLimit < int.MaxValue ? LogSizeLimit : 0);
        }

        public int LogSizeLimit { get; set; }


        public void Add(OverlayListEntry entry)
        {
            _items.Add(entry);

            if (_items.Count > LogSizeLimit)
            {
                RemoveAt(0);
            }
            this.NotifyDataSetChanged();
        }


        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);

            this.NotifyDataSetChanged();
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return _items.Count;
        }

        public override long GetItemId(int position)
        {
            return _items[position].Index;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var v = convertView ?? LayoutInflater.FromContext(parent.Context).Inflate((int)Resource.Layout.overlayLogItem, parent, false);

            var tv = v.FindViewById<TextView>(Resource.Id.logItemTextLabel);

            tv.Text = _items[position].Text;

            return v;
        }

        public override int Count => _items.Count;

        public IReadOnlyList<OverlayListEntry> Items => _items;

        public void Clear()
        {
            _items.Clear();
            this.NotifyDataSetChanged();
        }
    }
}
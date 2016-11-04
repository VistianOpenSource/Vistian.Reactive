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
    /// Represents an item in the heads up diagnostic view
    /// </summary>
    public class OverlayListEntry
    {
        public OverlayListEntry(int index, string text)
        {
            Index = index;
            Text = text;
        }
        public int Index { get; }
        public string Text { get; }
    }
}
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
    /// Helpers to show and hide the overlay
    /// </summary>
    public static class Overlay
    {
        private static ConsoleSubscriber _subscriber;

        public static ConsoleSubscriber DiagnosticsSubscriber => _subscriber;

        /// <summary>
        /// Show the console, with an optional limit on the list size
        /// </summary>
        /// <param name="context"></param>
        /// <param name="consoleSubscriber"></param>
        public static void Show(Context context, ConsoleSubscriber consoleSubscriber)
        {
            _subscriber = consoleSubscriber;

            var serviceIntent = new Intent(context, typeof(OverlayService));

            context.StartService(serviceIntent);
        }

        /// <summary>
        /// Stop any running console service.
        /// </summary>
        /// <param name="context"></param>
        public static void Hide(Context context)
        {
            var serviceIntent = new Intent(context, typeof(OverlayService));
            context.StopService(serviceIntent);
        }
    }
}
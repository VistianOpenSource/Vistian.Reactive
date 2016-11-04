using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Vistian.Reactive.Logging;
using Vistian.Reactive.Logging.Configuration;

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

    public class ConsoleSubscriber
    {
        public int BufferSize { get; }
        public ReplaySubject<string> Entries { get; }

        public const int DefaultBufferSize = 400;

        public ConsoleSubscriber(int bufferSize = DefaultBufferSize)
        {
            BufferSize = bufferSize;
            Entries = new ReplaySubject<string>(bufferSize);
        }

        public Func<RxLoggerConfiguration, IObservable<RxLogEntry>, IObservable<Unit>> Subscriber()
        {
            return (c, o) => o.Where(x => c.Formatting.Resolver.HasFormatter(x)).
                Select(r => c.Formatting.Resolver.GetFor(r).Formatted(c, r)).

                Do(f => Entries.OnNext(f)).
                Select(_ => Unit.Default);
        }
    }
}
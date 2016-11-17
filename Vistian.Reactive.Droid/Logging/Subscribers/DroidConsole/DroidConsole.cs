using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Vistian.Reactive.Logging;
using Vistian.Reactive.Logging.Configuration;
using Vistian.Reactive.Logging.Providers;

namespace Vistian.Reactive.Droid.Logging.Subscribers
{
    /// <summary>
    /// Simple debug output log subscriber
    /// </summary>
    public class AndroidConsole
    {
        /// <summary>
        /// Create a default Android Console subscriber that emits any log item which has a formatter
        /// </summary>
        public static Func<RxLoggerConfiguration, IObservable<RxLogEntry>, IObservable<Unit>> LogAll
        {
            get
            {
                return (c, o) => o.Where(x => c.Formatting.Resolver.HasFormatter(x)).
                    Select(r => c.Formatting.Resolver.GetFor(r).Formatted(c, r)).
                    Do(Console.WriteLine).
                    Select(_ => Unit.Default);
            }
        }

        /// <summary>
        /// Create a default AndroidConsole Subscriber that only emits <see cref="Classified"/> entries of specified <see cref="LogLevel"/>
        /// </summary>
        /// <param name="levels"></param>
        /// <returns></returns>
        public static Func<RxLoggerConfiguration, IObservable<RxLogEntry>, IObservable<Unit>> Classified(LogLevel levels)
        {
            return (c, o) => o.Where(x => x.InstanceOfType<Classified>()).
                Where(e => ((e.Specifics as Classified).Level & levels) != LogLevel.None).
                Select(r => c.Formatting.Resolver.GetFor(r).Formatted(c, r)).
                Do(Console.WriteLine).
                Select(_ => Unit.Default);
        }
    }
}
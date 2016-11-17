using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Vistian.Reactive.Logging;
using Vistian.Reactive.Logging.Configuration;

namespace Vistian.Reactive.Droid.Logging
{
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
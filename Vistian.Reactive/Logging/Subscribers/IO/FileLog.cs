using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Text;
using System.Threading.Tasks;
using Vistian.Reactive.Logging.Configuration;

namespace Vistian.Reactive.Logging.Subscribers.IO
{
    /// <summary>
    /// Helper class to facilitate the writing of logs to files.
    /// </summary>
    public class FileLog
    {
        private readonly LogFileManager _fileManager;

        /// <summary>
        /// Create an instance using a specified <see cref="LogFileManager"/>
        /// </summary>
        /// <param name="fileManager"></param>
        public FileLog(LogFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        /// <summary>
        /// Write an observable sequence, a line at a time to the current file manager.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <returns></returns>
        private IObservable<Unit> WriteObservable<TSource>(IObservable<string> source)
        {
            return source.Select(l => Observable.Defer(() => this.ProcessEntryAsync(l).ToObservable())).
                        Concat().
                        Select(_ => Unit.Default);
        }

        /// <summary>
        /// Process a single entry.
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public async Task<bool> ProcessEntryAsync(string entry)
        {
            try
            {
                await _fileManager.WriteLogEntryAsync(entry);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ProcessEntryAsync error {0}:{1}", ex.GetType().Name, ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Write an observable sequence of text to a specified log file.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="fileManager"></param>
        /// <returns></returns>
        public static IObservable<Unit> Write(IObservable<string> source, LogFileManager fileManager)
        {
            return Observable.Create<Unit>((o) =>
            {
                var f = new FileLog(fileManager);

                // we need to initialize and then start writing...

                var sub = f.WriteObservable<string>(source).
                            Do(_ => o.OnNext(Unit.Default),
                            o.OnError,
                            o.OnCompleted).
                            Subscribe();

                return sub;
            });
        }

        /// <summary>
        /// Create a default setup of once a day log files.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static Func<RxLoggerConfiguration, IObservable<RxLogEntry>, IObservable<Unit>> All(FileLogOptions options)

        {
            return (c, o) => o.Where(x => c.Formatting.HasFormatter(x)).
                Select(r => c.Formatting.FormatterFor(r).Formatted(c, r)).
                Write(new LogFileManager(options));
        }
    }
}

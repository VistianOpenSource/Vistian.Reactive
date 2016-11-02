using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Vistian.Contract;
using Vistian.Reactive.Logging.Providers;

namespace Vistian.Reactive.Logging
{
    public static class ObservableExtensions
    {
        /// <summary>
        /// Observbale classifed logging extension for observables.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="name"></param>
        /// <param name="callerMemberName"></param>
        /// <param name="lineNo"></param>
        /// <returns></returns>
        /// 
        [DebuggerStepThrough]
        public static IObservable<TSource> Trace<TSource>(this IObservable<TSource> source,
            string name = default(string), [CallerMemberName] string callerMemberName = "",
            [CallerLineNumber] int lineNo = 0)
        {
            Guard.NotNull(source);
            // counter used for our 'tick' occurance 
            var id = 0;

            return Observable.Create<TSource>(observer =>
            {
                var id1 = ++id;

                var meta = new RxLogEntryMeta(typeof(ObservableExtensions), callerMemberName, lineNo);

                Action<string, object> trace = (m, v) =>
                {
                    try
                    {

                        RxLog.Log(meta, Classified.Information($"{name} {id1}: {m}({v})"));
                    }
                    catch (Exception)
                    {
                        // do nothing
                    }
                };

                trace("Subscribe", string.Empty);

                var subscription = source.Subscribe(v =>
                    {
                        trace("OnNext", v);
                        observer.OnNext(v);
                    },
                    e =>
                    {
                        trace("OnError", e.Message);
                        observer.OnError(e);
                    },
                    () =>
                    {
                        trace("OnCompleted", "");
                        observer.OnCompleted();
                    });


                return new CompositeDisposable(subscription, Disposable.Create(() => trace("Dispose", string.Empty)));
            });
        }

        /// <summary>
        /// Log a specified instance of log data for each OnNext value seen.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="host"></param>
        /// <param name="loggedInstance"></param>
        /// <param name="callerMemberName"></param>
        /// <param name="lineNo"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static IObservable<TSource> Log<TSource, T>(this IObservable<TSource> source, object host,
            Func<TSource, T> loggedInstance,
            [CallerMemberName] string callerMemberName = "", [CallerLineNumber] int lineNo = 0)
        {
            Guard.NotNull(source);

            return Observable.Create<TSource>(observer =>
            {
                var meta = new RxLogEntryMeta(host.GetType(), callerMemberName, lineNo);

                var subscription = source.Subscribe(v =>
                    {
                        var i = loggedInstance(v);

                        try
                        {
                            RxLog.Log(meta, i);
                        }
                        catch (Exception ex)
                        {
                            
                            throw;
                        }


                    observer.OnNext(v);
                },
                    observer.OnError,
                    observer.OnCompleted);


                return new CompositeDisposable(subscription);
            });
        }

        /// <summary>
        /// If debugger is attached, break upon specified matching <see cref="NotificationKind"/> values.
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="breakOnSubscribe"></param>
        /// <param name="breakOnDispose"></param>
        /// <param name="breakOnValue"></param>
        /// <param name="breakOnNext"></param>
        /// <param name="breakOnCompleted"></param>
        /// <param name="breakOnError"></param>
        /// <returns></returns>
        [DebuggerStepThrough]
        public static IObservable<TSource> DebugBreak<TSource>(this IObservable<TSource> source,
                                                                bool breakOnSubscribe = false,
                                                                bool breakOnDispose = false,
                                                                Func<TSource, bool> breakOnValue = null,
                                                                bool breakOnNext = false,
                                                                bool breakOnCompleted = false,
                                                                bool breakOnError = false)
        {
            Guard.NotNull(source);

            breakOnValue = breakOnValue ?? ((v) => false);

            return Observable.Create<TSource>(observer =>
            {
                BreakOnCondition(() => breakOnSubscribe == true);

                var subscription = source.Subscribe(v =>
                {
                    BreakOnCondition(() => breakOnNext == true || breakOnValue(v));
                    observer.OnNext(v);
                },
                    e =>
                    {
                        BreakOnCondition(() => breakOnError == true);
                        observer.OnError(e);
                    },
                    () =>
                    {
                        BreakOnCondition(() => breakOnCompleted == true);
                        observer.OnCompleted();
                    });

                return new CompositeDisposable(subscription, Disposable.Create(() => BreakOnCondition(() => breakOnDispose == true)));
            });
        }


        /// <summary>
        /// Utility function to break when a condition matches.
        /// </summary>
        /// <param name="condition"></param>
        [DebuggerStepThrough]
        [Conditional("DEBUG")]
        private static void BreakOnCondition(Func<bool> condition)
        {
            if (condition() && Debugger.IsAttached)
            {
                Debugger.Break();
            }
        }
    }
}

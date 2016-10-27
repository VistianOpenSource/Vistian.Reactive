using System.Runtime.CompilerServices;
using Vistian.Reactive.Logging;

namespace Vistian.Reactive.Metrics.Logging
{
    public static class TimerContextMixins
    {
        /// <summary>
        /// Extensions to log a <see cref="TimerContext"/> upon disposal.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ITimerContext LogOnDispose(this ITimerContext context)
        {
            return new LoggingTimerContext(context);
        }

        public static ITimerContext LogOnDispose(this ITimerContext context, object host, [CallerMemberName] string callerMemberName = "", [CallerLineNumber] int lineNo = 0)
        {
            var meta = new RxLogEntryMeta(host.GetType(), callerMemberName, lineNo);

            return new LoggingTimerContext(context, meta);
        }

    }
}

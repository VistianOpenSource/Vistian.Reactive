using System.Runtime.CompilerServices;

namespace Vistian.Reactive.Logging
{
    public static class RxLogEntryCoreMixins
    {
        public static RxLogEntryMeta Create(this object logPublisher, [CallerMemberName] string callerMemberName = "", [CallerLineNumber] int lineNo = 0)
        {
            return new RxLogEntryMeta(logPublisher?.GetType(), callerMemberName, lineNo);
        }
    }
}
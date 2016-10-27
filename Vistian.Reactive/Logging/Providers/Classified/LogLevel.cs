using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vistian.Reactive.Logging.Providers
{
    /// <summary>
    /// Enum used to specify the 'level' a <see cref="Classified"/> message is
    /// </summary>
    [Flags]
    public enum LogLevel
    {
        Debug = 0x1,
        Information = 0x2,
        Warning = 0x3,
        Error = 0x4,
        Fatal = 0x5,
        All = Debug | Information | Warning | Error | Fatal,
        None = 0x0

    };
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vistian.Reactive.Logging.Configuration;

namespace Vistian.Reactive.Logging
{
    public static class RxLoggerConfigurationExtensions
    {
        /// <summary>
        /// Create the default log host using the specified <see cref="RxLoggerConfiguration"/>
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static void SetDefault(this RxLoggerConfiguration configuration)
        {
            var host = configuration.CreateHost();

            var log = new RxLog(host);

            RxLog.SetDefault(log);
        }
    }
}

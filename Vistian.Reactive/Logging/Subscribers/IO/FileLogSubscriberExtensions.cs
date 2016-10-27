using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vistian.Reactive.Logging.Subscribers.IO
{
    /// <summary>
    /// Extensions to make life easier for file logging options
    /// </summary>
    public static class FileLogSubscriberOptionsExtensions
    {
        /// <summary>
        /// Indicate that the file name changes daily
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static FileLogOptions DailyFile(this FileLogOptions options)
        {
            options.FileName = () =>
            {
                var now = DateTime.UtcNow;

                return $"{now.Year}-{now.Month}-{now.Day}";
            };

            return options;
        }

        /// <summary>
        /// Indicate that we are a per instance file
        /// </summary>
        /// <param name="options"></param>
        public static FileLogOptions PerInstanceFile(this FileLogOptions options)
        {
            options.FileName = PerInstanceFileName;
            return options;
        }

        /// <summary>
        /// Value populated once per start of the application.
        /// </summary>
        private static Guid _fileName = Guid.NewGuid();

        /// <summary>
        /// The per instance filename
        /// </summary>
        /// <returns></returns>
        private static string PerInstanceFileName()
        {
            return _fileName.ToString();
        }
    }
}

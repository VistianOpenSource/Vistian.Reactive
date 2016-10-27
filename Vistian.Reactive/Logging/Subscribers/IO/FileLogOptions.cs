using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vistian.Reactive.Logging.Subscribers.IO
{
    /// <summary>
    /// Options for file based logging.
    /// </summary>
    public class FileLogOptions
    {
        /// <summary>
        /// The log folder name
        /// </summary>
        private const string LogFolder = "Logs";


        /// <summary>
        /// Get or set the function used to create a filename
        /// </summary>
        public Func<string> FileName { get; set; }

        /// <summary>
        /// Get or set whether any existing log file should be cleared upon startup.
        /// </summary>
        public bool ClearAtStart { get; set; }

        /// <summary>
        /// Get the folder into which the logs are placed
        /// </summary>
        public string Folder { get; set; } = LogFolder;

        /// <summary>
        /// Create a default log setting.
        /// </summary>
        public static FileLogOptions OnePerDay
        {
            get
            {
                var options = new FileLogOptions();
                options.DailyFile();
                options.ClearAtStart = false;

                return options;
            }
        }
    }
}

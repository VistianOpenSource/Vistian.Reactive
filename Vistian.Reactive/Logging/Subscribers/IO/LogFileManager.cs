using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;

namespace Vistian.Reactive.Logging.Subscribers.IO
{
    /// <summary>
    /// Provides overall storage control for logging files
    /// </summary>
    public class LogFileManager
    {
        /// <summary>
        /// The folder containing the logs
        /// </summary>
        private IFolder _logFolder;

        /// <summary>
        /// Options indicating how the log files are named, constructed.
        /// </summary>
        private readonly FileLogOptions _options;

        /// <summary>
        /// Used when 1st pass 
        /// </summary>
        private bool _initialized = false;

        /// <summary>
        /// The current log file
        /// </summary>
        private IFile _currentLogFile;

        public LogFileManager(FileLogOptions options)
        {
            _options = options;
        }

        public async Task<bool> WriteLogEntryAsync(string entry)
        {
            var newFileName = _options.FileName();

            // if not been initialized for 1st time, then do so
            if (!_initialized)
            {
                await InitializeAsync();
            }

            // file name change implies we need to create / open a log file...
            if (FileNameChanged(newFileName))
            {
                await OpenLogFileAsync(newFileName, _options.ClearAtStart).ConfigureAwait(false);
            }

            using (var stream = await _currentLogFile.OpenAsync(FileAccess.ReadAndWrite).ConfigureAwait(false))
            {
                // now want to append... the formatted string...
                stream.Seek(0, SeekOrigin.End);

                var formattedEntry = entry + "\r\n";

                var data = Encoding.UTF8.GetBytes(formattedEntry);
                await stream.WriteAsync(data, 0, data.Length);
            }

            return true;
        }

        /// <summary>
        /// Initialize the folder we are using for the logs.
        /// </summary>
        /// <returns></returns>
        public async Task InitializeAsync()
        {
            _logFolder = await FileSystem.Current.LocalStorage.CreateFolderAsync(_options.Folder, CreationCollisionOption.OpenIfExists).ConfigureAwait(false);

            _initialized = true;
        }

        /// <summary>
        /// Enumerable the available log files
        /// </summary>
        /// <returns></returns>
        public async Task<IList<IFile>> AvailableLogFilesAsync()
        {
            if (!_initialized)
            {
                await InitializeAsync().ConfigureAwait(false);
            }

            return await _logFolder.GetFilesAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Determine if the filename has changed for the log file 
        /// </summary>
        /// <param name="newFileName"></param>
        /// <returns></returns>
        private bool FileNameChanged(string newFileName)
        {
            return _currentLogFile == null || _currentLogFile.Name != FullFileName(newFileName);
        }

        /// <summary>
        /// Determine the full filename for a log file, including its extension.
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        private static string FullFileName(string filename)
        {
            return filename + ".application.log";
        }
        private async Task OpenLogFileAsync(string newFileName, bool clearIfExists)
        {
            var fullFileName = FullFileName(newFileName);

            var existance = await _logFolder.CheckExistsAsync(fullFileName).ConfigureAwait(false);

            if (existance == ExistenceCheckResult.NotFound)
            {
                _currentLogFile = await _logFolder.CreateFileAsync(fullFileName, clearIfExists ? CreationCollisionOption.ReplaceExisting : CreationCollisionOption.OpenIfExists).ConfigureAwait(false);
            }
            else
            {
                _currentLogFile = await _logFolder.GetFileAsync(fullFileName).ConfigureAwait(false);
            }
        }
    }
}

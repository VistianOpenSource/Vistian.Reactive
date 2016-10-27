using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vistian.Reactive.Logging.Formatting;
using Vistian.Xamarin;

namespace Vistian.Reactive.Logging.Providers
{
    /// <summary>
    /// Classifed Log message
    /// </summary>
	[Preserve(AllMembers = true)]
    [Formatter(typeof(ClassifiedFormatter))]
    public class Classified
    {
        private const string DefaultExceptionMessage = "Exception Type {0} , Exception Message: {1}";
        private const string DefaultExceptionMessageWithAdditional = "Exception Type {0} , Exception Message: {1} Additional Details : {2}";
        private const string DefaultNoMessage = "";

        /// <summary>
        /// Get the level 
        /// </summary>
        public LogLevel Level { get; private set; }

        /// <summary>
        /// Get the message
        /// </summary>
        public string Message => _params != null ? string.Format(_message, _params) : _message;

        private readonly object[] _params;

        /// <summary>
        /// The message used, either straight forward text or a format string.
        /// </summary>
        private readonly string _message;


        /// <summary>
        /// Get any associated <see cref="Exception"/>
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Create an instance of a specified level.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="message"></param>
        internal Classified(LogLevel level, string message)
        {
            Level = level;
            _message = message;
        }

        /// <summary>
        /// Create 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="format"></param>
        /// <param name="params"></param>
        internal Classified(LogLevel level, string format, object[] @params)
        {
            Level = level;
            _message = format;
            _params = @params;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Classified Error()
        {
            return new Classified(LogLevel.Error, DefaultNoMessage);
        }

        public static Classified Error(string message)
        {
            return new Classified(LogLevel.Error, message);
        }


        public static Classified Error(Exception exception)
        {
            return new Classified(LogLevel.Error, exception.Message);
        }

        public static Classified Error(Exception exception, string message)
        {
            var fullMessage = string.Format(DefaultExceptionMessageWithAdditional, exception.GetType().Name, exception.Message, message);

            return new Classified(LogLevel.Error, fullMessage);
        }

        public static Classified Error(string format, params object[] args)
        {
            return new Classified(LogLevel.Error, format, args);
        }


        public static Classified Information()
        {
            return new Classified(LogLevel.Information, DefaultNoMessage);
        }

        public static Classified Information(string message)
        {
            return new Classified(LogLevel.Information, message);
        }

        public static Classified Information(Exception exception)
        {
            var message = string.Format(DefaultExceptionMessage, exception.GetType().Name, exception.Message);

            return new Classified(LogLevel.Information, message) { Exception = exception };
        }

        public static Classified Information(string format, params object[] args)
        {
            return new Classified(LogLevel.Information, format, args);
        }

        public static Classified Warn()
        {
            return new Classified(LogLevel.Warning, DefaultNoMessage);
        }

        public static Classified Warn(string message)
        {
            return new Classified(LogLevel.Warning, message);
        }

        public static Classified Warn(Exception exception)
        {
            var message = string.Format(DefaultExceptionMessage, exception.GetType().Name, exception.Message);

            return new Classified(LogLevel.Warning, message) { Exception = exception }; ;
        }
        public static Classified Warn(string format, params object[] args)
        {
            return new Classified(LogLevel.Warning, format, args);
        }

        public static Classified Debug()
        {
            return new Classified(LogLevel.Information, DefaultNoMessage);
        }

        public static Classified Debug(string message)
        {
            return new Classified(LogLevel.Information, message);
        }

        public static Classified Debug(Exception exception)
        {
            var message = string.Format(DefaultExceptionMessage, exception.GetType().Name, exception.Message);
            return new Classified(LogLevel.Debug, message) { Exception = exception }; ;
        }

        public static Classified Debug(string format, params object[] args)
        {
            return new Classified(LogLevel.Debug, format, args);
        }
    }
}

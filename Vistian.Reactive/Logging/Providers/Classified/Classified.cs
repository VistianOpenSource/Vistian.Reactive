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
        public const string DefaultExceptionMessage = "Exception Type {0} , Exception Message: {1}";
        public const string DefaultExceptionMessageWithAdditional = "Exception Type {0} , Exception Message: {1} Additional Details : {2}";
        public const string DefaultNoMessage = "";

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
            return new Classified(LogLevel.Error, DefaultExceptionMessage,new object[] { exception.GetType().Name, exception.Message }) { Exception = exception };
        }

        public static Classified Error(Exception exception, string message)
        {
            return new Classified(LogLevel.Error, DefaultExceptionMessageWithAdditional,new object[] { exception.GetType().Name, exception.Message, message }) {Exception = exception};
        }

        public static Classified Error(string format, params object[] args)
        {
            return new Classified(LogLevel.Error, format, args);
        }

        public static Classified Fatal()
        {
            return new Classified(LogLevel.Fatal, DefaultNoMessage);
        }

        public static Classified Fatal(string message)
        {
            return new Classified(LogLevel.Fatal, message);
        }


        public static Classified Fatal(Exception exception)
        {
            return new Classified(LogLevel.Fatal, DefaultExceptionMessage,new object[] { exception.GetType().Name, exception.Message }) { Exception = exception };
        }

        public static Classified Fatal(Exception exception, string message)
        {
            return new Classified(LogLevel.Fatal, DefaultExceptionMessageWithAdditional,new object[] { exception.GetType().Name, exception.Message, message }) { Exception = exception };
        }

        public static Classified Fatal(string format, params object[] args)
        {
            return new Classified(LogLevel.Fatal, format, args);
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
            return new Classified(LogLevel.Information, DefaultExceptionMessage,new object[] { exception.GetType().Name, exception.Message }) { Exception = exception };
        }

        public static Classified Information(Exception exception,string additional)
        {
            return new Classified(LogLevel.Information, DefaultExceptionMessageWithAdditional,new object[] {exception.GetType().Name,exception.Message,additional}) { Exception = exception };
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

        public static Classified Warn(Exception exception,string additional)
        {
            return new Classified(LogLevel.Warning, DefaultExceptionMessageWithAdditional, new object[] { exception.GetType().Name, exception.Message, additional }) { Exception = exception }; ;
        }

        public static Classified Warn(string format, params object[] args)
        {
            return new Classified(LogLevel.Warning, format, args);
        }

        public static Classified Debug()
        {
            return new Classified(LogLevel.Debug, DefaultNoMessage);
        }

        public static Classified Debug(string message)
        {
            return new Classified(LogLevel.Debug, message);
        }

        public static Classified Debug(Exception exception)
        {
            return new Classified(LogLevel.Debug, DefaultExceptionMessage,new object[] { exception.GetType().Name, exception.Message}) { Exception = exception }; ;
        }

        public static Classified Debug(Exception exception,string additional)
        {
            return new Classified(LogLevel.Debug, DefaultExceptionMessageWithAdditional,new object[] { exception.GetType().Name, exception.Message, additional }) { Exception = exception }; ;
        }


        public static Classified Debug(string format, params object[] args)
        {
            return new Classified(LogLevel.Debug, format, args);
        }
    }
}

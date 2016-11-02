using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vistian.Reactive.Logging.Providers;
using Xunit;

namespace Vistian.Reactive.UnitTests.Logging
{
    public class ClassifiedTests
    {
        const string testMessage = "TEST";

        const string formattedMessage = "{0} test";

        private int testParam = 23;

        private Exception exception = new InvalidCastException("too bad"); 


        [Fact]
        public void InformationMessagesTest()
        {
            CreateForMessage((s) => Classified.Information(), LogLevel.Information,String.Empty);
            CreateForMessage(Classified.Information, LogLevel.Information, testMessage);
            CreateForMessageWithParams(Classified.Information, LogLevel.Information, formattedMessage, testParam);
            CreateForException(Classified.Information, LogLevel.Information, exception);
            CreateForExceptionWithAdditional(Classified.Information, LogLevel.Information, exception, testMessage);
        }

        [Fact]
        public void WarnMessagesTest()
        {
            CreateForMessage((s) => Classified.Warn(), LogLevel.Warning, String.Empty);
            CreateForMessage(Classified.Warn, LogLevel.Warning, testMessage);
            CreateForMessageWithParams(Classified.Warn, LogLevel.Warning, formattedMessage, testParam);
            CreateForException(Classified.Warn, LogLevel.Warning, exception);
            CreateForExceptionWithAdditional(Classified.Warn, LogLevel.Warning, exception, testMessage);
        }


        [Fact]
        public void DebugMessagesTest()
        {
            CreateForMessage((s) => Classified.Debug(), LogLevel.Debug, String.Empty);
            CreateForMessage(Classified.Debug, LogLevel.Debug, testMessage);
            CreateForMessageWithParams(Classified.Debug, LogLevel.Debug, formattedMessage, testParam);
            CreateForException(Classified.Debug, LogLevel.Debug, exception);
            CreateForExceptionWithAdditional(Classified.Debug, LogLevel.Debug, exception, testMessage);
        }

        [Fact]
        public void ErrorMessagesTest()
        {
            CreateForMessage((s) => Classified.Error(), LogLevel.Error, String.Empty);
            CreateForMessage(Classified.Error, LogLevel.Error, testMessage);
            CreateForMessageWithParams(Classified.Error, LogLevel.Error, formattedMessage, testParam);
            CreateForException(Classified.Error, LogLevel.Error, exception);
            CreateForExceptionWithAdditional(Classified.Error, LogLevel.Error, exception, testMessage);
        }

        [Fact]
        public void FatalMessagesTest()
        {
            CreateForMessage((s) => Classified.Fatal(), LogLevel.Fatal, String.Empty);
            CreateForMessage(Classified.Fatal, LogLevel.Fatal, testMessage);
            CreateForMessageWithParams(Classified.Fatal, LogLevel.Fatal, formattedMessage, testParam);
            CreateForException(Classified.Fatal, LogLevel.Fatal, exception);
            CreateForExceptionWithAdditional(Classified.Fatal, LogLevel.Fatal, exception, testMessage);
        }


        private Classified CreateForMessage(Func<string,Classified> create,LogLevel level,string message)
        {
            var c = create(message);

            Assert.Equal(level,c.Level);
            Assert.Null(c.Exception);
            Assert.Equal(message,c.Message);

            return c;
        }

        private Classified CreateForMessageWithParams(Func<string,object[], Classified> create, LogLevel level, string message, params object[] args)
        {
            var c = create(message,args);

            Assert.Equal(level, c.Level);
            Assert.Null(c.Exception);

            var formatted = string.Format(message, args);

            Assert.Equal(formatted, c.Message);

            return c;
        }

        private Classified CreateForException(Func<Exception,Classified> create, LogLevel level,Exception exception)
        {
            var c = create(exception);

            Assert.Equal(level, c.Level);
            Assert.Equal(exception,c.Exception);

            var message = string.Format(Classified.DefaultExceptionMessage, exception.GetType().Name, exception.Message);

            Assert.Equal(message, c.Message);

            return c;
        }

        private Classified CreateForExceptionWithAdditional(Func<Exception, string,Classified> create, LogLevel level, Exception exception,string message)
        {
            var c = create(exception,message);

            Assert.Equal(level, c.Level);
            Assert.Equal(exception, c.Exception);

            var formatted = string.Format(Classified.DefaultExceptionMessageWithAdditional, exception.GetType().Name, exception.Message,message);

            Assert.Equal(formatted, c.Message);

            return c;
        }

    }
}

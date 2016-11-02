using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Vistian.Reactive.Logging;
using Vistian.Reactive.Logging.Configuration;
using Xunit;

namespace Vistian.Reactive.UnitTests.Logging
{
    //TODO, could do with a bit of a tidy up...
    public class RxObservableLogHostTests
    {
        private RxLogEntry _lastValue;

        [Fact]
        public void CreateSuccessfullyTest()
        {
            var configuration = RxLoggerConfiguration.Create();

            var host = new RxObservableLogHost(configuration);

            Assert.Equal(host.Configuration,configuration);
        }

        [Fact]
        public void PublishedItemsChanneledThroughConfigTest()
        {
            var hostConfiguration = new HostConfiguration(prePumpObservable:(e) => PumpTest(e),publishScheduler:Scheduler.Immediate);

            var configuration = RxLoggerConfiguration.Create(hostConfiguration: hostConfiguration);

            var host = new RxObservableLogHost(configuration);

            var entry = RxLogEntry.WithNoMeta(new object());

            host.Publish(entry);

            Assert.Same(entry,_lastValue);
        }

        [Fact]
        public void ErrorsDuringProcessingPassedToErrorHandlerTest()
        {
            Exception lastException = null;

            var hostConfiguration = new HostConfiguration(prePumpObservable: (e) => PumpTest(e,true), publishScheduler: Scheduler.Immediate,errored:
                (e) =>
                {
                    lastException = e;
                    return Observable.Return(Unit.Default);
                }) ;

            var configuration = RxLoggerConfiguration.Create(hostConfiguration: hostConfiguration);

            var host = new RxObservableLogHost(configuration);

            var entry = RxLogEntry.WithNoMeta(new object());

            host.Publish(entry);

            Assert.NotNull(lastException);
        }

        // also need : subscribers successfully build
        // crash in subscriber doesn't take down everything
        // can filter out things in pre-sub...

        [Fact]
        public void SubscribersCorrectlySubscribedTest()
        {
            var hostConfiguration = new HostConfiguration(prePumpObservable: (e) => PumpTest(e, false), publishScheduler: Scheduler.Immediate);

            var configuration = RxLoggerConfiguration.Create(hostConfiguration: hostConfiguration);

            RxLogEntry lastValue = default(RxLogEntry);

            configuration.AddSubscriber((c, en) => TestSubscriber(c,en,(v) => lastValue=v));

            var host = new RxObservableLogHost(configuration);

            var entry = RxLogEntry.WithNoMeta(new object());

            host.Publish(entry);

            Assert.Same(entry,lastValue);
        }

        [Fact]
        public void PublishedItemsCanBeFilteredTest()
        {
            // indicate that pump shouldn't pass through messages
            var hostConfiguration = new HostConfiguration(prePumpObservable: (e) => PumpTest(e, ignoreAll: true), publishScheduler: Scheduler.Immediate);

            var configuration = RxLoggerConfiguration.Create(hostConfiguration: hostConfiguration);


            RxLogEntry lastSubValue = default(RxLogEntry);
            configuration.AddSubscriber((c, en) => TestSubscriber(c, en, (r) => lastSubValue = r, raiseException: true));
          
            var host = new RxObservableLogHost(configuration);

            var entry = RxLogEntry.WithNoMeta(new object());

            host.Publish(entry);

            // verify not received anything.
            Assert.Null(lastSubValue);
        }

        [Fact]
        public void CrashingSubscriberHandledCorrectlyTest()
        {
            var hostConfiguration = new HostConfiguration(publishScheduler: Scheduler.Immediate);
            var configuration = RxLoggerConfiguration.Create(hostConfiguration);          

            RxLogEntry lastValueSub1 = null;
            RxLogEntry lastValueSub2 = null;

            configuration.AddSubscriber((c, en) => TestSubscriber(c, en,(r) => lastValueSub1=r,raiseException:true));
            configuration.AddSubscriber((c, en) => TestSubscriber(c, en, (r) => lastValueSub2 = r));

            // one subscription we will let it raise exceptions, other won't.
            var entry1 = RxLogEntry.WithNoMeta(new object());
            var entry2 = RxLogEntry.WithNoMeta(new object());

            var host = new RxObservableLogHost(configuration);

            host.Publish(entry1);
           
            Assert.Null(lastValueSub1);
            Assert.Same(entry1,lastValueSub2);

            host.Publish(entry2);

            Assert.Null(lastValueSub1);
            Assert.Same(entry2, lastValueSub2);
        }

        private IObservable<Unit> TestSubscriber(RxLoggerConfiguration rxLoggerConfiguration, IObservable<RxLogEntry> source,Action<RxLogEntry> reportLastValue,bool raiseException=false)
        {
            return Observable.Create<Unit>((o) =>
            {
                var sub = source.Subscribe(v =>
                {
                    if (raiseException)
                    {
                        o.OnError(new Exception());
                    }
                    else
                    {
                        reportLastValue(v);
                        o.OnNext(Unit.Default);
                    }
                });

                return sub;
            });
        }


        public IObservable<RxLogEntry> PumpTest(IObservable<RxLogEntry> source,bool throwException=false,bool ignoreAll=false)
        {
            return Observable.Create<RxLogEntry>((o) =>
            {
                var sub = source.Subscribe(v =>
                {
                    if (throwException)
                    {
                        o.OnError(new ArgumentException());
                    }

                    _lastValue = v;

                    if (!ignoreAll)
                    {
                        o.OnNext(v);
                    }
                });

                return sub;
            });
        }
    }
}

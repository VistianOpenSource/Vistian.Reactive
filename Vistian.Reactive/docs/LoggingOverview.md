# Vistian.Reactive.Logging 

Logging for ReactiveUI/Rx.Net based solutions, functioning in a reactive way.

## Background
Oh no I hear you cry, not another logging library. Isn't there enough of these already? More than likely yes. Our requirements were however a little different and when this codebase was first constructed the choice was a bit more limited.

Our primarily requirements were:
* Log structured data, not just textual.
* Adjust, shape and filter how this structured data is processed along with where the logged data ends up.
* Meta data about each logged item, including source class,member name, line number
* Adjustable behaviour through configuration.

## Our approach
When we looked at how we wanted to structure the processing of logging information it felt clear to us that using Observables and in effect creating subscriptions for the differing ways in which the log entries are processed would give us all of the flexibility we needed.

## How to use
1. Create a configuration specifying how information is to be logged along with those subscribers which are to be used.
2. Create a logger
3. Get logging!

## Example
Create a configuration, in this case a single log subscriber to the Android console.

```
    var config = RxLoggerConfiguration.Create(Droid.Logging.Console.LogAll);
```
Create the logger so that logging can be performed.

```
config.CreateLogger();
```

Get logging

```
this.Log(Classified.Information("We have got this far {0}",i))
this.Log(startUpMetrics)
this.Log(userEvent)

// and / or

RxLog.Log(this,Classified.Information("We have got this far"))
RxLog.Log(this,startUpMetrics)
RxLog.Log(this,userEvent)
```
Get console output
```
10:37:28.358 FacebookActivity.DefaultCreate:124 We have got this far
10:37:29.042 BootLoader.PerformBootSequence:161 BootLoader Timer
Total = 1
Last = 503 ms
...
```

## How it works
### Getting logging data in

Literally anything can be logged typically though this is structured data. In the example above, we have a textual format log, but this is infact
structured data, a **Classified** instance being logged.

Each log entry **RxLogEntry** is comprised of two things:
1. The meta data about the entry - when the entry was created, where it was created from etc.
2. The actual logged instance.

Custom meta data capture can either be provided in a custom fashion with each log entry, or by a configuration supplied custom meta data provider that would be invoked automatically (e.g. add threading details).

**RxLog.Log** 'publishes' each log entry (OnNext) to an internal **Subject**. It should be noted that custom behaviour (since we are dealing with observables) can be applied to each log entry prior to it being made available for subscription by log subscribers.

### Getting logged data out
As mentioned previously, all the processing of logs is done through subscribers of an IObservable sequence of **RxLogEntry**.

What each subscriber does is effectively down to it, but some support infrastucture is supplied for the most common scenarios.

1. The logger configuration allows for the specification of a FormatterResolver. Given an instance of a log entry the resolver will determine if a formatter exists for the specific instance.  The formatter is responsible for taking the log entry and producing a textual representation of it.
2. A default formatter, outputting the public properties of the log instance is supplied.
3. Default formatters for a particular instance type can be specified through attributes applied to the instance type e.g.

``` 
/// <summary>
/// A discrete timing block.
/// </summary>
///
[Formatter(typeof(TimingBlockFormatter))]
public class TimingBlock
{
/// <summary>
/// The list of individual timing entries.
/// </summary>

```
For example here is part of the output from the TimingBlockFormatter

```
10:37:27.042 BootLoader.PerformBootSequence:161 BootLoader Timer
Total = 1
Last = 503 ms
#1 begin =  11 ms (+11) ms
#2 initialization begin =  19 ms (+8) ms
#3 Component Start AutofacBootComponent =  28 ms (+9) ms
#4 Component End AutofacBootComponent =  117 ms (+89) ms
#5 Component Start JsonModuleSettingsBootComponent =  117 ms (+0) ms
#6 Component End JsonModuleSettingsBootComponent =  117 ms (+0) ms
#7 Component Start RxUiBootComponent =  117 ms (+0) ms
#8 Component End RxUiBootComponent =  249 ms (+132) ms
#9 Component Start ViewsRegistrationBootComponent =  249 ms (+0) ms
#10 Component End ViewsRegistrationBootComponent =  290 ms (+41) ms
#11 Component Start AutoMapperBootComponent =  290 ms (+0) ms
#12 Component End AutoMapperBootComponent =  290 ms (+0) ms
#13 Component Start ModulesAutofacBootComponent =  290 ms (+0) ms
#14 Component End ModulesAutofacBootComponent =  426 ms (+136) ms
#15 Component Start InstanceStartupComponent =  426 ms (+0) ms
#16 Component End InstanceStartupComponent =  427 ms (+1) ms
#17 Component Start AndroidUISetupComponent =  427 ms (+0) ms

```
The code which recorded the logging information doesn't do any of this formatting, but instead looks like this:

```
    var bootTimer = Metric.Timer("Boot");

    using (var bootContext = bootTimer.NewContext(this.GetType().Name).LogOnDispose(this))
    {
        bootContext.Mark("begin");

        var components = _bootComponents.OrderByDescending(b => b.Priority);

        bootContext.Mark("initialization begin");

        foreach (var item in components)
        {
            bootContext.Mark("Component Start " + item.GetType().Name);
            item.Boot(this);
            bootContext.Mark("Component End " + item.GetType().Name);
        }

        foreach (var item in components)
        {
            bootContext.Mark("Boot Complete Start " + item.GetType().Name);
            item.BootCompleted(this);
            bootContext.Mark("Boot Complete End " + item.GetType().Name);
        }
```
*This output came from our Metrics logging library extensions.*

## Some additional elements

Extensions for observable logging have also been created which make life just a little bit easier:
1. Trace - constructs and publishes Classified log entries for Subscribe, Dispose, OnError, OnCompleted and OnNext values.
2. Log - log custom entry values for each OnNext value.
3. DebugBreak - force debugger to break (if connected) upon specified conditions including OnNext values,Subscribe, Dispose, OnError & OnCompleted. Not really logging but fits well with the other two helpers.

## Android Console
An Android service allowing you to easily view the logging information on your Android device.

* Provides a 'shrinkable', unobtrusive window which can:
* View current log entries
* Clear log
* Copy log data to clipboard
* Minimized to just an icon
* Can be moved around screen

![Console](AndroidLoggingConsole.png?raw=true "Console")

## Outstanding
1. No console view currently for iOS, might be possible to do something similar to Android console.
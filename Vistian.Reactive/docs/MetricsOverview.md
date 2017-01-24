# Vistian.Reactive.Metrics

Metric extensions for ReactiveUI/Rx.Net based solutions, tying in with **Vistian.Reactive.Logging**.

## Background
Whether its understanding how long it takes individual components to execute when a mobile application starts or how many REST requests have been made (along with their average duration) the capture of meaningful metrics can greatly held in improving the customer experience.

This library of simple classes, combined with their integration with **Vistian.Reactive.Logging** has helped us in more easily observing application performance.

The library has heavily influenced by other .NET metric libraries including Metrics.Net

## Capabilities

* **Counter** - records a simplistic counter value which is incremented or decremented, e.g. total number of REST requests.
* **Discrete** - records discrete values, which can change over time e.g. size in bytes of last REST request.
* **Timer** - timing for a particular event e.g. time taken to perform a REST request.
* **TimerContext** - measures a particular region of code by wrappering it with a  block and a .
* **TimingBlock** - a multi-step sequence of timings e.g. measuring different parts of an operation.

A global **Metrics** context allows for metrics to be 'kept around' e.g. for counting total number of requests.

All core metric classes **are** thread safe for the recording of metric data.

**Important**: It should be noted that unless explicity requested, no logging of metric data occurs unless it is specifically requested. Logging can be either invoked by just performing the standard **RxLog.Log(...)** operation with the appropriate metric, or using some of the extension methods for performing logging.  Period dumping of global metrics can be easily achieved through an  triggering a  call.

## Logging Extensions

Each metric has its own **IRxFormatter** implementation.  A typical  output can be seen below

```
10:37:27.042 BootLoader.PerformBootSequence:161 BootLoader Timer
Total = 1
Last = 503 ms
1 begin =  11 ms (+11) ms
2 initialization begin =  19 ms (+8) ms
3 Component Start AutofacBootComponent =  28 ms (+9) ms
4 Component End AutofacBootComponent =  117 ms (+89) ms
5 Component Start JsonModuleSettingsBootComponent =  117 ms (+0) ms
6 Component End JsonModuleSettingsBootComponent =  117 ms (+0) ms
7 Component Start RxUiBootComponent =  117 ms (+0) ms
8 Component End RxUiBootComponent =  249 ms (+132) ms
9 Component Start ViewsRegistrationBootComponent =  249 ms (+0) ms
10 Component End ViewsRegistrationBootComponent =  290 ms (+41) ms
11 Component Start AutoMapperBootComponent =  290 ms (+0) ms
12 Component End AutoMapperBootComponent =  290 ms (+0) ms
13 Component Start ModulesAutofacBootComponent =  290 ms (+0) ms
14 Component End ModulesAutofacBootComponent =  426 ms (+136) ms
15 Component Start InstanceStartupComponent =  426 ms (+0) ms
16 Component End InstanceStartupComponent =  427 ms (+1) ms
17 Component Start AndroidUISetupComponent =  427 ms (+0) ms
```
Logging of a **TimingContext** upon dispose can be easily achieved through the use of the  extension, e.g.

```
var bootTimer = Metric.Timer("Boot");

using (var bootContext = bootTimer.NewContext(this.GetType().Name).LogOnDispose(this))**
**{
    bootContext.Mark("begin");
```
# HttpClient Diagnostic  Instrumentation Users Guide

This document describes `HttpClientHandler` instrumentation with [DiagnosticSource](https://github.com/dotnet/corefx/blob/master/src/System.Diagnostics.DiagnosticSource/src/DiagnosticSourceUsersGuide.md).

# Overview
Applications typically log outgoing HTTP requests; typically it's done with DelegatingHandler implementation that logs every request. However in an existing system it may require extensive code changes, since DelegatingHandler needs to be added to HttpClient pipeline every time when new client is created.
DiagnosticListener instrumentation allows to enable outgoing request tracing with a few lines of code; it also provides context necessary to correlate logs.

Context is represented as `System.Diagnostics.Activity` class. Activity may be started as a child of another Activity and the whole operation is represented with a tree of Activities. You can find more details in [Activity User Guide](https://github.com/dotnet/corefx/blob/master/src/System.Diagnostics.DiagnosticSource/src/ActivityUserGuide.md).

Activity carries useful properties for logging such as Id, start time, Tags, Baggage and parent information. 

Instrumentation ensures `Activity.Current` represents current outgoing request in Write event callbacks (if request is instrumented). Consumers **should not** assume `Activity.Current` is accurate in IsEnabled callbacks.

In a microservice environment some context should flow with outgoing requests to correlate telemetry from all services involved in processing an operation.
Instrumentation adds context into the request headers: 
 * Request-Id header with `Activity.Id` value
 * Correlation-Context header with `Activity.Baggage` key-value pair list in `k1=v1, k2=v2` format
 
See [HTTP Protocol proposal](https://github.com/dotnet/corefx/blob/master/src/System.Diagnostics.DiagnosticSource/src/HttpCorrelationProtocol.md) for more details.

## Subscription
Instrumentation is off by default; to enable it, consumer first needs to subscribe to a DiagnosticListener called "HttpHandlerDiagnosticListener". 

```C#
var subscription = DiagnosticListener.AllListeners.Subscribe(delegate (DiagnosticListener listener)
{
    if (listener.Name == "HttpHandlerDiagnosticListener")
    {
        listener.Subscribe(delegate (KeyValuePair<string, object> value)
        {
            Console.WriteLine($"Event: {value.Key} ActivityName: {Activity.Current.OperationName} Id: {Activity.Current.Id} ");
        });
    }
} );
```

## Events
If there is **at least one consumer** subscribed for "HttpHandlerDiagnosticListener" events, `HttpClientHandler` instruments outgoing request depending on subscription and request properties.

Note that DiagnosticListener best practice is to guard every `Write` method with `IsEnabled` check. In case there is more than one consumer, **each consumer** will receive Write event if **at least one** consumer returned true for `IsEnabled`.

#### IsEnabled: System.Net.Http.Activity
If there is a consumer, instrumentation calls `DiagnosticListener.IsEnabled("System.Net.Http.HttpRequestOut", request)` to check if particular request needs to be instrumented.
Consumer may optionally provide predicate to DiagnosticListener to prevent some requests from being instrumented: e.g. if logging system has HTTP interface, it could be necessary to filter out requests to logging system itself.

```C#
    var predicate = (name, r, _) => 
    {
        var request = r as HttpRequestMessage;
        if (request != null)
        {
           return !request.RequestUri.Equals(ElasticSearchUri);
        }
        return true;
    }
    listener.Subscribe(observer, predicate);
```
### System.Net.Http.Activity.Start
After initial instrumentation preconditions are met and `DiagnosticListener.IsEnabled("System.Net.Http.HttpRequestOut", request)` check is passed, instrumentation starts a new Activity to represent outgoing request.
If **"System.Net.Http.HttpRequestOut.Start"** is enabled, instrumentation writes it. Event payload has Request property with `HttpRequestMessage` object representing request.

### System.Net.Http.Activity.Stop
When request is completed (faulted with exception, cancelled or successfully completed), instrumentation stops activity and writes  **"System.Net.Http.HttpRequestOut.Stop"** event.

Event payload has following properties:
* **Response**  with `HttpResponseMessage` object representing response, which could be null if request was failed or cancelled.  
* **Request**  with `HttpRequestMessage` object representing request. If response was received, you can also access it with `HttpResponseMessage.RequestMessage`, but if there was no response, it could be accessed only from the event payload  
* **RequestTaskStatus** with `TaskStatus` enum value that describes status of the request task.

This event is sent under the same conditions as "System.Net.Http.HttpRequestOut.Start" event.

#### IsEnabled: System.Net.Http.Exception
If request processing causes an exception, instrumentation first checks if consumer wants to receive Exception event.

### System.Net.Http.Exception
If request throws an exception, instrumentation sends **"System.Net.Http.Exception"** event with payload containing `Exception` and `Request` properties.

If current outgoing request is instrumented, `Activity.Current` represents it's context.
Otherwise, `Activity.Current` represent some 'parent' activity (presumably incoming request).

# Events Flow and Order

1. `DiagnosticListener.IsEnabled()` - determines if there is a consumer
2. `DiagnosticListener.IsEnabled("System.Net.Http.HttpRequestOut", request)` - determines if this particular request should be instrumented
3. `DiagnosticListener.IsEnabled("System.Net.Http.HttpRequestOut.Start")` - determines if Start event should be written
4. `DiagnosticListener.Write("System.Net.Http.HttpRequestOut.Start", new {Request})` - notifies that activity (outgoing request) was started
5. `DiagnosticListener.IsEnabled("System.Net.Http.Exception")` - determines if exception event (if thrown) should be written
6. `DiagnosticListener.Write("System.Net.Http.HttpRequestOut.Exception", new {Exception, Request})` - notifies about exception during request processing (if thrown)
7. `DiagnosticListener.Write("System.Net.Http.HttpRequestOut.Stop", new {Response, RequestTaskStatus})` - notifies that activity (outgoing request) is stopping

# Non-Activity events (deprecated)
There are two events System.Net.Http.Request and System.Net.Http.Response, currently are also emited for compatibility purposes.  
They are redundant with the System.Net.Http.HttpRequestOut start and stop events (but do not set Activity.Current and follow activity conventions (start/stop)
They are deprecated, and consumers are advised only to depend on them to suport.NET Core V1.1 apps (where the new events are not present).
It is likely that these deprecated events will be remove at some point.  
Consumers should  migrate to System.Net.Http.HttpRequestOut.Start and System.Net.Http.HttpRequestOut.Stop events instead.
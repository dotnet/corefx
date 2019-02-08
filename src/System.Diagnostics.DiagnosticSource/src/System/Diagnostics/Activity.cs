// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Text;
#if ALLOW_PARTIALLY_TRUSTED_CALLERS
    using System.Security;
#endif
using System.Threading;

namespace System.Diagnostics
{
    /// <summary>
    /// Activity represents operation with context to be used for logging.
    /// Activity has operation name, Id, start time and duration, tags and baggage.
    ///  
    /// Current activity can be accessed with static AsyncLocal variable Activity.Current.
    /// 
    /// Activities should be created with constructor, configured as necessary
    /// and then started with Activity.Start method which maintains parent-child
    /// relationships for the activities and sets Activity.Current.
    /// 
    /// When activity is finished, it should be stopped with static Activity.Stop method.
    /// 
    /// No methods on Activity allow exceptions to escape as a response to bad inputs.
    /// They are thrown and caught (that allows Debuggers and Monitors to see the error)
    /// but the exception is suppressed, and the operation does something reasonable (typically
    /// doing nothing).  
    /// </summary>
    public partial class Activity
    {
        /// <summary>
        /// An operation name is a COARSEST name that is useful grouping/filtering. 
        /// The name is typically a compile-time constant.   Names of Rest APIs are
        /// reasonable, but arguments (e.g. specific accounts etc), should not be in
        /// the name but rather in the tags.  
        /// </summary>
        public string OperationName { get; }

        /// <summary>
        /// This is an ID that is specific to a particular request.   Filtering
        /// to a particular ID insures that you get only one request that matches.  
        /// Id has a hierarchical structure: '|root-id.id1_id2.id3_' Id is generated when 
        /// <see cref="Start"/> is called by appending suffix to Parent.Id
        /// or ParentId; Activity has no Id until it started
        /// <para/>
        /// See <see href="https://github.com/dotnet/corefx/blob/master/src/System.Diagnostics.DiagnosticSource/src/ActivityUserGuide.md#id-format"/> for more details
        /// </summary>
        /// <example>
        /// Id looks like '|a000b421-5d183ab6.1.8e2d4c28_1.':<para />
        ///  - '|a000b421-5d183ab6.' - Id of the first, top-most, Activity created<para />
        ///  - '|a000b421-5d183ab6.1.' - Id of a child activity. It was started in the same process as the first activity and ends with '.'<para />
        ///  - '|a000b421-5d183ab6.1.8e2d4c28_' - Id of the grand child activity. It was started in another process and ends with '_'<para />
        /// 'a000b421-5d183ab6' is a <see cref="RootId"/> for the first Activity and all its children
        /// </example>
        public string Id
        {
            get
            {
                // if we represented it as a traceId-spanId, convert it to a string.  
                if (_id == null && _spanIdSet)
                    _id = "00-" + _traceId.AsHexString + "-" + _spanId.AsHexString + "-00";
                return _id;
            }
        }

        /// <summary>
        /// The time that operation started.  It will typically be initialized when <see cref="Start"/>
        /// is called, but you can set at any time via <see cref="SetStartTime(DateTime)"/>.
        /// </summary>
        public DateTime StartTimeUtc { get; private set; }

        /// <summary>
        /// If the Activity that created this activity is from the same process you can get 
        /// that Activity with Parent.  However, this can be null if the Activity has no
        /// parent (a root activity) or if the Parent is from outside the process.
        /// </summary>
        /// <seealso cref="ParentId"/>
        public Activity Parent { get; private set; }

        /// <summary>
        /// If the parent for this activity comes from outside the process, the activity
        /// does not have a Parent Activity but MAY have a ParentId (which was deserialized from
        /// from the parent).   This accessor fetches the parent ID if it exists at all.  
        /// Note this can be null if this is a root Activity (it has no parent)
        /// <para/>
        /// See <see href="https://github.com/dotnet/corefx/blob/master/src/System.Diagnostics.DiagnosticSource/src/ActivityUserGuide.md#id-format"/> for more details
        /// </summary>
        public string ParentId
        {
            get
            {
                // if we represented it as a traceId-spanId, convert it to a string.  
                if (_parentId == null)
                {
                    if (_parentSpanIdSet)
                        _parentId = "00-" + _traceId.AsHexString + "-" + _parentSpanId.AsHexString + "-00";
                    else if (Parent != null)
                        _parentId = Parent.Id;
                }
                return _parentId;
            }
        }

        /// <summary>
        /// Root Id is substring from Activity.Id (or ParentId) between '|' (or beginning) and first '.'.
        /// Filtering by root Id allows to find all Activities involved in operation processing.
        /// RootId may be null if Activity has neither ParentId nor Id.
        /// See <see href="https://github.com/dotnet/corefx/blob/master/src/System.Diagnostics.DiagnosticSource/src/ActivityUserGuide.md#id-format"/> for more details
        /// </summary>
        public string RootId
        {
            get
            {
                //we expect RootId to be requested at any time after activity is created, 
                //possibly even before it was started for sampling or logging purposes
                //Presumably, it will be called by logging systems for every log record, so we cache it.
                if (_rootId == null)
                {
                    if (Id != null)
                    {
                        _rootId = GetRootId(Id);
                    }
                    else if (ParentId != null)
                    {
                        _rootId = GetRootId(ParentId);
                    }
                }
                return _rootId;
            }
        }

        /// <summary>
        /// Tags are string-string key-value pairs that represent information that will
        /// be logged along with the Activity to the logging system.   This information
        /// however is NOT passed on to the children of this activity.
        /// </summary>
        /// <seealso cref="Baggage"/>
        public IEnumerable<KeyValuePair<string, string>> Tags
        {
            get
            {
                for (var tags = _tags; tags != null; tags = tags.Next)
                    yield return tags.keyValue;
            }
        }

        /// <summary>
        /// Baggage is string-string key-value pairs that represent information that will
        /// be passed along to children of this activity.   Baggage is serialized 
        /// when requests leave the process (along with the ID).   Typically Baggage is
        /// used to do fine-grained control over logging of the activity and any children.  
        /// In general, if you are not using the data at runtime, you should be using Tags 
        /// instead. 
        /// </summary> 
        public IEnumerable<KeyValuePair<string, string>> Baggage
        {
            get
            {
                for (var activity = this; activity != null; activity = activity.Parent)
                    for (var baggage = activity._baggage; baggage != null; baggage = baggage.Next)
                        yield return baggage.keyValue;
            }
        }

        /// <summary>
        /// Returns the value of the key-value pair added to the activity with <see cref="AddBaggage(string, string)"/>.
        /// Returns null if that key does not exist.  
        /// </summary>
        public string GetBaggageItem(string key)
        {
            foreach (var keyValue in Baggage)
                if (key == keyValue.Key)
                    return keyValue.Value;
            return null;
        }

        /* Constructors  Builder methods */

        /// <summary>
        /// Note that Activity has a 'builder' pattern, where you call the constructor, a number of 'Set*' and 'Add*' APIs and then
        /// call <see cref="Start"/> to build the activity.  You MUST call <see cref="Start"/> before using it.
        /// </summary>
        /// <param name="operationName">Operation's name <see cref="OperationName"/></param>
        public Activity(string operationName)
        {
            if (string.IsNullOrEmpty(operationName))
            {
                NotifyError(new ArgumentException($"{nameof(operationName)} must not be null or empty"));
                return;
            }

            OperationName = operationName;
        }

        /// <summary>
        /// Update the Activity to have a tag with an additional 'key' and value 'value'.
        /// This shows up in the <see cref="Tags"/>  enumeration.   It is meant for information that
        /// is useful to log but not needed for runtime control (for the latter, <see cref="Baggage"/>)
        /// </summary>
        /// <returns>'this' for convenient chaining</returns>
        public Activity AddTag(string key, string value)
        {
            _tags = new KeyValueListNode() { keyValue = new KeyValuePair<string, string>(key, value), Next = _tags };
            return this;
        }

        /// <summary>
        /// Update the Activity to have baggage with an additional 'key' and value 'value'.
        /// This shows up in the <see cref="Baggage"/> enumeration as well as the <see cref="GetBaggageItem(string)"/>
        /// method.
        /// Baggage is meant for information that is needed for runtime control.   For information 
        /// that is simply useful to show up in the log with the activity use <see cref="Tags"/>.
        /// Returns 'this' for convenient chaining.
        /// </summary>
        /// <returns>'this' for convenient chaining</returns>
        public Activity AddBaggage(string key, string value)
        {
            _baggage = new KeyValueListNode() { keyValue = new KeyValuePair<string, string>(key, value), Next = _baggage };
            return this;
        }

        /// <summary>
        /// Updates the Activity To indicate that the activity with ID <paramref name="parentId"/>
        /// caused this activity.   This is intended to be used only at 'boundary' 
        /// scenarios where an activity from another process logically started 
        /// this activity. The Parent ID shows up the Tags (as well as the ParentID 
        /// property), and can be used to reconstruct the causal tree.  
        /// Returns 'this' for convenient chaining.
        /// </summary>
        /// <param name="parentId">The id of the parent operation.</param>
        public Activity SetParentId(string parentId)
        {
            if (Parent != null)
            {
                NotifyError(new InvalidOperationException($"Trying to set {nameof(ParentId)} on activity which has {nameof(Parent)}"));
            }
            else if (ParentId != null || _parentSpanIdSet)
            {
                NotifyError(new InvalidOperationException($"{nameof(ParentId)} is already set"));
            }
            else if (string.IsNullOrEmpty(parentId))
            {
                NotifyError(new ArgumentException($"{nameof(parentId)} must not be null or empty"));
            }
            else
            {
                _parentId = parentId;
            }
            return this;
        }

        /// <summary>
        /// Set the parent ID using the W3C convention using a TraceId and a SpanId.   This
        /// constructor has the advantage that no string manipulation is needed to set the ID.  
        /// </summary>
        public Activity SetParentId(in TraceId traceId, in SpanId spanId)
        {
            if (Parent != null)
            {
                NotifyError(new InvalidOperationException($"Trying to set {nameof(ParentId)} on activity which has {nameof(Parent)}"));
            }
            else if (ParentId != null || _parentSpanIdSet)
            {
                NotifyError(new InvalidOperationException($"{nameof(ParentId)} is already set"));
            }
            else
            {
                _traceId = traceId;     // The child will share the parent's traceId.  
                _traceIdSet = true;
                _parentSpanId = spanId;
                _parentSpanIdSet = true;
            }
            return this;
        }

        /// <summary>
        /// Update the Activity to set start time
        /// </summary>
        /// <param name="startTimeUtc">Activity start time in UTC (Greenwich Mean Time)</param>
        /// <returns>'this' for convenient chaining</returns>
        public Activity SetStartTime(DateTime startTimeUtc)
        {
            if (startTimeUtc.Kind != DateTimeKind.Utc)
            {
                NotifyError(new InvalidOperationException($"{nameof(startTimeUtc)} is not UTC"));
            }
            else
            {
                StartTimeUtc = startTimeUtc;
            }
            return this;
        }

        /// <summary>
        /// Update the Activity to set <see cref="Duration"/>
        /// as a difference between <see cref="StartTimeUtc"/>
        /// and <paramref name="endTimeUtc"/>.
        /// </summary>
        /// <param name="endTimeUtc">Activity stop time in UTC (Greenwich Mean Time)</param>
        /// <returns>'this' for convenient chaining</returns>
        public Activity SetEndTime(DateTime endTimeUtc)
        {
            if (endTimeUtc.Kind != DateTimeKind.Utc)
            {
                NotifyError(new InvalidOperationException($"{nameof(endTimeUtc)} is not UTC"));
            }
            else
            {
                Duration = endTimeUtc - StartTimeUtc;
                if (Duration.Ticks <= 0)
                    Duration = new TimeSpan(1); // We want Duration of 0 to mean  'EndTime not set)
            }
            return this;
        }

        /// <summary>
        /// If the Activity has ended (<see cref="Stop"/> or <see cref="SetEndTime"/> was called) then this is the delta
        /// between <see cref="StartTimeUtc"/> and end.   If Activity is not ended and <see cref="SetEndTime"/> was not called then this is 
        /// <see cref="TimeSpan.Zero"/>.
        /// </summary>
        public TimeSpan Duration { get; private set; }

        /// <summary>
        /// Starts activity
        /// <list type="bullet">
        /// <item>Sets <see cref="Parent"/> to hold <see cref="Current"/>.</item>
        /// <item>Sets <see cref="Current"/> to this activity.</item>
        /// <item>If <see cref="StartTimeUtc"/> was not set previously, sets it to <see cref="DateTime.UtcNow"/>.</item>
        /// <item>Generates a unique <see cref="Id"/> for this activity.</item>
        /// </list>
        /// Use <see cref="DiagnosticSource.StartActivity(Activity, object)"/> to start activity and write start event.
        /// </summary>
        /// <seealso cref="DiagnosticSource.StartActivity(Activity, object)"/>
        /// <seealso cref="SetStartTime(DateTime)"/>
        public Activity Start()
        {
            // Has the ID already been set (have we called Start()).  
            if (_id != null || _spanIdSet)
            {
                NotifyError(new InvalidOperationException("Trying to start an Activity that was already started"));
            }
            else
            {
                if (_parentId == null && !_parentSpanIdSet)
                {
                    var parent = Current;
                    if (parent != null)
                    {
                        // The parent change should not form a loop.   We are actually guaranteed this because
                        // 1. Unstarted activities can't be 'Current' (thus can't be 'parent'), we throw if you try.  
                        // 2. All started activities have a finite parent change (by inductive reasoning).  
                        Parent = parent;
                    }
                }

                if (StartTimeUtc == default(DateTime))
                    StartTimeUtc = GetUtcNow();

                // Figure out what format to use.  
                if (ForceDefaultIdFormat)
                    IdFormat = DefaultIdFormat;
                else if (Parent != null)
                    IdFormat = Parent.IdFormat;
                else if (_parentSpanIdSet)
                    IdFormat = ActivityIdFormat.W3C;
                else if (_parentId != null)
                    IdFormat = IsW3CId(_parentId) ? ActivityIdFormat.W3C : ActivityIdFormat.Hierarchical;
                else
                    IdFormat = DefaultIdFormat;

                // Generate the ID in the appropriate format.  
                if (IdFormat == ActivityIdFormat.W3C)
                    GenerateW3CId();
                else
                    _id = GenerateHierarchicalId();

                SetCurrent(this);
            }
            return this;
        }

        /// <summary>
        /// Stops activity: sets <see cref="Current"/> to <see cref="Parent"/>.
        /// If end time was not set previously, sets <see cref="Duration"/> as a difference between <see cref="DateTime.UtcNow"/> and <see cref="StartTimeUtc"/>
        /// Use <see cref="DiagnosticSource.StopActivity(Activity, object)"/>  to stop activity and write stop event.
        /// </summary>
        /// <seealso cref="DiagnosticSource.StopActivity(Activity, object)"/>
        /// <seealso cref="SetEndTime(DateTime)"/>
        public void Stop()
        {
            if (Id == null)
            {
                NotifyError(new InvalidOperationException("Trying to stop an Activity that was not started"));
                return;
            }

            if (!isFinished)
            {
                isFinished = true;

                if (Duration == TimeSpan.Zero)
                {
                    SetEndTime(GetUtcNow());
                }

                SetCurrent(Parent);
            }
        }

        /* W3C support functionality (see https://w3c.github.io/trace-context) */

        /// <summary>
        /// Holds the W3C 'tracestate' header as a string.   
        /// 
        /// Tracestate is intended to carry information supplemental to trace identity contained 
        /// in traceparent. List of key value pairs carried by tracestate convey information 
        /// about request position in multiple distributed tracing graphs. It is typically used 
        /// by distributed tracing systems and should not be used as a general purpose baggage
        /// as this use may break correlation of a distributed trace.
        /// 
        /// Logically it is just a kind of baggage (if flows just like baggage), but because
        /// it is expected to be special cased (it has its own HTTP header), it is more 
        /// convenient/efficient if it is not lumped in with other baggage.   
        /// </summary>
        public string TraceStateString
        {
            get
            {
                for (var activity = this; activity != null; activity = activity.Parent)
                {
                    var val = activity._traceState;
                    if (val != null)
                        return val;
                }
                return null;
            }
            set
            {
                _traceState = value;
            }
        }

        /// <summary>
        /// If the Activity has the W3C format, this returns the ID for the SPAN part of the Id.  
        /// Otherwise it returns a zero SpanId. 
        /// </summary>
        public ref readonly SpanId SpanId
        {
            get
            {
                if (!_spanIdSet)
                {
                    if (_id != null && IdFormat == ActivityIdFormat.W3C)
                    {
                        _spanId = new SpanId(((ReadOnlySpan<char>)_id).Slice(36, 16));
                        _spanIdSet = true;
                    }
                }
                return ref _spanId;
            }
        }

        /// <summary>
        /// If the Activity has the W3C format, this returns the ID for the TraceId part of the Id.  
        /// Otherwise it returns a zero TraceId. 
        /// </summary>
        public ref readonly TraceId TraceId
        {
            get
            {
                if (!_traceIdSet)
                {
                    if (_id != null && IdFormat == ActivityIdFormat.W3C)
                    {
                        _traceId = new TraceId(((ReadOnlySpan<char>)_id).Slice(3, 32));
                        _traceIdSet = true;
                    }
                }
                return ref _traceId;
            }
        }

        /// <summary>
        /// If the parent Activity ID has the W3C format, this returns the ID for the SpanId part of the ParentId.  
        /// Otherwise it returns a zero SpanId. 
        /// </summary>
        public ref readonly SpanId ParentSpanId
        {
            get
            {
                if (!_parentSpanIdSet && _parentId != null && IsW3CId(_parentId))
                {
                    _parentSpanId = new SpanId(((ReadOnlySpan<char>)_parentId).Slice(36, 16));
                    _parentSpanIdSet = true;
                }
                return ref _parentSpanId;
            }
        }

        /// <summary>
        /// Returns the format for the ID.   
        /// </summary>
        public ActivityIdFormat IdFormat { get; private set; }

        /* static state (configuration) */
        /// <summary>
        /// Activity tries to use the same format for IDs as its parent.
        /// However if the activity has no parent, it has to do something.   
        /// This determines the default format we use.  
        /// </summary>
        public static ActivityIdFormat DefaultIdFormat
        {
            get
            {
                if (s_DefaultIdFormat == ActivityIdFormat.Unknown)
                    s_DefaultIdFormat = ActivityIdFormat.Hierarchical;
                return s_DefaultIdFormat;
            }
            set
            {
                if (!(ActivityIdFormat.Hierarchical <= value && value <= ActivityIdFormat.W3C))
                    throw new ArgumentException($"value must be a valid ActivityIDFormat value");
                s_DefaultIdFormat = value;

            }
        }

        /// <summary>
        /// Normally if the ParentID is defined, the format of that is used to determine the
        /// format used by the Activity.   However if ForceDefaultFormat is set to true, the
        /// ID format will always be the DefaultIdFormat even if the ParentID is define and is
        /// a different format. 
        /// </summary>
        public static bool ForceDefaultIdFormat { get; set; }

        #region private 
        /// <summary>
        /// Returns true if 'id' has the format of a WC3 id see https://w3c.github.io/trace-context
        /// </summary>
        private static bool IsW3CId(string id)
        {
            // A W3CId is  
            //  * 2 chars Version
            //  * 1 char - char
            //  * 32 chars traceId
            //  * 1 char - char 
            //  * 16 chars spanId
            //  * 1 char - char
            //  * 2 chars flags 
            //  = 55 chars (see https://w3c.github.io/trace-context)
            // We require that all non-WC3IDs NOT start with a digit.  
            return id.Length == 55 && char.IsDigit(id[0]);
        }

        /// <summary>
        /// Set the ID (lazily, avoiding strings if possible) to a W3C ID (using the
        /// traceId from the parent if possible 
        /// </summary>
        private void GenerateW3CId()
        {
            // Get the TraceId from the parent or make a new one.  
            if (!_traceIdSet)
            {
                if (Parent != null && Parent.IdFormat == ActivityIdFormat.W3C)
                    _traceId = Parent.TraceId;
                else if (_parentId != null && IsW3CId(_parentId))
                    _traceId = new TraceId(((ReadOnlySpan<char>)_parentId).Slice(3, 32));
                else
                    _traceId = TraceId.NewTraceId();
                _traceIdSet = true;
            }
            // Create a new SpanID. 
            _spanId = SpanId.NewSpanId();
            _spanIdSet = true;
        }

        private static void NotifyError(Exception exception)
        {
            // Throw and catch the exception.  This lets it be seen by the debugger
            // ETW, and other monitoring tools.   However we immediately swallow the
            // exception.   We may wish in the future to allow users to hook this 
            // in other useful ways but for now we simply swallow the exceptions.  
            try
            {
                throw exception;
            }
            catch { }
        }

        /// <summary>
        /// Returns a new ID using the Hierarchical Id 
        /// </summary>
        private string GenerateHierarchicalId()
        {
            string ret;
            if (Parent != null)
            {
                // Normal start within the process
                Debug.Assert(!string.IsNullOrEmpty(Parent.Id));
                ret = AppendSuffix(Parent.Id, Interlocked.Increment(ref Parent._currentChildId).ToString(), '.');
            }
            else if (ParentId != null)
            {
                // Start from outside the process (e.g. incoming HTTP)
                Debug.Assert(ParentId.Length != 0);

                //sanitize external RequestId as it may not be hierarchical. 
                //we cannot update ParentId, we must let it be logged exactly as it was passed.
                string parentId = ParentId[0] == '|' ? ParentId : '|' + ParentId;

                char lastChar = parentId[parentId.Length - 1];
                if (lastChar != '.' && lastChar != '_')
                {
                    parentId += '.';
                }

                ret = AppendSuffix(parentId, Interlocked.Increment(ref s_currentRootId).ToString("x"), '_');
            }
            else
            {
                // A Root Activity (no parent).  
                ret = GenerateRootId();
            }
            // Useful place to place a conditional breakpoint.  
            return ret;
        }

        private string GetRootId(string id)
        {
            // If this is a W3C ID it has the format Version2-TraceId32-SpanId16-Flags2
            // and the root ID is the TraceId.   
            if (IdFormat == ActivityIdFormat.W3C)
                return id.Substring(3, 32);

            //id MAY start with '|' and contain '.'. We return substring between them
            //ParentId MAY NOT have hierarchical structure and we don't know if initially rootId was started with '|',
            //so we must NOT include first '|' to allow mixed hierarchical and non-hierarchical request id scenarios
            int rootEnd = id.IndexOf('.');
            if (rootEnd < 0)
                rootEnd = id.Length;
            int rootStart = id[0] == '|' ? 1 : 0;
            return id.Substring(rootStart, rootEnd - rootStart);
        }

        private string AppendSuffix(string parentId, string suffix, char delimiter)
        {
#if DEBUG
            suffix = OperationName.Replace('.', '-') + "-" + suffix;
#endif
            if (parentId.Length + suffix.Length < RequestIdMaxLength)
                return parentId + suffix + delimiter;

            //Id overflow:
            //find position in RequestId to trim
            int trimPosition = RequestIdMaxLength - 9; // overflow suffix + delimiter length is 9
            while (trimPosition > 1)
            {
                if (parentId[trimPosition - 1] == '.' || parentId[trimPosition - 1] == '_')
                    break;
                trimPosition--;
            }

            //ParentId is not valid Request-Id, let's generate proper one.
            if (trimPosition == 1)
                return GenerateRootId();

            //generate overflow suffix
            string overflowSuffix = ((int)GetRandomNumber()).ToString("x8");
            return parentId.Substring(0, trimPosition) + overflowSuffix + '#';
        }

        private string GenerateRootId()
        {
            // It is important that the part that changes frequently be first, because
            // many hash functions don't 'randomize' the tail of a string.   This makes
            // sampling based on the hash produce poor samples.
            return '|' + Interlocked.Increment(ref s_currentRootId).ToString("x") + s_uniqSuffix;
        }
#if ALLOW_PARTIALLY_TRUSTED_CALLERS
        [SecuritySafeCritical]
#endif
        private static unsafe long GetRandomNumber()
        {
            // Use the first 8 bytes of the GUID as a random number.  
            Guid g = Guid.NewGuid();
            return *((long*)&g);
        }

        private static bool ValidateSetCurrent(Activity activity)
        {
            bool canSet = activity == null || (activity.Id != null && !activity.isFinished);
            if (!canSet)
            {
                NotifyError(new InvalidOperationException("Trying to set an Activity that is not running"));
            }

            return canSet;
        }

        private string _rootId;
        private int _currentChildId;  // A unique number for all children of this activity.  

        // Used to generate an ID it represents the machine and process we are in.  
        private static readonly string s_uniqSuffix = "-" + GetRandomNumber().ToString("x") + ".";

        //A unique number inside the appdomain, randomized between appdomains. 
        //Int gives enough randomization and keeps hex-encoded s_currentRootId 8 chars long for most applications
        private static long s_currentRootId = (uint)GetRandomNumber();

        private const int RequestIdMaxLength = 1024;

        /// <summary>
        /// Having our own key-value linked list allows us to be more efficient  
        /// </summary>
        private partial class KeyValueListNode
        {
            public KeyValuePair<string, string> keyValue;
            public KeyValueListNode Next;
        }

        private static ActivityIdFormat s_DefaultIdFormat;

        private KeyValueListNode _tags;
        private KeyValueListNode _baggage;
        private string _traceState;

        // State associated with ID.  It can be represented by a string or by TraceId, SpanId.  
        private string _id;
        private TraceId _traceId;
        private bool _traceIdSet;
        private SpanId _spanId;
        private bool _spanIdSet;

        // State associated with ParentId.  It can be represented by a string or by TraceId, SpanId.  
        private string _parentId;
        private SpanId _parentSpanId;
        private bool _parentSpanIdSet;

        private bool isFinished;
        #endregion // private
    }

    /// <summary>
    /// The possibilities for the format of the ID
    /// </summary>
    public enum ActivityIdFormat : byte
    {
        Unknown,      // ID format is not known.     
        Hierarchical, //|XXXX.XX.X_X ... see https://github.com/dotnet/corefx/blob/master/src/System.Diagnostics.DiagnosticSource/src/ActivityUserGuide.md#id-format
        W3C,          // 00-XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX-XXXXXXXXXXXXXXXX-XX see https://w3c.github.io/trace-context/
    };

    /// <summary>
    /// A TraceId is the format the W3C standard requires for its ID for the entire trace. 
    /// It represents 16 binary bytes of information, typically displayed as 32 characters
    /// of Hexadecimal.  A TraceId is a STRUCT, and does contain the 16 bytes of binary information
    /// so there is value in passing it by reference.   It does know how to convert to and
    /// from its Hexadecimal string representation, tries to avoid changing formats until
    /// it has to, and caches the string representation after it was created.   
    /// It is mostly useful as an exchange type.  
    /// </summary>
    public unsafe struct TraceId
    {
        /// <summary>
        /// Creates a TraceId from a 16 byte span 'idBytes'.   The bytes are copied.  
        /// </summary>
        public TraceId(ReadOnlySpan<byte> idData, bool isUtf8Chars = false)
        {
            _id1 = 0;
            _id2 = 0;
            _asHexString = null;
            fixed (ulong* idPtr = &_id1)
            {
                var outBuff = new Span<byte>(idPtr, sizeof(ulong) * 2);
                if (isUtf8Chars)
                {
                    if (idData.Length != 32)
                        throw new ArgumentOutOfRangeException(nameof(idData));
                    TraceId.SetSpanFromHexUtf8Chars(outBuff, idData);
                }
                else
                {
                    if (idData.Length != 16)
                        throw new ArgumentOutOfRangeException(nameof(idData));
                    idData.CopyTo(outBuff);
                }
            }
        }

        public TraceId(ReadOnlySpan<char> idData)
        {
            _id1 = 0;
            _id2 = 0;
            _asHexString = null;
            fixed (ulong* idPtr = &_id1)
            {
                if (idData.Length != 32)
                    throw new ArgumentOutOfRangeException(nameof(idData));
                TraceId.SetSpanFromHexChars(new Span<byte>(idPtr, sizeof(ulong) * 2), idData);
            }
        }

        /// <summary>
        /// Create a new TraceId with at random number in it (very likely to be unique)
        /// </summary>
        public static TraceId NewTraceId()
        {
            TraceId ret = new TraceId();
            SetToRandomBytes(new Span<byte>(&ret._id1, sizeof(ulong) * 2));
            return ret;
        }

        /// <summary>
        /// Return the bytes of the ID.  Its length is always 16
        /// </summary>
        public ReadOnlySpan<byte> AsBytes
        {
            get
            {
                fixed (ulong* idPtr = &_id1)
                    return new ReadOnlySpan<byte>(idPtr, sizeof(ulong) * 2);
            }
        }

        /// <summary>
        /// Returns the TraceId as a 32 character hexadecimal string.  
        /// </summary>
        public string AsHexString
        {
            get
            {
                if (_asHexString == null)
                    _asHexString = SpanToHexString(AsBytes);
                return _asHexString;
            }
        }

        /// <summary>
        /// Returns the TraceId as a 32 character hexadecimal string.  
        /// </summary>
        public override string ToString() => AsHexString;

        #region private
        internal static void SetToRandomBytes(Span<byte> outBytes)
        {
            Debug.Assert(outBytes.Length <= sizeof(Guid));     // Guid is 16 bytes, and so is TraceId 
            Guid guid = Guid.NewGuid();
            ReadOnlySpan<byte> guidBytes = new ReadOnlySpan<byte>(&guid, sizeof(Guid));
            guidBytes.Slice(0, outBytes.Length).CopyTo(outBytes);
        }

        internal static string SpanToHexString(ReadOnlySpan<byte> bytes)
        {
            // TODO replace with ValueStringBuilder when available.  
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bytes)
                sb.Append(BinaryToHexDigit(b >> 4)).Append(BinaryToHexDigit(b));
            return sb.ToString();
        }

        internal static void SetSpanFromHexUtf8Chars(Span<byte> outBytes, ReadOnlySpan<byte> idData)
        {
            Debug.Assert(outBytes.Length * 2 == idData.Length);
            for (int i = 0; i < outBytes.Length; i++)
                outBytes[i] = HexByteFromChars((char)idData[i * 2], (char)idData[i * 2 + 1]);
        }

        internal static void SetSpanFromHexChars(Span<byte> outBytes, ReadOnlySpan<char> charData)
        {
            Debug.Assert(outBytes.Length * 2 == charData.Length);
            for (int i = 0; i < outBytes.Length; i++)
                outBytes[i] = HexByteFromChars(charData[i * 2], charData[i * 2 + 1]);
        }
        internal static byte HexByteFromChars(char char1, char char2)
        {
            return (byte)(HexDigitToBinary(char1) * 16 + HexDigitToBinary(char2));
        }

        internal static byte HexDigitToBinary(char c)
        {
            if (Char.IsDigit(c))
                return (byte)(c - '0');

            if ('A' <= c && c <= 'F')
                return (byte)(c - ('A' - 10));

            if ('a' <= c && c <= 'f')
                return (byte)(c - ('a' - 10));

            throw new ArgumentOutOfRangeException("idData");
        }

        internal static char BinaryToHexDigit(int val)
        {
            val &= 0xF;
            if (val <= 9)
                return (char)('0' + val);
            return (char)(('A' - 10) + val);
        }

        ulong _id1;
        ulong _id2;
        string _asHexString;  // ultimately change to object, and allow several representations based on type.  
        #endregion
    }

    /// <summary>
    /// A SpanId is the format the W3C standard requires for its ID for a single span in a trace.  
    /// It represents 8 binary bytes of information, typically displayed as 16 characters
    /// of Hexadecimal.  A SpanId is a STRUCT, and does contain the 8 bytes of binary information
    /// so there is value in passing it by reference.  It does know how to convert to and
    /// from its Hexadecimal string representation, tries to avoid changing formats until
    /// it has to, and caches the string representation after it was created.   
    /// It is mostly useful as an exchange type.  
    /// </summary>
    public unsafe struct SpanId
    {
        /// <summary>
        /// Creates a new SpanId from a given set of bytes 'idData'   idData 
        /// can either be an 8 byte Span of bytes, or a 16 byte Span of Utf8 characters
        /// and 'isUtf8Chars' indicates which of these it is.   
        /// </summary>
        public SpanId(ReadOnlySpan<byte> idData, bool isUtf8Chars = false)
        {
            _id1 = 0;
            _asHexString = null;
            fixed (ulong* idPtr = &_id1)
            {
                var outBuff = new Span<byte>(idPtr, sizeof(ulong));
                if (isUtf8Chars)
                {
                    if (idData.Length != 16)
                        throw new ArgumentOutOfRangeException(nameof(idData));
                    TraceId.SetSpanFromHexUtf8Chars(outBuff, idData);
                }
                else
                {
                    if (idData.Length != 8)
                        throw new ArgumentOutOfRangeException(nameof(idData));
                    idData.CopyTo(outBuff);
                }
            }
        }

        /// <summary>
        /// Creates a new SpanId from the give characters in 'idData'   IdData
        /// must be exactly 16 bytes long and contain only Hex digits.   
        /// </summary>
        /// <param name="idData"></param>
        public SpanId(ReadOnlySpan<char> idData)
        {
            _id1 = 0;
            _asHexString = null;
            fixed (ulong* idPtr = &_id1)
            {
                if (idData.Length != 16)
                    throw new ArgumentOutOfRangeException(nameof(idData));
                TraceId.SetSpanFromHexChars(new Span<byte>(idPtr, sizeof(ulong)), idData);
            }
        }

        /// <summary>
        /// Create a new SpanId with at random number in it (very likely to be unique)
        /// </summary>
        public static SpanId NewSpanId()
        {
            SpanId ret = new SpanId();
            TraceId.SetToRandomBytes(new Span<byte>(&ret._id1, sizeof(ulong)));
            return ret;
        }

        /// <summary>
        /// Return the bytes of the ID.  Its length is always 8
        /// </summary>
        public ReadOnlySpan<byte> AsBytes
        {
            get
            {
                fixed (ulong* idPtr = &_id1)
                    return new ReadOnlySpan<byte>(idPtr, sizeof(ulong));
            }
        }

        /// <summary>
        /// Returns the TraceId as a 16 character hexadecimal string.  
        /// </summary>
        /// <returns></returns>
        public string AsHexString
        {
            get
            {
                if (_asHexString == null)
                    _asHexString = TraceId.SpanToHexString(AsBytes);
                return _asHexString;
            }
        }

        /// <summary>
        /// Returns SpanId as a hex string. 
        /// </summary>
        public override string ToString() => AsHexString;

        #region private
        ulong _id1;
        string _asHexString;   // ultimately change to object, and allow several representations based on type.   
        #endregion
    }
}

#if false

        /// <summary>
        /// Copies the 16 byte binary trace Id int 'outputBuffer'. 
        /// </summary>
        public void CopyTo(Span<byte> outputBuffer)
        {
            if (_asHexString != null && _id1 == 0 && _id2 == 0)
            {
                UInt64.TryParse(_asHexString, out _id1);
                UInt64.TryParse(_asHexString.Substring(8), out _id2);
            }
            fixed (ulong* idBytes = &_id1)
                new ReadOnlySpan<byte>(idBytes, sizeof(ulong) * 2).CopyTo(outputBuffer);
        }

        /// <summary>
        /// Creates a SpanId from a long id.  
        /// </summary>
        /// <param name="id"></param>
        public SpanId(long id)
        {
            _id1 = id;
            _asHexString = null;
        }

        /// <summary>
        /// Returns the SpanId as a 8 byte long.  
        /// </summary>
        public long AsLong
        {
            get
            {
                if (_asHexString != null && _id1 == 0)
                    Int64.TryParse(_asHexString, out _id1);
                return _id1;
            }
        }
#endif

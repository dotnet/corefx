// This is auto generated file. Please don’t modify manually.
// The file is generated as part of the build through the ResourceGenerator tool 
// which takes the project resx resource file and generated this source code file.
// By default the tool will use Resources\Strings.resx but projects can customize
// that by overriding the StringResourcesPath property group.
namespace System
{
    internal static partial class SR
    {
#pragma warning disable 0414
        private const string s_resourcesName = "System.Linq.Parallel.resources"; // assembly Name + .resources
#pragma warning restore 0414

#if !DEBUGRESOURCES
        internal static string EmptyEnumerable {
              get { return SR.GetResourceString("EmptyEnumerable", null); }
        }
        internal static string MoreThanOneElement {
              get { return SR.GetResourceString("MoreThanOneElement", null); }
        }
        internal static string MoreThanOneMatch {
              get { return SR.GetResourceString("MoreThanOneMatch", null); }
        }
        internal static string NoElements {
              get { return SR.GetResourceString("NoElements", null); }
        }
        internal static string NoMatch {
              get { return SR.GetResourceString("NoMatch", null); }
        }
        internal static string ParallelPartitionable_NullReturn {
              get { return SR.GetResourceString("ParallelPartitionable_NullReturn", null); }
        }
        internal static string ParallelPartitionable_IncorretElementCount {
              get { return SR.GetResourceString("ParallelPartitionable_IncorretElementCount", null); }
        }
        internal static string ParallelPartitionable_NullElement {
              get { return SR.GetResourceString("ParallelPartitionable_NullElement", null); }
        }
        internal static string PLINQ_CommonEnumerator_Current_NotStarted {
              get { return SR.GetResourceString("PLINQ_CommonEnumerator_Current_NotStarted", null); }
        }
        internal static string PLINQ_ExternalCancellationRequested {
              get { return SR.GetResourceString("PLINQ_ExternalCancellationRequested", null); }
        }
        internal static string PLINQ_DisposeRequested {
              get { return SR.GetResourceString("PLINQ_DisposeRequested", null); }
        }
        internal static string ParallelQuery_DuplicateTaskScheduler {
              get { return SR.GetResourceString("ParallelQuery_DuplicateTaskScheduler", null); }
        }
        internal static string ParallelQuery_DuplicateDOP {
              get { return SR.GetResourceString("ParallelQuery_DuplicateDOP", null); }
        }
        internal static string ParallelQuery_DuplicateExecutionMode {
              get { return SR.GetResourceString("ParallelQuery_DuplicateExecutionMode", null); }
        }
        internal static string PartitionerQueryOperator_NullPartitionList {
              get { return SR.GetResourceString("PartitionerQueryOperator_NullPartitionList", null); }
        }
        internal static string PartitionerQueryOperator_WrongNumberOfPartitions {
              get { return SR.GetResourceString("PartitionerQueryOperator_WrongNumberOfPartitions", null); }
        }
        internal static string PartitionerQueryOperator_NullPartition {
              get { return SR.GetResourceString("PartitionerQueryOperator_NullPartition", null); }
        }
        internal static string ParallelQuery_DuplicateWithCancellation {
              get { return SR.GetResourceString("ParallelQuery_DuplicateWithCancellation", null); }
        }
        internal static string ParallelQuery_DuplicateMergeOptions {
              get { return SR.GetResourceString("ParallelQuery_DuplicateMergeOptions", null); }
        }
        internal static string PLINQ_EnumerationPreviouslyFailed {
              get { return SR.GetResourceString("PLINQ_EnumerationPreviouslyFailed", null); }
        }
        internal static string ParallelQuery_PartitionerNotOrderable {
              get { return SR.GetResourceString("ParallelQuery_PartitionerNotOrderable", null); }
        }
        internal static string ParallelQuery_InvalidAsOrderedCall {
              get { return SR.GetResourceString("ParallelQuery_InvalidAsOrderedCall", null); }
        }
        internal static string ParallelQuery_InvalidNonGenericAsOrderedCall {
              get { return SR.GetResourceString("ParallelQuery_InvalidNonGenericAsOrderedCall", null); }
        }
        internal static string ParallelEnumerable_BinaryOpMustUseAsParallel {
              get { return SR.GetResourceString("ParallelEnumerable_BinaryOpMustUseAsParallel", null); }
        }
        internal static string ParallelEnumerable_WithCancellation_TokenSourceDisposed {
              get { return SR.GetResourceString("ParallelEnumerable_WithCancellation_TokenSourceDisposed", null); }
        }
        internal static string ParallelEnumerable_WithQueryExecutionMode_InvalidMode {
              get { return SR.GetResourceString("ParallelEnumerable_WithQueryExecutionMode_InvalidMode", null); }
        }
        internal static string ParallelEnumerable_WithMergeOptions_InvalidOptions {
              get { return SR.GetResourceString("ParallelEnumerable_WithMergeOptions_InvalidOptions", null); }
        }
#else
        internal static string EmptyEnumerable {
              get { return SR.GetResourceString("EmptyEnumerable", @"Enumeration yielded no results"); }
        }
        internal static string MoreThanOneElement {
              get { return SR.GetResourceString("MoreThanOneElement", @"Sequence contains more than one element"); }
        }
        internal static string MoreThanOneMatch {
              get { return SR.GetResourceString("MoreThanOneMatch", @"Sequence contains more than one matching element"); }
        }
        internal static string NoElements {
              get { return SR.GetResourceString("NoElements", @"Sequence contains no elements"); }
        }
        internal static string NoMatch {
              get { return SR.GetResourceString("NoMatch", @"Sequence contains no matching element"); }
        }
        internal static string ParallelPartitionable_NullReturn {
              get { return SR.GetResourceString("ParallelPartitionable_NullReturn", @"The return value must not be null."); }
        }
        internal static string ParallelPartitionable_IncorretElementCount {
              get { return SR.GetResourceString("ParallelPartitionable_IncorretElementCount", @"The returned array's length must equal the number of partitions requested."); }
        }
        internal static string ParallelPartitionable_NullElement {
              get { return SR.GetResourceString("ParallelPartitionable_NullElement", @"Elements returned must not be null."); }
        }
        internal static string PLINQ_CommonEnumerator_Current_NotStarted {
              get { return SR.GetResourceString("PLINQ_CommonEnumerator_Current_NotStarted", @"Enumeration has not started. MoveNext must be called to initiate enumeration."); }
        }
        internal static string PLINQ_ExternalCancellationRequested {
              get { return SR.GetResourceString("PLINQ_ExternalCancellationRequested", @"The query has been canceled via the token supplied to WithCancellation."); }
        }
        internal static string PLINQ_DisposeRequested {
              get { return SR.GetResourceString("PLINQ_DisposeRequested", @"The query enumerator has been disposed."); }
        }
        internal static string ParallelQuery_DuplicateTaskScheduler {
              get { return SR.GetResourceString("ParallelQuery_DuplicateTaskScheduler", @"The WithTaskScheduler operator may be used at most once in a query."); }
        }
        internal static string ParallelQuery_DuplicateDOP {
              get { return SR.GetResourceString("ParallelQuery_DuplicateDOP", @"The WithDegreeOfParallelism operator may be used at most once in a query."); }
        }
        internal static string ParallelQuery_DuplicateExecutionMode {
              get { return SR.GetResourceString("ParallelQuery_DuplicateExecutionMode", @"The WithExecutionMode operator may be used at most once in a query."); }
        }
        internal static string PartitionerQueryOperator_NullPartitionList {
              get { return SR.GetResourceString("PartitionerQueryOperator_NullPartitionList", @"Partitioner returned null instead of a list of partitions."); }
        }
        internal static string PartitionerQueryOperator_WrongNumberOfPartitions {
              get { return SR.GetResourceString("PartitionerQueryOperator_WrongNumberOfPartitions", @"Partitioner returned a wrong number of partitions."); }
        }
        internal static string PartitionerQueryOperator_NullPartition {
              get { return SR.GetResourceString("PartitionerQueryOperator_NullPartition", @"Partitioner returned a null partition."); }
        }
        internal static string ParallelQuery_DuplicateWithCancellation {
              get { return SR.GetResourceString("ParallelQuery_DuplicateWithCancellation", @"The WithCancellation operator may by used at most once in a query."); }
        }
        internal static string ParallelQuery_DuplicateMergeOptions {
              get { return SR.GetResourceString("ParallelQuery_DuplicateMergeOptions", @"The WithMergeOptions operator may be used at most once in a query."); }
        }
        internal static string PLINQ_EnumerationPreviouslyFailed {
              get { return SR.GetResourceString("PLINQ_EnumerationPreviouslyFailed", @"The query enumerator previously threw an exception."); }
        }
        internal static string ParallelQuery_PartitionerNotOrderable {
              get { return SR.GetResourceString("ParallelQuery_PartitionerNotOrderable", @"AsOrdered may not be used with a partitioner that is not orderable."); }
        }
        internal static string ParallelQuery_InvalidAsOrderedCall {
              get { return SR.GetResourceString("ParallelQuery_InvalidAsOrderedCall", @"AsOrdered may only be called on the result of AsParallel, ParallelEnumerable.Range, or ParallelEnumerable.Repeat."); }
        }
        internal static string ParallelQuery_InvalidNonGenericAsOrderedCall {
              get { return SR.GetResourceString("ParallelQuery_InvalidNonGenericAsOrderedCall", @"Non-generic AsOrdered may only be called on the result of the non-generic AsParallel."); }
        }
        internal static string ParallelEnumerable_BinaryOpMustUseAsParallel {
              get { return SR.GetResourceString("ParallelEnumerable_BinaryOpMustUseAsParallel", @"The second data source of a binary operator must be of type System.Linq.ParallelQuery<T> rather than System.Collections.Generic.IEnumerable<T>. To fix this problem, use the AsParallel() extension method to convert the right data source to System.Linq.ParallelQuery<T>."); }
        }
        internal static string ParallelEnumerable_WithCancellation_TokenSourceDisposed {
              get { return SR.GetResourceString("ParallelEnumerable_WithCancellation_TokenSourceDisposed", @"The CancellationTokenSource associated with this CancellationToken has been disposed."); }
        }
        internal static string ParallelEnumerable_WithQueryExecutionMode_InvalidMode {
              get { return SR.GetResourceString("ParallelEnumerable_WithQueryExecutionMode_InvalidMode", @"The executionMode argument contains an invalid value."); }
        }
        internal static string ParallelEnumerable_WithMergeOptions_InvalidOptions {
              get { return SR.GetResourceString("ParallelEnumerable_WithMergeOptions_InvalidOptions", @"The mergeOptions argument contains an invalid value."); }
        }

#endif
    }
}

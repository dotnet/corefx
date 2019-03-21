// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Security;
using System.Reflection;
using System.Security.Permissions;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

[module: BidIdentity("System.Data.1")]
[module: BidMetaText(":FormatControl: InstanceID='' ")]
[module: BidMetaText("<CountHint> Trace=1200; Scope=250;")]
[module: BidMetaText("<Alias>  ds = System.Data;"
                            + "comm = System.Data.Common;"
                            + "odbc = System.Data.Odbc;"
                            + "oledb= System.Data.OleDb;"
                            + "prov = System.Data.ProviderBase;"
                            + "sc   = System.Data.Sql;"
                            + "sql  = System.Data.SqlClient;" 
                            + "cqt  = System.Data.Common.CommandTrees;" 
                            + "cqti = System.Data.Common.CommandTrees.Internal;" 
                            + "esql = System.Data.Common.EntitySql;"
                            + "ec   = System.Data.EntityClient;"
                            + "dobj = System.Data.Objects;"
                            + "md   = System.Data.Metadata;"
                            + "ra   = System.Data.Query.ResultAssembly;"
                            + "pc   = System.Data.Query.PlanCompiler;"
	                    + "iqt  = System.Data.Query.InternalTrees;"
     		    	    + "mp   = System.Data.Mapping;"
		    	    + "upd  = System.Data.Mapping.Update;"
		    	    + "vgen = System.Data.Mapping.ViewGeneration;"
)]

//
//  DbConnectionPool.cs: const Bid.ApiGroup PoolerTracePoints
//
[module: BidMetaText("<ApiGroup|ProviderBase|CPOOL> 0x00001000: Connection Pooling")]

//
//  SqlDependency.cs: const Bid.ApiGroup NotificationsTracePoints
//
[module: BidMetaText("<ApiGroup|SqlClient|DEP> 0x00002000: SqlDependency Notifications")]

//
//  System\Data\Query\Bridge\IteratorSource.cs:        internal const Bid.ApiGroup ResultAssemblyTracePoints
//
[module: BidMetaText("<ApiGroup|System.Data.Query|RA> 0x00004000: Result Assembly")]

//
//  System\Data\Query\PlanCompiler\PlanCompiler.cs:        internal const Bid.ApiGroup PlanCompilerTracePoints
//
[module: BidMetaText("<ApiGroup|System.Data.Query.PlanCompiler|PC> 0x00008000: Plan Compilation")]

//
//  System\Data\Common\ActivityCorrelator.cs:        internal const Bid.ApiGroup Correlation
//
[module: BidMetaText("<ApiGroup|SqlClient|Correlation> 0x00040000: Correlation")]


internal static partial class Bid
{
    private const string dllName = "System.Data.dll";

    //
    //  Manually added wrappers
    //
    [BidMethod]
    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    internal static void PoolerTrace(string fmtPrintfW, System.Int32 a1) {
        if ((modFlags & System.Data.ProviderBase.DbConnectionPool.PoolerTracePoints) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1);
    }

    [BidMethod]
    internal static void PoolerTrace(string fmtPrintfW, System.Int32 a1, System.Int32 a2) {
        if ((modFlags & System.Data.ProviderBase.DbConnectionPool.PoolerTracePoints) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1,a2);
    }

    [BidMethod]
    internal static void PoolerTrace(string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.Int32 a3) {
        if ((modFlags & System.Data.ProviderBase.DbConnectionPool.PoolerTracePoints) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1,a2,a3);
    }

    [BidMethod]
    internal static void PoolerTrace(string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.Int32 a3, System.Int32 a4) {
        if ((modFlags & System.Data.ProviderBase.DbConnectionPool.PoolerTracePoints) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1,a2,a3,a4);
    }

    [BidMethod]
    internal static void PoolerTrace(
            string fmtPrintfW, 
            System.Int32 a1, 
            [BidArgumentType(typeof(String))] System.Exception a2) {
        if ((modFlags & System.Data.ProviderBase.DbConnectionPool.PoolerTracePoints) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1,a2.ToString());
    }

    [BidMethod]
    internal static void PoolerScopeEnter(out IntPtr hScp, string fmtPrintfW, System.Int32 a1) {
        if ((modFlags & System.Data.ProviderBase.DbConnectionPool.PoolerTracePoints) != 0  &&  modID != NoData) {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW,a1);
        } else {
            hScp = NoData;
        }
    }

    [BidMethod]
    internal static void NotificationsScopeEnter(out IntPtr hScp, string fmtPrintfW, string fmtPrintfW2) {
        if ((modFlags & System.Data.SqlClient.SqlDependency.NotificationsTracePoints) != 0  &&  modID != NoData) {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, fmtPrintfW2);
        } else {
            hScp = NoData;
        }
    }

    [BidMethod]
    internal static void NotificationsScopeEnter(out IntPtr hScp, string fmtPrintfW, System.Int32 a1) {
        if ((modFlags & System.Data.SqlClient.SqlDependency.NotificationsTracePoints) != 0  &&  modID != NoData) {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1);
        } else {
            hScp = NoData;
        }
    }

    [BidMethod]
    internal static void NotificationsScopeEnter(out IntPtr hScp, string fmtPrintfW, string fmtPrintfW2, string fmtPrintfW3) {
        if ((modFlags & System.Data.SqlClient.SqlDependency.NotificationsTracePoints) != 0  &&  modID != NoData) {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, fmtPrintfW2, fmtPrintfW3);
        } else {
            hScp = NoData;
        }
    }

    [BidMethod]
    internal static void NotificationsScopeEnter(out IntPtr hScp, string fmtPrintfW, System.Int32 a1, string fmtPrintfW2) {
        if ((modFlags & System.Data.SqlClient.SqlDependency.NotificationsTracePoints) != 0  &&  modID != NoData) {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1, fmtPrintfW2);
        } else {
            hScp = NoData;
        }
    }

    [BidMethod]
    internal static void NotificationsScopeEnter(out IntPtr hScp, string fmtPrintfW, System.Int32 a1, System.Int32 a2) {
        if ((modFlags & System.Data.SqlClient.SqlDependency.NotificationsTracePoints) != 0  &&  modID != NoData) {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1, a2);
        } else {
            hScp = NoData;
        }
    }

    [BidMethod]
    internal static void NotificationsScopeEnter(out IntPtr hScp, string fmtPrintfW, string fmtPrintfW2, string fmtPrintfW3, string fmtPrintfW4) {
        if ((modFlags & System.Data.SqlClient.SqlDependency.NotificationsTracePoints) != 0  &&  modID != NoData) {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, fmtPrintfW2, fmtPrintfW3, fmtPrintfW4);
        } else {
            hScp = NoData;
        }
    }

    [BidMethod]
    internal static void NotificationsScopeEnter(out IntPtr hScp, string fmtPrintfW, System.Int32 a1, string fmtPrintfW2, System.Int32 a2) {
        if ((modFlags & System.Data.SqlClient.SqlDependency.NotificationsTracePoints) != 0  &&  modID != NoData) {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1, fmtPrintfW2, a2);
        } else {
            hScp = NoData;
        }
    }

    [BidMethod]
    internal static void NotificationsScopeEnter(out IntPtr hScp, string fmtPrintfW, System.Int32 a1, string fmtPrintfW2, string fmtPrintfW3, System.Int32 a4) {
        if ((modFlags & System.Data.SqlClient.SqlDependency.NotificationsTracePoints) != 0  &&  modID != NoData) {
            NativeMethods.ScopeEnter(modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW, a1, fmtPrintfW2, fmtPrintfW3, a4);
        } else {
            hScp = NoData;
        }
    }

    [BidMethod]
    internal static void NotificationsTrace(string fmtPrintfW) {
        if ((modFlags & System.Data.SqlClient.SqlDependency.NotificationsTracePoints) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW);
    }

    [BidMethod]
    internal static void NotificationsTrace(string fmtPrintfW, string fmtPrintfW2) {
        if ((modFlags & System.Data.SqlClient.SqlDependency.NotificationsTracePoints) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, fmtPrintfW2);
    }

    [BidMethod]
    internal static void NotificationsTrace(string fmtPrintfW, System.Int32 a1) {
        if ((modFlags & System.Data.SqlClient.SqlDependency.NotificationsTracePoints) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1);
    }

    [BidMethod]
    internal static void NotificationsTrace(string fmtPrintfW, System.Boolean a1) {
        if ((modFlags & System.Data.SqlClient.SqlDependency.NotificationsTracePoints) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1);
    }

    [BidMethod]
    internal static void NotificationsTrace(string fmtPrintfW, string fmtPrintfW2, System.Int32 a1) {
        if ((modFlags & System.Data.SqlClient.SqlDependency.NotificationsTracePoints) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, fmtPrintfW2, a1);
    }

    [BidMethod]
    internal static void NotificationsTrace(string fmtPrintfW, System.Int32 a1, string fmtPrintfW2) {
        if ((modFlags & System.Data.SqlClient.SqlDependency.NotificationsTracePoints) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, fmtPrintfW2);
    }

    [BidMethod]
    internal static void NotificationsTrace(string fmtPrintfW, System.Int32 a1, System.Int32 a2) {
        if ((modFlags & System.Data.SqlClient.SqlDependency.NotificationsTracePoints) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2);
    }

    [BidMethod]
    internal static void NotificationsTrace(string fmtPrintfW, System.Int32 a1, System.Boolean a2) {
        if ((modFlags & System.Data.SqlClient.SqlDependency.NotificationsTracePoints) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2);
    }

    [BidMethod]
    internal static void NotificationsTrace(string fmtPrintfW, System.String a1, System.String a2) {
        if ((modFlags & System.Data.SqlClient.SqlDependency.NotificationsTracePoints) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2);
    }

    [BidMethod]
    internal static void NotificationsTrace(string fmtPrintfW, string fmtPrintfW2, string fmtPrintfW3, System.Int32 a1) {
        if ((modFlags & System.Data.SqlClient.SqlDependency.NotificationsTracePoints) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, fmtPrintfW2, fmtPrintfW3, a1);
    }

    [BidMethod]
    internal static void NotificationsTrace(string fmtPrintfW, System.Boolean a1, string fmtPrintfW2, string fmtPrintfW3, string fmtPrintfW4) {
        if ((modFlags & System.Data.SqlClient.SqlDependency.NotificationsTracePoints) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, fmtPrintfW2, fmtPrintfW3, fmtPrintfW4);
    }

    [BidMethod]
    internal static void NotificationsTrace(string fmtPrintfW, System.Int32 a1, string fmtPrintfW2, string fmtPrintfW3, string fmtPrintfW4) {
        if ((modFlags & System.Data.SqlClient.SqlDependency.NotificationsTracePoints) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, fmtPrintfW2, fmtPrintfW3, fmtPrintfW4);
    }

    [BidMethod]
    [BidArgumentType(typeof(string))] // format string should have a string spec (%ls) for an Activity ID argument as last
    internal static void CorrelationTrace(string fmtPrintfW, System.Int32 a1) {
        if ((modFlags & System.Data.Common.ActivityCorrelator.CorrelationTracePoints) != 0
            && (modFlags & Bid.ApiGroup.Trace) != 0 && modID != NoData) {
            System.Data.Common.ActivityCorrelator.ActivityId actId = System.Data.Common.ActivityCorrelator.Next();
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, actId.ToString());
        }
    }

    [BidMethod]
    [BidArgumentType(typeof(string))] // format string should have a string spec (%ls) for an Activity ID argument as last
    internal static void CorrelationTrace(string fmtPrintfW)
    {
        if ((modFlags & System.Data.Common.ActivityCorrelator.CorrelationTracePoints) != 0
            && (modFlags & Bid.ApiGroup.Trace) != 0 && modID != NoData) {
            System.Data.Common.ActivityCorrelator.ActivityId actId = System.Data.Common.ActivityCorrelator.Next();
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, actId.ToString());
        }
    }

    [BidMethod]
    [BidArgumentType(typeof(string))] // format string should have a string spec (%ls) for an Activity ID argument as last
    internal static void CorrelationTrace(string fmtPrintfW, System.Int32 a1, System.Int32 a2) {
        if ((modFlags & System.Data.Common.ActivityCorrelator.CorrelationTracePoints) != 0
            && (modFlags & Bid.ApiGroup.Trace) != 0 && modID != NoData) {
            System.Data.Common.ActivityCorrelator.ActivityId actId = System.Data.Common.ActivityCorrelator.Next();
            NativeMethods.Trace(modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1, a2, actId.ToString());
        }
    }


    //
    //  Manually edited wrappers
    //
    [BidMethod]
    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    internal static void TraceSqlReturn(
        string fmtPrintfW, 
        [BidArgumentType(typeof(System.Int32))] System.Data.Odbc.ODBC32.RetCode a1) {
        if (((System.Data.Odbc.ODBC32.RetCode.SUCCESS != a1) ||  (modFlags & ApiGroup.StatusOk) != 0) &&  (modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, (int)(short)a1);
    }

    [BidMethod]
    internal static void TraceSqlReturn(
        string fmtPrintfW,
        [BidArgumentType(typeof(System.Int32))] System.Data.Odbc.ODBC32.RetCode a1, 
        string a2) {
        if (((System.Data.Odbc.ODBC32.RetCode.SUCCESS != a1) ||  (modFlags & ApiGroup.StatusOk) != 0) &&  (modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, (int)(short)a1, a2);
    }

    [BidMethod]
    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    internal static void Trace(string fmtPrintfW, System.Data.OleDb.OleDbHResult a1) { // TODO: rename to TraceHResult
        if (((System.Data.OleDb.OleDbHResult.S_OK != a1) ||  (modFlags & ApiGroup.StatusOk) != 0) &&  (modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, (int)a1);
    }

    [BidMethod]
    internal static void Trace(string fmtPrintfW, System.Data.OleDb.OleDbHResult a1, System.String a2) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, (int)a1,a2);
    }

    [BidMethod]
    internal static void Trace(string fmtPrintfW, System.Data.OleDb.OleDbHResult a1, System.IntPtr a2) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, (int)a1,a2);
    }

    [BidMethod]
    internal static void Trace(string fmtPrintfW, System.Data.OleDb.OleDbHResult a1, System.Int32 a2) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, (int)a1,a2);
    }


    [BidMethod]
    internal static void Trace(string fmtPrintfW, System.String a1, System.String a2) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW,a1,a2);
    }

    [BidMethod]
    internal static void Trace(string fmtPrintfW, System.Int32 a1, System.String a2, System.Boolean a3) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1,a2,a3);
    }

    [BidMethod]
    internal static void Trace(string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.String a3, System.String a4, System.Int32 a5) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1,a2,a3,a4,a5);
    }

    [BidMethod]
    internal static void Trace(string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.Int64 a3, System.UInt32 a4, System.Int32 a5, System.UInt32 a6, System.UInt32 a7) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW, a1,a2,a3,a4,a5,a6,a7);
    }



    [BidMethod]
    internal static void ScopeEnter(
            out IntPtr hScp, 
            string fmtPrintfW, 
            System.Int32 a1,
            [BidArgumentType(typeof(String))] System.Guid a2) {
        if ((modFlags & ApiGroup.Scope) != 0  &&  modID != NoData) {
            NativeMethods.ScopeEnter (modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW,a1, a2.ToString());
        } else {
            hScp = NoData;
        }
    }

    [BidMethod]
    internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW, System.Int32 a1, System.String a2, System.Int32 a3) {
        if ((modFlags & ApiGroup.Scope) != 0  &&  modID != NoData) {
            NativeMethods.ScopeEnter (modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW,a1,a2,a3);
        } else {
            hScp = NoData;
        }
    }

    [BidMethod]
    internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW, System.Int32 a1, System.Boolean a2, System.Int32 a3) {
        if ((modFlags & ApiGroup.Scope) != 0  &&  modID != NoData) {
            NativeMethods.ScopeEnter (modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW,a1,a2,a3);
        } else {
            hScp = NoData;
        }
    }    
    

    //
    //  Trace overloads
    //
    [BidMethod]
    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    internal static void Trace(string fmtPrintfW, System.Int32 a1, System.String a2) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW,a1,a2);
    }

    [BidMethod]
    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    internal static void Trace(string fmtPrintfW, System.IntPtr a1) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW,a1);
    }

    [BidMethod]
    internal static void Trace(string fmtPrintfW, System.Int32 a1) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW,a1);
    }

    [BidMethod]
    internal static void Trace(string fmtPrintfW, System.Int32 a1, System.Int32 a2) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW,a1,a2);
    }

    [BidMethod]
    internal static void Trace(string fmtPrintfW, System.Int32 a1, System.IntPtr a2, System.IntPtr a3) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW,a1,a2,a3);
    }

    [BidMethod]
    internal static void Trace(string fmtPrintfW, System.Int32 a1, System.IntPtr a2) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW,a1,a2);
    }

    [BidMethod]
    internal static void Trace(string fmtPrintfW, System.Int32 a1, System.String a2, System.String a3) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW,a1,a2,a3);
    }

    [BidMethod]
    internal static void Trace(string fmtPrintfW, System.Int32 a1, System.String a2, System.Int32 a3) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW,a1,a2,a3);
    }

    [BidMethod]
    internal static void Trace(string fmtPrintfW, System.Int32 a1, System.String a2, System.String a3, System.Int32 a4) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW,a1,a2,a3,a4);
    }

    [BidMethod]
    internal static void Trace(string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.Int32 a3, System.String a4, System.String a5, System.Int32 a6) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW,a1,a2,a3,a4,a5,a6);
    }

    [BidMethod]
    internal static void Trace(string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.Int32 a3) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW,a1,a2,a3);
    }

    [BidMethod]
    internal static void Trace(string fmtPrintfW, System.Int32 a1, System.Boolean a2) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW,a1,a2);
    }

    [BidMethod]
    internal static void Trace(string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.Int32 a3, System.Int32 a4) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW,a1,a2,a3,a4);
    }

    [BidMethod]
    internal static void Trace(string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.Boolean a3) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW,a1,a2,a3);
    }

    [BidMethod]
    internal static void Trace(string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.Int32 a3, System.Int32 a4, System.Int32 a5, System.Int32 a6, System.Int32 a7) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW,a1,a2,a3,a4,a5,a6,a7);
    }

    [BidMethod]
    internal static void Trace(string fmtPrintfW, System.Int32 a1, System.String a2, System.Int32 a3, System.Int32 a4, System.Boolean a5) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW,a1,a2,a3,a4,a5);
    }

    [BidMethod]
    internal static void Trace(string fmtPrintfW, System.Int32 a1, System.Int64 a2) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW,a1,a2);
    }

    [BidMethod]
    internal static void Trace(string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.Int64 a3) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW,a1,a2,a3 );
    }

    [BidMethod]
    internal static void Trace(string fmtPrintfW, System.Int32 a1, System.String a2, System.String a3, System.String a4, System.Int32 a5, System.Int64 a6) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW,a1,a2,a3,a4,a5,a6);
    }

    [BidMethod]
    internal static void Trace(string fmtPrintfW, System.Int32 a1, System.Int64 a2, System.Int32 a3, System.Int32 a4) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW,a1,a2,a3,a4);
    }

    [BidMethod]
    internal static void Trace(string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.Int64 a3, System.Int32 a4) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW,a1,a2,a3,a4);
    }

    [BidMethod]
    internal static void Trace(string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.Int32 a3, System.Int32 a4, System.String a5, System.String a6, System.String a7, System.Int32 a8) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW,a1,a2,a3,a4,a5,a6,a7,a8);
    }

    [BidMethod]
    internal static void Trace(string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.String a3, System.String a4) {
        if ((modFlags & ApiGroup.Trace) != 0  &&  modID != NoData)
            NativeMethods.Trace (modID, UIntPtr.Zero, UIntPtr.Zero, fmtPrintfW,a1,a2,a3,a4);
    }


    //
    //  ScopeEnter overloads
    //
    [BidMethod]
    internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW, System.Int32 a1, System.String a2) {
        if ((modFlags & ApiGroup.Scope) != 0  &&  modID != NoData) {
            NativeMethods.ScopeEnter (modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW,a1,a2);
        } else {
            hScp = NoData;
        }
    }

    [BidMethod]
    internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW, System.Int32 a1, System.Boolean a2) {
        if ((modFlags & ApiGroup.Scope) != 0  &&  modID != NoData) {
            NativeMethods.ScopeEnter (modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW,a1,a2);
        } else {
            hScp = NoData;
        }
    }

    [BidMethod]
    internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.String a3) {
        if ((modFlags & ApiGroup.Scope) != 0  &&  modID != NoData) {
            NativeMethods.ScopeEnter (modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW,a1,a2,a3);
        } else {
            hScp = NoData;
        }
    }

    [BidMethod]
    internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW, System.Int32 a1, System.String a2, System.Boolean a3) {
        if ((modFlags & ApiGroup.Scope) != 0  &&  modID != NoData) {
            NativeMethods.ScopeEnter (modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW,a1,a2,a3);
        } else {
            hScp = NoData;
        }
    }

    [BidMethod]
    internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.Boolean a3) {
        if ((modFlags & ApiGroup.Scope) != 0  &&  modID != NoData) {
            NativeMethods.ScopeEnter (modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW,a1,a2,a3);
        } else {
            hScp = NoData;
        }
    }

    [BidMethod]
    internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.Int32 a3, System.String a4) {
        if ((modFlags & ApiGroup.Scope) != 0  &&  modID != NoData) {
            NativeMethods.ScopeEnter (modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW,a1,a2,a3,a4);
        } else {
            hScp = NoData;
        }
    }

    [BidMethod]
    internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.Int32 a3) {
        if ((modFlags & ApiGroup.Scope) != 0  &&  modID != NoData) {
            NativeMethods.ScopeEnter (modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW,a1,a2,a3);
        } else {
            hScp = NoData;
        }
    }

    [BidMethod]
    internal static void ScopeEnter(out IntPtr hScp, string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.Boolean a3, System.Int32 a4) {
        if ((modFlags & ApiGroup.Scope) != 0  &&  modID != NoData) {
            NativeMethods.ScopeEnter (modID, UIntPtr.Zero, UIntPtr.Zero, out hScp, fmtPrintfW,a1,a2,a3,a4);
        } else {
            hScp = NoData;
        }
    }


    //
    // Interop calls to pluggable hooks [SuppressUnmanagedCodeSecurity] applied
    //
    private static partial class NativeMethods
    {
        //
        //  Manually edited wrappers
        //
        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.String a3, System.String a4, System.Int32 a5);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, System.Int32 a1, System.String a2, System.Boolean a3);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.Int64 a3, System.UInt32 a4, System.Int32 a5, System.UInt32 a6, System.UInt32 a7);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, System.String a1, System.String a2);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidScopeEnterCW")] extern
        internal static void ScopeEnter (IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, System.String a1, System.String a2);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidScopeEnterCW")] extern
        internal static void ScopeEnter (IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, System.Int32 a1, System.String a2, System.Int32 a3);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidScopeEnterCW")] extern
        internal static void ScopeEnter (IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, System.Int32 a1, System.Boolean a2, System.Int32 a3);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidScopeEnterCW")] extern
        internal static void ScopeEnter (IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, System.String a1, System.String a2, System.String a3);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidScopeEnterCW")] extern
        internal static void ScopeEnter (IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, System.Int32 a1, System.String a2, System.String a3, System.Int32 a4);

        //
        //  Trace
        //

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, System.IntPtr a1);

        [ResourceExposure(ResourceScope.None)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, System.Int32 a1);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, System.Boolean a1);

        [ResourceExposure(ResourceScope.None)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, string fmtPrintfW2, System.Int32 a1);

        [ResourceExposure(ResourceScope.None)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, System.Int32 a1, System.String a2);

        [ResourceExposure(ResourceScope.None)]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport(dllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "DllBidTraceCW")] extern
        internal static void Trace(IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.String a3);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, System.Int32 a1, System.Int32 a2);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, System.Int32 a1, System.IntPtr a2, System.IntPtr a3);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, System.Int32 a1, System.IntPtr a2);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, System.Int32 a1, System.String a2, System.String a3);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, System.Int32 a1, System.String a2, System.Int32 a3);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, System.Int32 a1, System.String a2, System.String a3, System.Int32 a4);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.Int32 a3, System.String a4, System.String a5, System.Int32 a6);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.Int32 a3);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, System.Int32 a1, System.Boolean a2);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, System.Int32 a1, System.String a2, System.String a3, System.String a4);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, System.Boolean a1, System.String a2, System.String a3, System.String a4);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.Int32 a3, System.Int32 a4);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.Boolean a3);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.Int32 a3, System.Int32 a4, System.Int32 a5, System.Int32 a6, System.Int32 a7);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, System.Int32 a1, System.String a2, System.Int32 a3, System.Int32 a4, System.Boolean a5);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, System.Int32 a1, System.Int64 a2);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.Int64 a3);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW1, string fmtPrintfW2, string fmtPrintfW3, System.Int64 a4);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, System.Int32 a1, System.String a2, System.String a3, System.String a4, System.Int32 a5, System.Int64 a6);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, System.Int32 a1, System.Int64 a2, System.Int32 a3, System.Int32 a4);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.Int64 a3, System.Int32 a4);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.Int32 a3, System.Int32 a4, System.String a5, System.String a6, System.String a7, System.Int32 a8);
        
        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidTraceCW")] extern
        internal static void Trace (IntPtr hID, UIntPtr src, UIntPtr info, string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.String a3, System.String a4);

        //
        //  ScopeEnter
        //
        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "DllBidScopeEnterCW")] extern
        internal static void ScopeEnter(IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, string a1);
        
        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidScopeEnterCW")] extern
        internal static void ScopeEnter (IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, System.Int32 a1, System.String a2);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidScopeEnterCW")] extern
        internal static void ScopeEnter (IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, System.Int32 a1, System.Boolean a2);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidScopeEnterCW")] extern
        internal static void ScopeEnter (IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.String a3);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidScopeEnterCW")] extern
        internal static void ScopeEnter (IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, System.Int32 a1, System.String a2, System.Boolean a3);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidScopeEnterCW")] extern
        internal static void ScopeEnter (IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.Boolean a3);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidScopeEnterCW")] extern
        internal static void ScopeEnter (IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.Int32 a3, System.String a4);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidScopeEnterCW")] extern
        internal static void ScopeEnter (IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.Int32 a3);

        [ResourceExposure(ResourceScope.None)]
        [DllImport(dllName, CharSet=CharSet.Unicode, CallingConvention=CallingConvention.Cdecl, EntryPoint="DllBidScopeEnterCW")] extern
        internal static void ScopeEnter (IntPtr hID, UIntPtr src, UIntPtr info, out IntPtr hScp, string fmtPrintfW, System.Int32 a1, System.Int32 a2, System.Boolean a3, System.Int32 a4);

    } // Native

} // Bid


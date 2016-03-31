// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace System.Threading.Tasks.Tests
{
    public static class CancellationTokenTests
    {
        [Fact]
        public static void CancellationTokenRegister_Exceptions()
        {
            CancellationToken token = new CancellationToken();
            Assert.Throws<ArgumentNullException>(() => token.Register(null));

            Assert.Throws<ArgumentNullException>(() => token.Register(null, false));

            Assert.Throws<ArgumentNullException>(() => token.Register(null, null));
        }

        [Fact]
        public static void CancellationTokenEquality()
        {
            //simple empty token comparisons
            Assert.Equal(new CancellationToken(), new CancellationToken());

            //inflated empty token comparisons
            CancellationToken inflated_empty_CT1 = new CancellationToken();
            bool temp1 = inflated_empty_CT1.CanBeCanceled; // inflate the CT
            CancellationToken inflated_empty_CT2 = new CancellationToken();
            bool temp2 = inflated_empty_CT2.CanBeCanceled; // inflate the CT

            Assert.Equal(inflated_empty_CT1, new CancellationToken());
            Assert.Equal(new CancellationToken(), inflated_empty_CT1);

            Assert.Equal(inflated_empty_CT1, inflated_empty_CT2);

            // inflated pre-set token comparisons
            CancellationToken inflated_defaultSet_CT1 = new CancellationToken(true);
            bool temp3 = inflated_defaultSet_CT1.CanBeCanceled; // inflate the CT
            CancellationToken inflated_defaultSet_CT2 = new CancellationToken(true);
            bool temp4 = inflated_defaultSet_CT2.CanBeCanceled; // inflate the CT

            Assert.Equal(inflated_defaultSet_CT1, new CancellationToken(true));
            Assert.Equal(inflated_defaultSet_CT1, inflated_defaultSet_CT2);


            // Things that are not equal
            Assert.NotEqual(inflated_empty_CT1, inflated_defaultSet_CT2);
            Assert.NotEqual(inflated_empty_CT1, new CancellationToken(true));
            Assert.NotEqual(new CancellationToken(true), inflated_empty_CT1);
        }

        [Fact]
        public static void CancellationToken_GetHashCode()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken ct = cts.Token;
            int hash1 = cts.GetHashCode();
            int hash2 = cts.Token.GetHashCode();
            int hash3 = ct.GetHashCode();

            Assert.Equal(hash1, hash2);
            Assert.Equal(hash2, hash3);

            CancellationToken defaultUnsetToken1 = new CancellationToken();
            CancellationToken defaultUnsetToken2 = new CancellationToken();
            int hashDefaultUnset1 = defaultUnsetToken1.GetHashCode();
            int hashDefaultUnset2 = defaultUnsetToken2.GetHashCode();
            Assert.Equal(hashDefaultUnset1, hashDefaultUnset2);

            CancellationToken defaultSetToken1 = new CancellationToken(true);
            CancellationToken defaultSetToken2 = new CancellationToken(true);
            int hashDefaultSet1 = defaultSetToken1.GetHashCode();
            int hashDefaultSet2 = defaultSetToken2.GetHashCode();
            Assert.Equal(hashDefaultSet1, hashDefaultSet2);

            Assert.NotEqual(hash1, hashDefaultUnset1);
            Assert.NotEqual(hash1, hashDefaultSet1);
            Assert.NotEqual(hashDefaultUnset1, hashDefaultSet1);
        }

        [Fact]
        public static void CancellationToken_EqualityAndDispose()
        {
            //hashcode.
            Assert.Throws<ObjectDisposedException>(
               () =>
               {
                   CancellationTokenSource cts = new CancellationTokenSource();
                   cts.Dispose();
                   cts.Token.GetHashCode();
               });

            //x.Equals(y)
            Assert.Throws<ObjectDisposedException>(
               () =>
               {
                   CancellationTokenSource cts = new CancellationTokenSource();
                   cts.Dispose();
                   cts.Token.Equals(new CancellationToken());
               });

            //x.Equals(y)
            Assert.Throws<ObjectDisposedException>(
               () =>
               {
                   CancellationTokenSource cts = new CancellationTokenSource();
                   cts.Dispose();
                   new CancellationToken().Equals(cts.Token);
               });

            //x==y
            Assert.Throws<ObjectDisposedException>(
               () =>
               {
                   CancellationTokenSource cts = new CancellationTokenSource();
                   cts.Dispose();
                   bool result = cts.Token == new CancellationToken();
               });

            //x==y
            Assert.Throws<ObjectDisposedException>(
               () =>
               {
                   CancellationTokenSource cts = new CancellationTokenSource();
                   cts.Dispose();
                   bool result = new CancellationToken() == cts.Token;
               });

            //x!=y
            Assert.Throws<ObjectDisposedException>(
               () =>
               {
                   CancellationTokenSource cts = new CancellationTokenSource();
                   cts.Dispose();
                   bool result = cts.Token != new CancellationToken();
               });

            //x!=y
            Assert.Throws<ObjectDisposedException>(
               () =>
               {
                   CancellationTokenSource cts = new CancellationTokenSource();
                   cts.Dispose();
                   bool result = new CancellationToken() != cts.Token;
               });
        }

        [Fact]
        public static void TokenSourceDispose()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            CancellationTokenRegistration preDisposeRegistration = token.Register(() => { });

            //WaitHandle and Dispose
            WaitHandle wh = token.WaitHandle; //ok
            Assert.NotNull(wh);
            tokenSource.Dispose();

            // Regression test: allow ctr.Dispose() to succeed when the backing cts has already been disposed.
            try
            {
                preDisposeRegistration.Dispose();
            }
            catch
            {
                Assert.True(false, string.Format("TokenSourceDispose:    > ctr.Dispose() failed when referring to a disposed CTS"));
            }

            bool cr = tokenSource.IsCancellationRequested; //this is ok after dispose.
            tokenSource.Dispose(); //Repeat calls to Dispose should be ok.
        }

        /// <summary>
        /// Test passive signalling.
        /// 
        /// Gets a token, then polls on its ThrowIfCancellationRequested property.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public static void CancellationTokenPassiveListening()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            Assert.False(token.IsCancellationRequested,
               "CancellationTokenPassiveListening:  Cancellation should not have occurred yet.");

            tokenSource.Cancel();
            Assert.True(token.IsCancellationRequested,
               "CancellationTokenPassiveListening:  Cancellation should now have occurred.");
        }

        /// <summary>
        /// Test active signalling.
        /// 
        /// Gets a token, registers a notification callback and ensure it is called.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public static void CancellationTokenActiveListening()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            CancellationToken token = tokenSource.Token;
            bool signalReceived = false;
            token.Register(() => signalReceived = true);

            Assert.False(signalReceived,
               "CancellationTokenActiveListening:  Cancellation should not have occurred yet.");
            tokenSource.Cancel();
            Assert.True(signalReceived,
               "CancellationTokenActiveListening:  Cancellation should now have occurred and caused a signal.");
        }

        private static event EventHandler AddAndRemoveDelegates_TestEvent;

        [Fact]
        public static void AddAndRemoveDelegates()
        {
            //Test various properties of callbacks:
            // 1. the same handler can be added multiple times
            // 2. removing a handler only removes one instance of a repeat
            // 3. after some add and removes, everything appears to be correct
            // 4. The behaviour matches the behaviour of a regular Event(Multicast-delegate).

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            List<string> output = new List<string>();

            Action action1 = () => output.Add("action1");
            Action action2 = () => output.Add("action2");

            CancellationTokenRegistration reg1 = token.Register(action1);
            CancellationTokenRegistration reg2 = token.Register(action2);
            CancellationTokenRegistration reg3 = token.Register(action2);
            CancellationTokenRegistration reg4 = token.Register(action1);

            reg2.Dispose();
            reg3.Dispose();
            reg4.Dispose();
            tokenSource.Cancel();

            Assert.Equal(1, output.Count);
            Assert.Equal("action1", output[0]);

            // and prove this is what normal events do...
            output.Clear();
            EventHandler handler1 = (sender, obj) => output.Add("handler1");
            EventHandler handler2 = (sender, obj) => output.Add("handler2");

            AddAndRemoveDelegates_TestEvent += handler1;
            AddAndRemoveDelegates_TestEvent += handler2;
            AddAndRemoveDelegates_TestEvent += handler2;
            AddAndRemoveDelegates_TestEvent += handler1;
            AddAndRemoveDelegates_TestEvent -= handler2;
            AddAndRemoveDelegates_TestEvent -= handler2;
            AddAndRemoveDelegates_TestEvent -= handler1;
            AddAndRemoveDelegates_TestEvent(null, EventArgs.Empty);
            Assert.Equal(1, output.Count);
            Assert.Equal("handler1", output[0]);
        }

        /// <summary>
        /// Test late enlistment.
        /// 
        /// If a handler is added to a 'canceled' cancellation token, the handler is called immediately.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public static void CancellationTokenLateEnlistment()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            CancellationToken token = tokenSource.Token;
            bool signalReceived = false;
            tokenSource.Cancel(); //Signal

            //Late enlist.. should fire the delegate synchronously
            token.Register(() => signalReceived = true);

            Assert.True(signalReceived,
               "CancellationTokenLateEnlistment:  The signal should have been received even after late enlistment.");
        }

        /// <summary>
        /// Test the wait handle exposed by the cancellation token
        /// 
        /// The signal occurs on a separate thread, and should happen after the wait begins.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public static void CancellationTokenWaitHandle_SignalAfterWait()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            CancellationToken token = tokenSource.Token;

            Task.Run(
                () =>
                {
                    tokenSource.Cancel(); //Signal
                });

            token.WaitHandle.WaitOne();

            Assert.True(token.IsCancellationRequested,
               "CancellationTokenWaitHandle_SignalAfterWait:  the token should have been canceled.");
        }

        /// <summary>
        /// Test the wait handle exposed by the cancellation token
        /// 
        /// The signal occurs on a separate thread, and should happen after the wait begins.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public static void CancellationTokenWaitHandle_SignalBeforeWait()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();

            CancellationToken token = tokenSource.Token;

            tokenSource.Cancel();
            token.WaitHandle.WaitOne(); // the wait handle should already be set.

            Assert.True(token.IsCancellationRequested,
               "CancellationTokenWaitHandle_SignalBeforeWait:  the token should have been canceled.");
        }

        /// <summary>
        /// Test that WaitAny can be used with a CancellationToken.WaitHandle
        /// </summary>
        /// <returns></returns>
        [Fact]
        public static void CancellationTokenWaitHandle_WaitAny()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;
            CancellationToken tokenNoSource = new CancellationToken();
            tokenSource.Cancel();

            WaitHandle.WaitAny(new[] { token.WaitHandle, tokenNoSource.WaitHandle }); //make sure the dummy tokens has a valid WaitHanle
            Assert.True(token.IsCancellationRequested,
               "CancellationTokenWaitHandle_WaitAny:  The token should have been canceled.");
        }

        [Fact]
        public static void CreateLinkedTokenSource_Simple_TwoToken()
        {
            CancellationTokenSource signal1 = new CancellationTokenSource();
            CancellationTokenSource signal2 = new CancellationTokenSource();

            //Neither token is signalled.
            CancellationTokenSource combined = CancellationTokenSource.CreateLinkedTokenSource(signal1.Token, signal2.Token);
            Assert.False(combined.IsCancellationRequested,
                "CreateLinkedToken_Simple_TwoToken:  The combined token should start unsignalled");

            signal1.Cancel();
            Assert.True(combined.IsCancellationRequested,
                "CreateLinkedToken_Simple_TwoToken:  The combined token should now be signalled");
        }

        [Fact]
        public static void CreateLinkedTokenSource_Simple_MultiToken()
        {
            CancellationTokenSource signal1 = new CancellationTokenSource();
            CancellationTokenSource signal2 = new CancellationTokenSource();
            CancellationTokenSource signal3 = new CancellationTokenSource();

            //Neither token is signalled.
            CancellationTokenSource combined = CancellationTokenSource.CreateLinkedTokenSource(new[] { signal1.Token, signal2.Token, signal3.Token });
            Assert.False(combined.IsCancellationRequested,
                "CreateLinkedToken_Simple_MultiToken:  The combined token should start unsignalled");

            signal1.Cancel();
            Assert.True(combined.IsCancellationRequested,
                "CreateLinkedToken_Simple_MultiToken:  The combined token should now be signalled");
        }

        [Fact]
        public static void CreateLinkedToken_SourceTokenAlreadySignalled()
        {
            //creating a combined token, when a source token is already signalled.
            CancellationTokenSource signal1 = new CancellationTokenSource();
            CancellationTokenSource signal2 = new CancellationTokenSource();

            signal1.Cancel(); //early signal.

            CancellationTokenSource combined = CancellationTokenSource.CreateLinkedTokenSource(signal1.Token, signal2.Token);
            Assert.True(combined.IsCancellationRequested,
                "CreateLinkedToken_SourceTokenAlreadySignalled:  The combined token should immediately be in the signalled state.");
        }

        [Fact]
        public static void CreateLinkedToken_MultistepComposition_SourceTokenAlreadySignalled()
        {
            //two-step composition
            CancellationTokenSource signal1 = new CancellationTokenSource();
            signal1.Cancel(); //early signal.

            CancellationTokenSource signal2 = new CancellationTokenSource();
            CancellationTokenSource combined1 = CancellationTokenSource.CreateLinkedTokenSource(signal1.Token, signal2.Token);

            CancellationTokenSource signal3 = new CancellationTokenSource();
            CancellationTokenSource combined2 = CancellationTokenSource.CreateLinkedTokenSource(signal3.Token, combined1.Token);

            Assert.True(combined2.IsCancellationRequested,
               "CreateLinkedToken_MultistepComposition_SourceTokenAlreadySignalled:  The 2-step combined token should immediately be in the signalled state.");
        }

        [Fact]
        public static void CallbacksOrderIsLifo()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            List<string> callbackOutput = new List<string>();
            token.Register(() => callbackOutput.Add("Callback1"));
            token.Register(() => callbackOutput.Add("Callback2"));

            tokenSource.Cancel();
            Assert.Equal("Callback2", callbackOutput[0]);
            Assert.Equal("Callback1", callbackOutput[1]);
        }

        [Fact]
        public static void Enlist_EarlyAndLate()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            CancellationTokenSource earlyEnlistedTokenSource = new CancellationTokenSource();

            token.Register(() => earlyEnlistedTokenSource.Cancel());
            tokenSource.Cancel();

            Assert.Equal(true, earlyEnlistedTokenSource.IsCancellationRequested);


            CancellationTokenSource lateEnlistedTokenSource = new CancellationTokenSource();
            token.Register(() => lateEnlistedTokenSource.Cancel());
            Assert.Equal(true, lateEnlistedTokenSource.IsCancellationRequested);
        }

        /// <summary>
        /// This test from donnya. Thanks Donny.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public static void WaitAll()
        {
            Debug.WriteLine("WaitAll:  Testing CancellationTokenTests.WaitAll, If Join does not work, then a deadlock will occur.");

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationTokenSource signal2 = new CancellationTokenSource();
            ManualResetEvent mre = new ManualResetEvent(false);
            ManualResetEvent mre2 = new ManualResetEvent(false);

            Task t = new Task(() =>
            {
                WaitHandle.WaitAll(new WaitHandle[] { tokenSource.Token.WaitHandle, signal2.Token.WaitHandle, mre });
                mre2.Set();
            });

            t.Start();
            tokenSource.Cancel();
            signal2.Cancel();
            mre.Set();
            mre2.WaitOne();
            t.Wait();

            //true if the Join succeeds.. otherwise a deadlock will occur.
        }

        [Fact]
        public static void BehaviourAfterCancelSignalled()
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            token.Register(() => { });
            tokenSource.Cancel();
        }

        [Fact]
        public static void Cancel_ThrowOnFirstException()
        {
            ManualResetEvent mre_CancelHasBeenEnacted = new ManualResetEvent(false);

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            // Main test body 
            ArgumentException caughtException = null;
            token.Register(() =>
                               {
                                   throw new InvalidOperationException();
                               });

            token.Register(() =>
                               {
                                   throw new ArgumentException();
                               });  // !!NOTE: Due to LIFO ordering, this delegate should be the only one to run.


            Task.Run(() =>
                {
                    try
                    {
                        tokenSource.Cancel(true);
                    }
                    catch (ArgumentException ex)
                    {
                        caughtException = ex;
                    }
                    catch (Exception ex)
                    {
                        Assert.True(false, string.Format("Cancel_ThrowOnFirstException:  The wrong exception type was thrown. ex=" + ex));
                    }
                    mre_CancelHasBeenEnacted.Set();
                });

            mre_CancelHasBeenEnacted.WaitOne();
            Assert.NotNull(caughtException);
        }

        [Fact]
        public static void Cancel_DontThrowOnFirstException()
        {
            ManualResetEvent mre_CancelHasBeenEnacted = new ManualResetEvent(false);

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;

            // Main test body 
            AggregateException caughtException = null;
            token.Register(() => { throw new ArgumentException(); });
            token.Register(() => { throw new InvalidOperationException(); });


            Task.Run(
                () =>
                {
                    try
                    {
                        tokenSource.Cancel(false);
                    }
                    catch (AggregateException ex)
                    {
                        caughtException = ex;
                    }
                    mre_CancelHasBeenEnacted.Set();
                }
                );

            mre_CancelHasBeenEnacted.WaitOne();
            Assert.NotNull(caughtException);
            Assert.Equal(2, caughtException.InnerExceptions.Count);
            Assert.True(caughtException.InnerExceptions[0] is InvalidOperationException,
               "Cancel_ThrowOnFirstException:  Due to LIFO call order, the first inner exception should be an InvalidOperationException.");
            Assert.True(caughtException.InnerExceptions[1] is ArgumentException,
               "Cancel_ThrowOnFirstException:  Due to LIFO call order, the second inner exception should be an ArgumentException.");
        }

        [Fact]
        public static void CancellationRegistration_RepeatDispose()
        {
            Exception caughtException = null;

            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken ct = cts.Token;

            CancellationTokenRegistration registration = ct.Register(() => { });
            try
            {
                registration.Dispose();
                registration.Dispose();
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            Assert.Null(caughtException);
        }

        [Fact]
        public static void CancellationTokenRegistration_EqualityAndHashCode()
        {
            CancellationTokenSource outerCTS = new CancellationTokenSource();

            {
                // different registrations on 'different' default tokens
                CancellationToken ct1 = new CancellationToken();
                CancellationToken ct2 = new CancellationToken();

                CancellationTokenRegistration ctr1 = ct1.Register(() => outerCTS.Cancel());
                CancellationTokenRegistration ctr2 = ct2.Register(() => outerCTS.Cancel());

                Assert.True(ctr1.Equals(ctr2),
                   "CancellationTokenRegistration_EqualityAndHashCode:  [1]The two registrations should compare equal, as they are both dummies.");
                Assert.True(ctr1 == ctr2,
                   "CancellationTokenRegistration_EqualityAndHashCode:  [2]The two registrations should compare equal, as they are both dummies.");
                Assert.False(ctr1 != ctr2,
                   "CancellationTokenRegistration_EqualityAndHashCode:  [3]The two registrations should compare equal, as they are both dummies.");
                Assert.True(ctr1.GetHashCode() == ctr2.GetHashCode(),
                   "CancellationTokenRegistration_EqualityAndHashCode:  [4]The two registrations should have the same hashcode, as they are both dummies.");
            }

            {
                // different registrations on the same already cancelled token
                CancellationTokenSource cts = new CancellationTokenSource();
                cts.Cancel();
                CancellationToken ct = cts.Token;

                CancellationTokenRegistration ctr1 = ct.Register(() => outerCTS.Cancel());
                CancellationTokenRegistration ctr2 = ct.Register(() => outerCTS.Cancel());

                Assert.True(ctr1.Equals(ctr2),
                   "CancellationTokenRegistration_EqualityAndHashCode:  [1]The two registrations should compare equal, as they are both dummies due to CTS being already canceled.");
                Assert.True(ctr1 == ctr2,
                   "CancellationTokenRegistration_EqualityAndHashCode:  [2]The two registrations should compare equal, as they are both dummies due to CTS being already canceled.");
                Assert.False(ctr1 != ctr2,
                   "CancellationTokenRegistration_EqualityAndHashCode:  [3]The two registrations should compare equal, as they are both dummies due to CTS being already canceled.");
                Assert.True(ctr1.GetHashCode() == ctr2.GetHashCode(),
                   "CancellationTokenRegistration_EqualityAndHashCode:  [4]The two registrations should have the same hashcode, as they are both dummies due to CTS being already canceled.");
            }

            {
                // different registrations on one real token    
                CancellationTokenSource cts1 = new CancellationTokenSource();

                CancellationTokenRegistration ctr1 = cts1.Token.Register(() => outerCTS.Cancel());
                CancellationTokenRegistration ctr2 = cts1.Token.Register(() => outerCTS.Cancel());

                Assert.False(ctr1.Equals(ctr2),
                   "CancellationTokenRegistration_EqualityAndHashCode:  The two registrations should not compare equal.");
                Assert.False(ctr1 == ctr2,
                   "CancellationTokenRegistration_EqualityAndHashCode:  The two registrations should not compare equal.");
                Assert.True(ctr1 != ctr2,
                   "CancellationTokenRegistration_EqualityAndHashCode:  The two registrations should not compare equal.");
                Assert.False(ctr1.GetHashCode() == ctr2.GetHashCode(),
                   "CancellationTokenRegistration_EqualityAndHashCode:  The two registrations should not have the same hashcode.");

                CancellationTokenRegistration ctr1copy = ctr1;
                Assert.True(ctr1 == ctr1copy, "The two registrations should be equal.");
            }

            {
                // registrations on different real tokens.
                // different registrations on one token    
                CancellationTokenSource cts1 = new CancellationTokenSource();
                CancellationTokenSource cts2 = new CancellationTokenSource();

                CancellationTokenRegistration ctr1 = cts1.Token.Register(() => outerCTS.Cancel());
                CancellationTokenRegistration ctr2 = cts2.Token.Register(() => outerCTS.Cancel());

                Assert.False(ctr1.Equals(ctr2),
                   "CancellationTokenRegistration_EqualityAndHashCode:  The two registrations should not compare equal.");
                Assert.False(ctr1 == ctr2,
                   "CancellationTokenRegistration_EqualityAndHashCode:  The two registrations should not compare equal.");
                Assert.True(ctr1 != ctr2,
                   "CancellationTokenRegistration_EqualityAndHashCode:  The two registrations should not compare equal.");
                Assert.False(ctr1.GetHashCode() == ctr2.GetHashCode(),
                   "CancellationTokenRegistration_EqualityAndHashCode:  The two registrations should not have the same hashcode.");

                CancellationTokenRegistration ctr1copy = ctr1;
                Assert.True(ctr1.Equals(ctr1copy), "The two registrations should be equal.");
            }
        }

        [Fact]
        public static void CancellationTokenLinking_ODEinTarget()
        {
            CancellationTokenSource cts1 = new CancellationTokenSource();
            CancellationTokenSource cts2 = CancellationTokenSource.CreateLinkedTokenSource(cts1.Token, new CancellationToken());
            Exception caughtException = null;

            cts2.Token.Register(() => { throw new ObjectDisposedException("myException"); });

            try
            {
                cts1.Cancel(true);
            }
            catch (Exception ex)
            {
                caughtException = ex;
            }

            Assert.True(
               caughtException is AggregateException
                  && caughtException.InnerException is ObjectDisposedException
                  && caughtException.InnerException.Message.Contains("myException"),
               "CancellationTokenLinking_ODEinTarget:  The users ODE should be caught. Actual:" + caughtException);
        }

        [Fact]
        public static void ThrowIfCancellationRequested()
        {
            OperationCanceledException caughtEx = null;

            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken ct = cts.Token;

            ct.ThrowIfCancellationRequested();
            // no exception should occur

            cts.Cancel();

            try
            {
                ct.ThrowIfCancellationRequested();
            }
            catch (OperationCanceledException oce)
            {
                caughtEx = oce;
            }

            Assert.NotNull(caughtEx);
            Assert.Equal(ct, caughtEx.CancellationToken);
        }

        /// <summary>
        /// ensure that calling ctr.Dipose() from within a cancellation callback will not deadlock.
        /// </summary>
        /// <returns></returns>
        [Fact]
        public static void DeregisterFromWithinACallbackIsSafe_BasicTest()
        {
            Debug.WriteLine("CancellationTokenTests.Bug720327_DeregisterFromWithinACallbackIsSafe_BasicTest()");
            Debug.WriteLine("  - this method should complete immediately.  Delay to complete indicates a deadlock failure.");

            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken ct = cts.Token;

            CancellationTokenRegistration ctr1 = ct.Register(() => { });
            ct.Register(() => { ctr1.Dispose(); });

            cts.Cancel();
            Debug.WriteLine("  - Completed OK.");
        }

        // regression test
        // Disposing a linkedCTS would previously throw if a source CTS had been 
        // disposed already.  (it is an error for a user to get in this situation, but we decided to allow it to work).
        [Fact]
        public static void ODEWhenDisposingLinkedCTS()
        {
            try
            {
                // User passes a cancellation token (CT) to component A. 
                CancellationTokenSource userTokenSource = new CancellationTokenSource();
                CancellationToken userToken = userTokenSource.Token;

                // Component A implements "timeout", by creating its own cancellation token source (CTS) and invoking cts.Cancel() when the timeout timer fires.
                CancellationTokenSource cts2 = new CancellationTokenSource();
                CancellationToken cts2Token = cts2.Token;

                // Component A creates a linked token source representing the CT from the user and the "timeout" CT.
                var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cts2Token, userToken);

                // User calls Cancel() on his CTS and then Dispose()
                userTokenSource.Cancel();
                userTokenSource.Dispose();

                // Component B correctly cancels the operation, returns to component A. 
                // ...

                // Component A now disposes the linked CTS => ObjectDisposedException is thrown by cts.Dispose() because the user CTS was already disposed.
                linkedTokenSource.Dispose();
            }
            catch (Exception ex)
            {
                if (ex is ObjectDisposedException)
                {
                    Assert.True(false, string.Format("Bug901737_ODEWhenDisposingLinkedCTS:  - ODE Occurred!"));
                }
                else
                {
                    Assert.True(false, string.Format("Bug901737_ODEWhenDisposingLinkedCTS:  - Exception Occurred (not an ODE!!): " + ex));
                }
            }
        }

        // Several tests for deriving custom user types from CancellationTokenSource
        [Fact]
        public static void DerivedCancellationTokenSource()
        {
            // Verify that a derived CTS is functional
            {
                CancellationTokenSource c = new DerivedCTS(null);
                CancellationToken token = c.Token;

                var task = Task.Factory.StartNew(() => c.Cancel());
                task.Wait();

                Assert.True(token.IsCancellationRequested,
                   "DerivedCancellationTokenSource:  The token should have been cancelled.");
            }

            // Verify that callback list on a derived CTS is functional
            {
                CancellationTokenSource c = new DerivedCTS(null);
                CancellationToken token = c.Token;
                int callbackRan = 0;

                token.Register(() => Interlocked.Increment(ref callbackRan));

                var task = Task.Factory.StartNew(() => c.Cancel());
                task.Wait();
                SpinWait.SpinUntil(() => callbackRan > 0, 1000);

                Assert.True(callbackRan == 1,
                   "DerivedCancellationTokenSource:  Expected the callback to run once. Instead, it ran " + callbackRan + " times.");
            }

            // Test the Dispose path for a class derived from CancellationTokenSource
            {
                var disposeTracker = new DisposeTracker();
                CancellationTokenSource c = new DerivedCTS(disposeTracker);
                Assert.True(c.Token.CanBeCanceled,
                    "DerivedCancellationTokenSource:  The token should be cancellable.");

                c.Dispose();

                // Dispose() should have prevented the finalizer from running. Give the finalizer a chance to run. If this
                // results in Dispose(false) getting called, we'll catch the issue.
                GC.Collect();
                GC.WaitForPendingFinalizers();

                Assert.True(disposeTracker.DisposeTrueCalled,
                    "DerivedCancellationTokenSource:  Dispose(true) should have been called.");
                Assert.False(disposeTracker.DisposeFalseCalled,
                    "DerivedCancellationTokenSource:  Dispose(false) should not have been called.");
            }

            // Test the finalization code path for a class derived from CancellationTokenSource
            {
                var disposeTracker = new DisposeTracker();

                // Since the object is not assigned into a variable, it can be GC'd before the current method terminates.
                // (This is only an issue in the Debug build)
                new DerivedCTS(disposeTracker);

                // Wait until the DerivedCTS object is finalized
                SpinWait.SpinUntil(() =>
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    return disposeTracker.DisposeTrueCalled;
                }, 500);

                Assert.False(disposeTracker.DisposeTrueCalled,
                    "DerivedCancellationTokenSource:  Dispose(true) should not have been called.");
                Assert.True(disposeTracker.DisposeFalseCalled,
                    "DerivedCancellationTokenSource:  Dispose(false) should have been called.");
            }

            // Verify that Dispose(false) is a no-op on the CTS. Dispose(false) should only release any unmanaged resources, and
            // CTS does not currently hold any unmanaged resources.
            {
                var disposeTracker = new DisposeTracker();

                DerivedCTS c = new DerivedCTS(disposeTracker);
                c.DisposeUnmanaged();

                // No exception expected - the CancellationTokenSource should be valid
                Assert.True(c.Token.CanBeCanceled,
                   "DerivedCancellationTokenSource:  The token should still be cancellable.");

                Assert.False(disposeTracker.DisposeTrueCalled,
                   "DerivedCancellationTokenSource:  Dispose(true) should not have been called.");
                Assert.True(disposeTracker.DisposeFalseCalled,
                   "DerivedCancellationTokenSource:  Dispose(false) should have run.");
            }
        }

        // Several tests for deriving custom user types from CancellationTokenSource
        [Fact]
        public static void DerivedCancellationTokenSource_Negative()
        {
            // Test the Dispose path for a class derived from CancellationTokenSource
            {
                var disposeTracker = new DisposeTracker();
                CancellationTokenSource c = new DerivedCTS(disposeTracker);

                c.Dispose();

                // Dispose() should have prevented the finalizer from running. Give the finalizer a chance to run. If this
                // results in Dispose(false) getting called, we'll catch the issue.
                GC.Collect();
                GC.WaitForPendingFinalizers();
                Assert.Throws<ObjectDisposedException>(
                    () =>
                    {
                        // Accessing the Token property should throw an ObjectDisposedException
                        if (c.Token.CanBeCanceled)
                            Assert.True(false, string.Format("DerivedCancellationTokenSource: Accessing the Token property should throw an ObjectDisposedException, but it did not."));
                        else
                            Assert.True(false, string.Format("DerivedCancellationTokenSource: Accessing the Token property should throw an ObjectDisposedException, but it did not."));
                    });
            }
        }

        [Fact]
        public static void CancellationTokenSourceWithTimer()
        {
            TimeSpan bigTimeSpan = new TimeSpan(2000, 0, 0, 0, 0);
            TimeSpan reasonableTimeSpan = new TimeSpan(0, 0, 1);
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.Dispose();


            //
            // Test out some int-based timeout logic
            //
            cts = new CancellationTokenSource(-1); // should be an infinite timeout
            CancellationToken token = cts.Token;
            ManualResetEventSlim mres = new ManualResetEventSlim(false);
            CancellationTokenRegistration ctr = token.Register(() => mres.Set());

            Assert.False(token.IsCancellationRequested,
               "CancellationTokenSourceWithTimer:  Cancellation signaled on infinite timeout (int)!");

            cts.CancelAfter(1000000);

            Assert.False(token.IsCancellationRequested,
               "CancellationTokenSourceWithTimer:  Cancellation signaled on super-long timeout (int) !");

            cts.CancelAfter(1);

            Debug.WriteLine("CancellationTokenSourceWithTimer: > About to wait on cancellation that should occur soon (int)... if we hang, something bad happened");

            mres.Wait();

            cts.Dispose();

            //
            // Test out some TimeSpan-based timeout logic
            //
            TimeSpan prettyLong = new TimeSpan(1, 0, 0);
            cts = new CancellationTokenSource(prettyLong);
            token = cts.Token;
            mres = new ManualResetEventSlim(false);
            ctr = token.Register(() => mres.Set());

            Assert.False(token.IsCancellationRequested,
               "CancellationTokenSourceWithTimer:  Cancellation signaled on super-long timeout (TimeSpan,1)!");

            cts.CancelAfter(prettyLong);

            Assert.False(token.IsCancellationRequested,
               "CancellationTokenSourceWithTimer:  Cancellation signaled on super-long timeout (TimeSpan,2) !");

            cts.CancelAfter(new TimeSpan(1000));

            Debug.WriteLine("CancellationTokenSourceWithTimer: > About to wait on cancellation that should occur soon (TimeSpan)... if we hang, something bad happened");

            mres.Wait();

            cts.Dispose();
        }
        [Fact]
        public static void CancellationTokenSourceWithTimer_Negative()
        {
            TimeSpan bigTimeSpan = new TimeSpan(2000, 0, 0, 0, 0);
            TimeSpan reasonableTimeSpan = new TimeSpan(0, 0, 1);
            //
            // Test exception logic
            //
            Assert.Throws<ArgumentOutOfRangeException>(
               () => { new CancellationTokenSource(-2); });
            Assert.Throws<ArgumentOutOfRangeException>(
               () => { new CancellationTokenSource(bigTimeSpan); });

            CancellationTokenSource cts = new CancellationTokenSource();
            Assert.Throws<ArgumentOutOfRangeException>(
               () => { cts.CancelAfter(-2); });
            Assert.Throws<ArgumentOutOfRangeException>(
               () => { cts.CancelAfter(bigTimeSpan); });

            cts.Dispose();
            Assert.Throws<ObjectDisposedException>(
               () => { cts.CancelAfter(1); });
            Assert.Throws<ObjectDisposedException>(
               () => { cts.CancelAfter(reasonableTimeSpan); });
        }

        [Fact]
        public static void EnlistWithSyncContext_BeforeCancel()
        {
            ManualResetEvent mre_CancelHasBeenEnacted = new ManualResetEvent(false); //synchronization helper

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;


            // Install a SynchronizationContext...
            SynchronizationContext prevailingSyncCtx = SynchronizationContext.Current;
            TestingSynchronizationContext testContext = new TestingSynchronizationContext();
            SetSynchronizationContext(testContext);

            // Main test body 

            // register a null delegate, but use the currently registered syncContext.
            // the testSyncContext will track that it was used when the delegate is invoked.
            token.Register(() => { }, true);

            Task.Run(
                () =>
                {
                    tokenSource.Cancel();
                    mre_CancelHasBeenEnacted.Set();
                }
                );

            mre_CancelHasBeenEnacted.WaitOne();
            Assert.True(testContext.DidSendOccur,
               "EnlistWithSyncContext_BeforeCancel:  the delegate should have been called via Send to SyncContext.");

            //Cleanup.
            SetSynchronizationContext(prevailingSyncCtx);
        }

        [Fact]
        public static void EnlistWithSyncContext_BeforeCancel_ThrowingExceptionInSyncContextDelegate()
        {
            ManualResetEvent mre_CancelHasBeenEnacted = new ManualResetEvent(false); //synchronization helper

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;


            // Install a SynchronizationContext...
            SynchronizationContext prevailingSyncCtx = SynchronizationContext.Current;
            TestingSynchronizationContext testContext = new TestingSynchronizationContext();
            SetSynchronizationContext(testContext);

            // Main test body 
            AggregateException caughtException = null;

            // register a null delegate, but use the currently registered syncContext.
            // the testSyncContext will track that it was used when the delegate is invoked.
            token.Register(() => { throw new ArgumentException(); }, true);

            Task.Run(
                () =>
                {
                    try
                    {
                        tokenSource.Cancel();
                    }
                    catch (AggregateException ex)
                    {
                        caughtException = ex;
                    }
                    mre_CancelHasBeenEnacted.Set();
                }
                );

            mre_CancelHasBeenEnacted.WaitOne();
            Assert.True(testContext.DidSendOccur,
               "EnlistWithSyncContext_BeforeCancel_ThrowingExceptionInSyncContextDelegate:  the delegate should have been called via Send to SyncContext.");
            Assert.NotNull(caughtException);
            Assert.Equal(1, caughtException.InnerExceptions.Count);
            Assert.True(caughtException.InnerExceptions[0] is ArgumentException,
               "EnlistWithSyncContext_BeforeCancel_ThrowingExceptionInSyncContextDelegate:  The inner exception should be an ArgumentException.");

            //Cleanup.
            SetSynchronizationContext(prevailingSyncCtx);
        }
        [Fact]
        public static void EnlistWithSyncContext_BeforeCancel_ThrowingExceptionInSyncContextDelegate_ThrowOnFirst()
        {
            ManualResetEvent mre_CancelHasBeenEnacted = new ManualResetEvent(false); //synchronization helper

            CancellationTokenSource tokenSource = new CancellationTokenSource();
            CancellationToken token = tokenSource.Token;


            // Install a SynchronizationContext...
            SynchronizationContext prevailingSyncCtx = SynchronizationContext.Current;
            TestingSynchronizationContext testContext = new TestingSynchronizationContext();
            SetSynchronizationContext(testContext);

            // Main test body 
            ArgumentException caughtException = null;

            // register a null delegate, but use the currently registered syncContext.
            // the testSyncContext will track that it was used when the delegate is invoked.
            token.Register(() => { throw new ArgumentException(); }, true);

            Task.Run(
                () =>
                {
                    try
                    {
                        tokenSource.Cancel(true);
                    }
                    catch (ArgumentException ex)
                    {
                        caughtException = ex;
                    }
                    mre_CancelHasBeenEnacted.Set();
                }
                );

            mre_CancelHasBeenEnacted.WaitOne();
            Assert.True(testContext.DidSendOccur,
               "EnlistWithSyncContext_BeforeCancel_ThrowingExceptionInSyncContextDelegate_ThrowOnFirst:  the delegate should have been called via Send to SyncContext.");
            Assert.NotNull(caughtException);

            //Cleanup
            SetSynchronizationContext(prevailingSyncCtx);
        }

        // Test that we marshal exceptions back if we run callbacks on a sync context.
        // (This assumes that a syncContext.Send() may not be doing the marshalling itself).
        [Fact]
        public static void SyncContextWithExceptionThrowingCallback()
        {
            Exception caughtEx1 = null;
            AggregateException caughtEx2 = null;

            SynchronizationContext prevailingSyncCtx = SynchronizationContext.Current;
            SetSynchronizationContext(new ThreadCrossingSynchronizationContext());


            // -- Test 1 -- //
            CancellationTokenSource cts = new CancellationTokenSource();
            cts.Token.Register(
                () => { throw new Exception("testEx1"); }, true);

            try
            {
                cts.Cancel(true); //throw on first exception
            }
            catch (Exception ex)
            {
                caughtEx1 = (AggregateException)ex;
            }

            Assert.NotNull(caughtEx1);

            // -- Test 2 -- //
            cts = new CancellationTokenSource();
            cts.Token.Register(
               () => { throw new ArgumentException("testEx2"); }, true);

            try
            {
                cts.Cancel(false); //do not throw on first exception
            }
            catch (AggregateException ex)
            {
                caughtEx2 = (AggregateException)ex;
            }
            Assert.NotNull(caughtEx2);
            Assert.Equal(1, caughtEx2.InnerExceptions.Count);

            // clean up
            SetSynchronizationContext(prevailingSyncCtx);
        }

        [Fact]
        public static void Bug720327_DeregisterFromWithinACallbackIsSafe_SyncContextTest()
        {
            Debug.WriteLine("* CancellationTokenTests.Bug720327_DeregisterFromWithinACallbackIsSafe_SyncContextTest()");
            Debug.WriteLine("  - this method should complete immediately.  Delay to complete indicates a deadlock failure.");

            //Install our syncContext.
            SynchronizationContext prevailingSyncCtx = SynchronizationContext.Current;
            ThreadCrossingSynchronizationContext threadCrossingSyncCtx = new ThreadCrossingSynchronizationContext();
            SetSynchronizationContext(threadCrossingSyncCtx);

            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken ct = cts.Token;

            CancellationTokenRegistration ctr1 = ct.Register(() => { });
            CancellationTokenRegistration ctr2 = ct.Register(() => { });
            CancellationTokenRegistration ctr3 = ct.Register(() => { });
            CancellationTokenRegistration ctr4 = ct.Register(() => { });

            ct.Register(() => { ctr1.Dispose(); }, true);  // with a custom syncContext
            ct.Register(() => { ctr2.Dispose(); }, false);  // without
            ct.Register(() => { ctr3.Dispose(); }, true);  // with a custom syncContext
            ct.Register(() => { ctr4.Dispose(); }, false);  // without

            cts.Cancel();
            Debug.WriteLine("  - Completed OK.");

            //cleanup
            SetSynchronizationContext(prevailingSyncCtx);
        }

        #region Helper Classes and Methods

        private class TestingSynchronizationContext : SynchronizationContext
        {
            public bool DidSendOccur = false;

            override public void Send(SendOrPostCallback d, Object state)
            {
                //Note: another idea was to install this syncContext on the executing thread.
                //unfortunately, the ExecutionContext business gets in the way and reestablishes a default SyncContext.

                DidSendOccur = true;
                base.Send(d, state); // call the delegate with our syncContext installed.
            }
        }

        /// <summary>
        /// This syncContext uses a different thread to run the work
        /// This is similar to how WindowsFormsSynchronizationContext works.
        /// </summary>
        private class ThreadCrossingSynchronizationContext : SynchronizationContext
        {
            public bool DidSendOccur = false;

            override public void Send(SendOrPostCallback d, Object state)
            {
                Exception marshalledException = null;
                Task t = new Task(
                    (passedInState) =>
                    {
                        //Debug.WriteLine(" threadCrossingSyncContext..running callback delegate on threadID = " + Thread.CurrentThread.ManagedThreadId);

                        try
                        {
                            d(passedInState);
                        }
                        catch (Exception e)
                        {
                            marshalledException = e;
                        }
                    }, state);

                t.Start();
                t.Wait();

                //t.Start(state);
                //t.Join();

                if (marshalledException != null)
                    throw new AggregateException("DUMMY: ThreadCrossingSynchronizationContext.Send captured and propogated an exception",
                        marshalledException);
            }
        }

        /// <summary>
        /// A test class derived from CancellationTokenSource
        /// </summary>
        internal class DerivedCTS : CancellationTokenSource
        {
            private DisposeTracker _disposeTracker;

            public DerivedCTS(DisposeTracker disposeTracker)
            {
                _disposeTracker = disposeTracker;
            }

            protected override void Dispose(bool disposing)
            {
                // Dispose any derived class state. DerivedCTS simply records that Dispose() has been called.
                if (_disposeTracker != null)
                {
                    if (disposing) { _disposeTracker.DisposeTrueCalled = true; }
                    else { _disposeTracker.DisposeFalseCalled = true; }
                }

                // Dispose the state in the CancellationTokenSource base class
                base.Dispose(disposing);
            }

            /// <summary>
            /// A helper method to call Dispose(false). That allows us to easily simulate finalization of CTS, while still maintaining
            /// a reference to the CTS.
            /// </summary>
            public void DisposeUnmanaged()
            {
                Dispose(false);
            }

            ~DerivedCTS()
            {
                Dispose(false);
            }
        }

        /// <summary>
        /// A simple class to track whether Dispose(bool) method has been called and if so, what was the bool flag.
        /// </summary>
        internal class DisposeTracker
        {
            public bool DisposeTrueCalled = false;
            public bool DisposeFalseCalled = false;
        }

        public static void SetSynchronizationContext(SynchronizationContext sc)
        {
            SynchronizationContext.SetSynchronizationContext(sc);
        }

        #endregion
    }
}

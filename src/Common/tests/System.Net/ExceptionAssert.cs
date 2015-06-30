namespace NCLTest.Common
{
    using CoreFXTestLibrary;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public static class ExceptionAssert
    {
        public static void ThrowsObjectDisposed(Action action, string failureText)
        {
            Throws<ObjectDisposedException>(action, failureText);
        }

        public static void ThrowsFormat(Action action, string failureText)
        {            
            Throws<FormatException>(action, failureText);
        }

        public static void ThrowsInvalidOperation(Action action, string failureText)
        {
            Throws<InvalidOperationException>(action, failureText);
        }

        public static void Throws<T>(Action action, string failureText) where T : Exception
        {
            CoreFXTestLibrary.ExceptionAssert.Throws<T>(failureText, action);
        }

        public static void AreEqual(Action action, Type exceptionType)
        {
            try
            {
                action();
                Assert.Fail("Expected to throw {0}", exceptionType);
            }
            catch (Exception actualException)
            {
                AreEqual(actualException, exceptionType);
            }
        }

        public static void AreEqual(Action action, Type exceptionType, string exceptionMessage)
        {
            AreEqual(action, new Tuple<Type, string>[] { new Tuple<Type, string>(exceptionType, exceptionMessage) });
        }

        public static void AreEqual(Action action, Tuple<Type, string>[] expectedExceptions)
        {
            bool exceptionThrown = false;
            try
            {
                action();
            }
            catch (Exception actualException)
            {
                exceptionThrown = true;
                AreEqual(actualException, expectedExceptions);
            }

            if (!exceptionThrown)
            {
                AssertFailExceptionNotFound(null, expectedExceptions);
            }
        }

        public static void AreEqual(Exception actualException, Type exceptionType)
        {
            Logger.LogInformation("Validating exception...");
            Exception innerException = actualException;
            while (innerException != null)
            {
                Logger.LogInformation("{0}: {1}", innerException.GetType(), innerException.Message);
                if (innerException.GetType().Equals(exceptionType))
                {
                    Logger.LogInformation("Validated exception successfully!");
                    return;
                }

                innerException = innerException.InnerException;
            }
            Assert.Fail("Exception not found. {0}", exceptionType);
        }

        public static void AreEqual(Exception actualException, Type exceptionType, string exceptionMesssage)
        {
            AreEqual(actualException, new Tuple<Type, string>[] { new Tuple<Type, string>(exceptionType, exceptionMesssage) });
        }

        public static void AreEqual(Exception actualException, Tuple<Type, string>[] expectedExceptions)
        {
            Logger.LogInformation("Validating exception...");
            List<Exception> exceptions = new List<Exception>();
            exceptions.Add(actualException);

            AggregateException aggregateException = actualException as AggregateException;
            if (aggregateException != null)
            {
                exceptions.AddRange(aggregateException.Flatten().InnerExceptions);
            }

            for (int i = 0; i < exceptions.Count; i++)
            {
                Exception innerException = exceptions[i];
                while (innerException != null)
                {
                    Logger.LogInformation("{0}: {1}", innerException.GetType(), innerException.Message);

                    foreach (Tuple<Type, string> expectedException in expectedExceptions)
                    {
                        if (innerException.GetType().Equals(expectedException.Item1))
                        {
                            if (innerException.Message.Equals(expectedException.Item2))
                            {
                                Logger.LogInformation("Validated exception successfully!");
                                return;
                            }
                        }
                    }

                    innerException = innerException.InnerException;
                }
            }

            AssertFailExceptionNotFound(actualException, expectedExceptions);
        }

        private static void AssertFailExceptionNotFound(Exception actualException, Tuple<Type, string>[] expectedExceptions)
        {
            string expectedExceptionsParameter;

            if (expectedExceptions.Length == 1)
            {
                expectedExceptionsParameter =
                    string.Format("{0}:{1}", expectedExceptions[0].Item1, expectedExceptions[0].Item2);
            }
            else
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("{ ");
                for (int i = 0; i < expectedExceptions.Length; i++)
                {
                    string.Format("{0}:{1}", expectedExceptions[i].Item1, expectedExceptions[i].Item2);

                    if (i < expectedExceptions.Length - 1)
                    {
                        sb.Append(" | ");
                    }
                }
                sb.Append(" }");

                expectedExceptionsParameter = sb.ToString();
            }

            Assert.Fail(String.Format("Exception not found. '{0}'. Actual exception thrown: '{1}'",
                expectedExceptionsParameter,
                actualException != null ? actualException.ToString() : "<NULL>"));
        }
    }
}

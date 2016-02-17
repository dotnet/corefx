// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;

using Xunit;

#pragma warning disable 0067

namespace System.Reflection.Tests
{
    public class EventInfoTests
    {
        [Theory]
        [InlineData(typeof(BaseClass), "EventPublic", "add_EventPublic")]
        [InlineData(typeof(BaseClass), "EventPublicStatic", "add_EventPublicStatic")]
        [InlineData(typeof(SubClass), "EventPublic", "add_EventPublic")]
        [InlineData(typeof(SubClass), "EventPublicNew", "add_EventPublicNew")]
        public void AddMethod(Type type, string name, string expectedName)
        {
            MethodInfo methodInfo = GetEventInfo(type, name).AddMethod;
            Assert.Equal(expectedName, methodInfo.Name);
        }

        public static IEnumerable<object[]> AddRemoveEventHandler_TestData()
        {
            yield return new object[] { typeof(BaseClass), "EventPublic", new BaseClass() };
            yield return new object[] { typeof(BaseClass), "EventPublicStatic", null };
            yield return new object[] { typeof(BaseClass), "EventPublicVirtual", new BaseClass() };
            yield return new object[] { typeof(SubClass), "EventPublic", new SubClass() };
            yield return new object[] { typeof(SubClass), "EventPublicNew", new SubClass() };
        }

        [Theory]
        [MemberData("AddRemoveEventHandler_TestData")]
        public void AddRemoveEventHandler(Type type, string name, object obj)
        {
            EventInfo eventInfo = GetEventInfo(type, name);
            EventHandler myHandler = new EventHandler(MyEventHandler);
            eventInfo.AddEventHandler(obj, myHandler);
            eventInfo.RemoveEventHandler(obj, myHandler);
        }

        public static IEnumerable<object[]> AddEventHandler_Invalid_TestData()
        {
            // Target is null (Win8P throws generic Exception)
            yield return new object[] { typeof(BaseClass), "EventPublic", null, new EventHandler(MyEventHandler), typeof(Exception) };

            // Handler is not correct (Win8P throws generic Exception)
            yield return new object[] { typeof(BaseClass), "EventPublic", "hello", new EventHandler(MyEventHandler), typeof(Exception) };

            // Handler is not correct
            yield return new object[] { typeof(BaseClass), "EventPublic", new BaseClass(), new SomeDelegate(SomeHandler), typeof(ArgumentException) };
        }

        [Theory]
        [MemberData("AddEventHandler_Invalid_TestData")]
        public void AddEventHandler_Invalid(Type type, string name, object target, Delegate handler, Type exceptionType)
        {
            EventInfo eventInfo = GetEventInfo(type, name);
            if (exceptionType == typeof(Exception))
            {
                Assert.ThrowsAny<Exception>(() => eventInfo.AddEventHandler(target, handler));
            }
            else
            {
                Assert.Throws(exceptionType, () => eventInfo.AddEventHandler(target, handler));
            }
        }

        [Theory]
        [InlineData(typeof(BaseClass), "EventPublic", EventAttributes.None)]
        [InlineData(typeof(BaseClass), "EventPublicStatic", EventAttributes.None)]
        [InlineData(typeof(BaseClass), "EventPublicVirtual", EventAttributes.None)]
        [InlineData(typeof(SubClass), "EventPublic", EventAttributes.None)]
        [InlineData(typeof(SubClass), "EventPublicNew", EventAttributes.None)]
        public void Attribute(Type type, string name, EventAttributes expected)
        {
            EventInfo eventInfo = GetEventInfo(type, name);
            Assert.Equal(expected, eventInfo.Attributes);
        }

        [Theory]
        [InlineData(typeof(BaseClass), "EventPublic", typeof(EventHandler))]
        [InlineData(typeof(BaseClass), "EventPublicStatic", typeof(EventHandler))]
        [InlineData(typeof(SubClass), "EventPublic", typeof(EventHandler))]
        [InlineData(typeof(SubClass), "EventPublicNew", typeof(EventHandler))]
        [InlineData(typeof(BaseClass), "EventPublicVirtual", typeof(EventHandler))]
        public void EventHandler(Type type, string name, Type expected)
        {
            EventInfo eventInfo = GetEventInfo(type, name);
            Assert.Equal(expected, eventInfo.EventHandlerType);
        }

        [Theory]
        [InlineData(typeof(BaseClass), "EventPublic")]
        [InlineData(typeof(BaseClass), "EventPublicStatic")]
        public void RaiseMethod(Type type, string name)
        {
            // C# compiler generated have no associated raise method, so this always returns null.
            EventInfo eventInfo = GetEventInfo(type, name);
            Assert.Null(eventInfo.RaiseMethod);
        }

        [Theory]
        [InlineData(typeof(BaseClass), "EventPublic", "remove_EventPublic")]
        [InlineData(typeof(BaseClass), "EventPublicStatic", "remove_EventPublicStatic")]
        [InlineData(typeof(SubClass), "EventPublic", "remove_EventPublic")]
        [InlineData(typeof(SubClass), "EventPublicNew", "remove_EventPublicNew")]
        public void RemoveMethod(Type type, string name, string expectedName)
        {
            MethodInfo methodInfo = GetEventInfo(type, name).RemoveMethod;
            Assert.Equal(expectedName, methodInfo.Name);
        }

        [Theory]
        [InlineData(typeof(BaseClass), "EventPublic", false)]
        [InlineData(typeof(BaseClass), "EventPublicVirtual", false)]
        public void IsSpecialName(Type type, string name, bool expected)
        {
            EventInfo eventInfo = GetEventInfo(type, name);
            Assert.Equal(expected, eventInfo.IsSpecialName);
        }

        public static IEnumerable<object[]> RemoveEventHandler_Invalid_TestData()
        {
            // Target is null (Win8P throws generic Exception)
            yield return new object[] { typeof(BaseClass), "EventPublic",
                new BaseClass(), new EventHandler(MyEventHandler), null, new EventHandler(MyEventHandler), typeof(Exception) };

            // Target is not correct (Win8P throws generic Exception)
            yield return new object[] { typeof(BaseClass), "EventPublic",
                new BaseClass(), new EventHandler(MyEventHandler), "hello", new EventHandler(MyEventHandler), typeof(Exception) };

            // Handler is not correct
            yield return new object[] { typeof(BaseClass), "EventPublic",
                new BaseClass(), new EventHandler(MyEventHandler), new BaseClass(), new SomeDelegate(SomeHandler), typeof(ArgumentException) };
        }

        [Theory]
        [MemberData("RemoveEventHandler_Invalid_TestData")]
        public void RemovEventHandler_Invalid(Type type, string name, object addTarget, Delegate addHandler, object removeTarget, Delegate removeHandler, Type exceptionType)
        {
            EventInfo ei = GetEventInfo(type, name);
            ei.AddEventHandler(addTarget, addHandler);

            if (exceptionType == typeof(Exception))
            {
                Assert.ThrowsAny<Exception>(() => ei.RemoveEventHandler(removeTarget, removeHandler));
            }
            else
            {
                Assert.Throws(exceptionType, () => ei.RemoveEventHandler(removeTarget, removeHandler));
            }
        }

        [Theory]
        [InlineData(typeof(BaseClass), "EventPublic", typeof(BaseClass), "EventPublic", true)]
        [InlineData(typeof(BaseClass), "EventPublic", typeof(SubClass), "EventPublic", false)]
        [InlineData(typeof(BaseClass), "EventPublic", typeof(BaseClass), "EventPublicStatic", false)]
        public void Equals(Type type1, string name1, Type type2, string name2, bool expected)
        {
            EventInfo eventInfo1 = GetEventInfo(type1, name1);
            EventInfo eventInfo2 = GetEventInfo(type2, name2);
            Assert.Equal(expected, eventInfo1.Equals(eventInfo2));
            Assert.Equal(expected, eventInfo1.GetHashCode().Equals(eventInfo2.GetHashCode()));
        }

        private static EventInfo GetEventInfo(Type type, string name)
        {
            return type.GetTypeInfo().DeclaredEvents.FirstOrDefault(methodInfo => methodInfo.Name.Equals(name));
        }

        private static void MyEventHandler(object o, EventArgs e) { }
        private static void SomeHandler(object o) { }
        public delegate void SomeDelegate(object o);

        // Metadata for reflection
        public class BaseClass
        {
            public event EventHandler EventPublic;
            public static event EventHandler EventPublicStatic;
            public virtual event EventHandler EventPublicVirtual;
        }

        public class SubClass : BaseClass
        {
            public new event EventHandler EventPublic; // Overrides event
            public event EventHandler EventPublicNew; // New event
        }
    }
}

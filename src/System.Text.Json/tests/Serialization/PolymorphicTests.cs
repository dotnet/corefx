﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static class PolymorphicTests
    {
        [Fact]
        public static void PrimitivesAsRootObject()
        {
            string json = JsonSerializer.ToString<object>(1);
            Assert.Equal("1", json);

            json = JsonSerializer.ToString(1, typeof(object));
            Assert.Equal("1", json);

            json = JsonSerializer.ToString<object>("foo");
            Assert.Equal(@"""foo""", json);

            json = JsonSerializer.ToString("foo", typeof(object));
            Assert.Equal(@"""foo""", json);
        }

        [Fact]
        public static void ArrayAsRootObject()
        {
            const string ExpectedJson = @"[1,true,{""City"":""MyCity""},{},""foo""]";

            Address address = new Address();
            address.Initialize();

            object[] array = new object[] { 1, true, address, null, "foo" };
            string json = JsonSerializer.ToString(array);
            Assert.Equal(ExpectedJson, json);

            json = JsonSerializer.ToString<object>(array);
            Assert.Equal(ExpectedJson, json);

            List<object> list = new List<object> { 1, true, address, null, "foo" };
            json = JsonSerializer.ToString(list);
            Assert.Equal(ExpectedJson, json);

            json = JsonSerializer.ToString<object>(list);
            Assert.Equal(ExpectedJson, json);
        }

        [Fact]
        public static void SimpleTestClassAsRootObject()
        {
            // Sanity checks on test type.
            Assert.Equal(typeof(object), typeof(SimpleTestClassWithObject).GetProperty("MyInt16").PropertyType);
            Assert.Equal(typeof(object), typeof(SimpleTestClassWithObject).GetProperty("MyBooleanTrue").PropertyType);
            Assert.Equal(typeof(object), typeof(SimpleTestClassWithObject).GetProperty("MyInt16Array").PropertyType);

            var obj = new SimpleTestClassWithObject();
            obj.Initialize();

            // Verify with actual type.
            string json = JsonSerializer.ToString(obj);
            Assert.Contains(@"""MyInt16"":1", json);
            Assert.Contains(@"""MyBooleanTrue"":true", json);
            Assert.Contains(@"""MyInt16Array"":[1]", json);

            // Verify with object type.
            json = JsonSerializer.ToString<object>(obj);
            Assert.Contains(@"""MyInt16"":1", json);
            Assert.Contains(@"""MyBooleanTrue"":true", json);
            Assert.Contains(@"""MyInt16Array"":[1]", json);
        }

        [Fact]
        public static void NestedObjectAsRootObject()
        {
            // Sanity checks on test type.
            Assert.Equal(typeof(object), typeof(ObjectWithObjectProperties).GetProperty("Address").PropertyType);
            Assert.Equal(typeof(object), typeof(ObjectWithObjectProperties).GetProperty("List").PropertyType);
            Assert.Equal(typeof(object), typeof(ObjectWithObjectProperties).GetProperty("Array").PropertyType);
            Assert.Equal(typeof(object), typeof(ObjectWithObjectProperties).GetProperty("NullableInt").PropertyType);
            Assert.Equal(typeof(object), typeof(ObjectWithObjectProperties).GetProperty("NullableIntArray").PropertyType);

            var person = new ObjectWithObjectProperties();

            string json = JsonSerializer.ToString(person);
            Assert.Equal(ObjectWithObjectProperties.ExpectedJson, json);

            json = JsonSerializer.ToString<object>(person);
            Assert.Equal(ObjectWithObjectProperties.ExpectedJson, json);
        }

        [Fact]
        public static void NestedObjectAsRootObjectIgnoreNullable()
        {
            // Ensure that null properties are properly written and support ignore.
            var person = new ObjectWithObjectProperties();
            person.NullableInt = null;

            string json = JsonSerializer.ToString(person);
            Assert.Equal(ObjectWithObjectProperties.ExpectedJsonNullInt, json);

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.IgnoreNullPropertyValueOnWrite = true;
            json = JsonSerializer.ToString(person, options);
            Assert.Equal(ObjectWithObjectProperties.ExpectedJsonNullIntIgnoreNulls, json);
        }

        [Fact]
        public static void StaticAnalysisBaseline()
        {
            Customer customer = new Customer();
            customer.Initialize();
            customer.Verify();

            string json = JsonSerializer.ToString(customer);
            Customer deserializedCustomer = JsonSerializer.Parse<Customer>(json);
            deserializedCustomer.Verify();
        }

        [Fact]
        public static void StaticAnalysis()
        {
            Customer customer = new Customer();
            customer.Initialize();
            customer.Verify();

            Person person = customer;

            // Generic inference used <TValue> = <Person>
            string json = JsonSerializer.ToString(person);

            Customer deserializedCustomer = JsonSerializer.Parse<Customer>(json);

            // We only serialized the Person base class, so the Customer fields should be default.
            Assert.Equal(typeof(Customer), deserializedCustomer.GetType());
            Assert.Equal(0, deserializedCustomer.CreditLimit);
            ((Person)deserializedCustomer).VerifyNonVirtual();
        }

        [Fact]
        public static void WriteStringWithRuntimeType()
        {
            Customer customer = new Customer();
            customer.Initialize();
            customer.Verify();

            Person person = customer;

            string json = JsonSerializer.ToString(person, person.GetType());

            Customer deserializedCustomer = JsonSerializer.Parse<Customer>(json);

            // We serialized the Customer
            Assert.Equal(typeof(Customer), deserializedCustomer.GetType());
            deserializedCustomer.Verify();
        }

        [Fact]
        public static void StaticAnalysisWithRelationship()
        {
            UsaCustomer usaCustomer = new UsaCustomer();
            usaCustomer.Initialize();
            usaCustomer.Verify();

            // Note: this could be typeof(UsaAddress) if we preserve objects created in the ctor. Currently we only preserve IEnumerables.
            Assert.Equal(typeof(Address), usaCustomer.Address.GetType());

            Customer customer = usaCustomer;

            // Generic inference used <TValue> = <Customer>
            string json = JsonSerializer.ToString(customer);

            UsaCustomer deserializedCustomer = JsonSerializer.Parse<UsaCustomer>(json);

            // We only serialized the Customer base class
            Assert.Equal(typeof(UsaCustomer), deserializedCustomer.GetType());
            Assert.Equal(typeof(Address), deserializedCustomer.Address.GetType());
            ((Customer)deserializedCustomer).VerifyNonVirtual();
       }
    }
}

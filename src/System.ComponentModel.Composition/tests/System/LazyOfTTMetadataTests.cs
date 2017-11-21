// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Factories;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.UnitTesting;
using Xunit;

namespace System
{
    public class LazyOfTMetadataTests
    {
        public class MetadataView
        {
        }

        [Fact]
        public void Constructor1_MetadataViewSet()
        {
            MetadataView metadataView = new MetadataView();
            var export = new Lazy<string, MetadataView>(() => "Value", metadataView, false);
            Assert.Equal(metadataView, export.Metadata);
        }

        [Fact]
        public void Constructor1_MetadataViewSetToNull()
        {
            MetadataView metadataView = new MetadataView();
            var export = new Lazy<string, MetadataView>(() => "Value", null, false);
            Assert.Null(export.Metadata);
        }

[Fact]
        public void Constructor1_NullAsExportedValueGetterArgument_ShouldThrowArgumentNull()
        {
            ExceptionAssert.ThrowsArgument<ArgumentNullException>("valueFactory", () =>
            {
                new Lazy<string, MetadataView>((Func<string>)null, new MetadataView(), false);
            });
        }

        [Fact]
        public void Constructor1_FuncReturningAStringAsExportedValueGetter_ShouldBeReturnedByGetExportedValue()
        {
            var export = new Lazy<string, MetadataView>(() => "Value", new MetadataView(), false);

            Assert.Equal("Value", export.Value);
        }

        [Fact]
        public void Constructor1_FuncReturningNullAsExportedValueGetter_ShouldBeReturnedByGetExportedValue()
        {
            var export = new Lazy<string, MetadataView>(() => null, new MetadataView(), false);

            Assert.Null(export.Value);
        }

        [Fact]
        public void Value_ShouldCacheExportedValueGetter()
        {
            int count = 0;

            var export = new Lazy<int, MetadataView>(() =>
            {
                count++;
                return count;
            }, new MetadataView(), false);

            Assert.Equal(1, export.Value);
            Assert.Equal(1, export.Value);
            Assert.Equal(1, export.Value);
        }
        [Fact]
        public void Constructor2_MetadataSet()
        {
            MetadataView metadataView = new MetadataView();
            var export = new Lazy<object, MetadataView>(metadataView, false);

            Assert.Same(metadataView, export.Metadata);
            Assert.NotNull(export.Value);
        }

        [Fact]
        public void Constructor3_MetadataSet()
        {
            MetadataView metadataView = new MetadataView();
            var export = new Lazy<object, MetadataView>(metadataView, true);

            Assert.Same(metadataView, export.Metadata);
            Assert.NotNull(export.Value);
        }

        [Fact]
        public void Constructor4_MetadataSet()
        {
            MetadataView metadataView = new MetadataView();
            var export = new Lazy<string, MetadataView>(() => "Value",
                metadataView, true);

            Assert.Same(metadataView, export.Metadata);
            Assert.NotNull(export.Value);
        }
    }
}

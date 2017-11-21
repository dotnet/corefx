using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.ComponentModel.Composition
{
    public interface ITrans_MetadataView
    {
        string Foo { get; }

        [System.ComponentModel.DefaultValue(null)]
        string OptionalFoo { get; }
    }

    public interface ITrans_MetadataViewWithDefaultedInt64
    {
        [DefaultValue(Int64.MaxValue)]
        Int64 MyInt64 { get; }
    }

    public interface ITrans_MetadataViewWithTypeMismatchDefaultValue
    {
        [DefaultValue("Strings can't cast to numbers")]
        int MyInt { get; }
    }

    public interface ITrans_MetadataViewWithDefaultedInt
    {
        [DefaultValue(120)]
        int MyInt { get; }
    }

    public interface ITrans_MetadataViewWithDefaultedBool
    {
        [DefaultValue(false)]
        bool MyBool { get; }
    }

    public interface ITrans_MetadataViewWithDefaultedString
    {
        [DefaultValue("MyString")]
        string MyString { get; }
    }

    public interface ITrans_MetadataViewUnboxAsInt
    {
        int Value { get; }
    }

    public interface ITrans_HasInt64
    {
        Int32 Value { get; }
    }
}

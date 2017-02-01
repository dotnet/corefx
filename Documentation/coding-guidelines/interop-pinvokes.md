P/Invokes
=========

This document extends the [Interop Guidelines](interop-guidelines.md) to provide more specific guidelines, notes, and resources for defining P/Invokes.

Guidelines
----------
1. Avoid StringBuilder
2. Use `ExactSpelling` where possible
3. Don't use `SetLastError` where not appropriate
4. Be careful of `BOOL` vs `BOOLEAN`
5. Watch for buffer size for output strings (null/no null)

Attributes
----------

**Implicit attributes applied to parameter and return values:**

|                  | Implicit Attribute |
|------------------|--------------------|
| parameter        | `[In]`             |
| `out` parameter  | `[Out]`            |
| `ref` parameter  | `[In],[Out]`       |
| return value     | `[Out]`            |

**`[DllImport()]` [1] attribute settings:**

| Setting | Recommendation | Details |
|---------|----------------|---------|
| [`PreserveSig`][2]   | keep default       | When this is explicitly set to false (the default is true), failed HRESULT return values will be turned into Exceptions (and the return value in the definition becomes null as a result).|
| [`SetLastError`][3]  | as per API         | Set this to true (default is false) if the API uses GetLastError and use Marshal.GetLastWin32Error to get the value. If the API sets a condition that says it has an error, get the error before making other calls to avoid inadvertently having it overwritten.|
| [`ExactSpelling`][4] | `true`             | Set this to true (deafult is false) and gain a slight perf benefit as the framework will avoid looking for an "A" or "W" version. (See NDirectMethodDesc::FindEntryPoint).|
| [`CharSet`][5]       | `CharSet.Unicode` if strings present in signature | This specifies marshalling behavior of strings and what `ExactSpelling` does when `false`. Be explicit with this one as the documented default is `CharSet.Ansi`.|

[1]: https://msdn.microsoft.com/en-us/library/system.runtime.interopservices.dllimportattribute.aspx "MSDN"
[2]: https://msdn.microsoft.com/en-us/library/system.runtime.interopservices.dllimportattribute.preservesig.aspx "MSDN"
[3]: https://msdn.microsoft.com/en-us/library/system.runtime.interopservices.dllimportattribute.setlasterror.aspx "MSDN"
[4]: https://msdn.microsoft.com/en-us/library/system.runtime.interopservices.dllimportattribute.exactspelling.aspx "MSDN"
[5]: https://msdn.microsoft.com/en-us/library/system.runtime.interopservices.dllimportattribute.charset.aspx "MSDN"

Strings
-------

When the CharSet is Unicode or the argument is explicitly marked as `[MarshalAs(UnmanagedType.LPWSTR)]` _and_ the string is
is passed by value (not `ref` or `out`) the string will be be pinned and used directly by native code (rather than copied).

Remember to mark the `[DllImport]` as `Charset.Unicode` unless you explicitly want ANSI treatment of your strings.

**[AVOID]** `StringBuilder` marshalling *always* creates a native buffer copy (see `ILWSTRBufferMarshaler`). As such it can be extremely inefficient. Take the typical
scenario of calling a Windows API that takes a string:

1. Create a SB of the desired capacity (allocates managed capacity) **{1}**
2. Invoke
   1. Allocates a native buffer **{2}**  
   2. Copies the contents if `[In]` _(the default for a `StringBuilder` parameter)_  
   3. Copies the native buffer into a newly allocated managed array if `[Out]` **{3}** _(also the default for `StringBuilder`)_  
3. `ToString()` allocates yet another managed array **{4}**

That is *{4}* allocations to get a string out of native code. The best you can do to limit this is to reuse the `StringBuilder`
in another call but this still only saves *1* allocation. It is much better to use and cache a native buffer- you can then
get down to just the allocation for the `ToString()` on subsequent calls.

The other issue with `StringBuilder` is that it always copies the return buffer back up to the first null. If the passed back string isn't terminated or is a double-null-terminated string your P/Invoke is incorrect at best.

If you *do* use `StringBuilder` one last gotcha is that the capacity does **not** include a hidden null which is always accounted for in interop. It is pretty common for people to get this wrong as most APIs want the size of the buffer *including* the null. This can result in wasted/unnecessary allocations.

**[USE]** Char arrays from `ArrayPool` or `StringBuffer`.

[Default Marshalling for Strings](https://msdn.microsoft.com/en-us/library/s9ts558h.aspx "MSDN")

> ### Windows Specific

> For `[Out]` strings the CLR will use `CoTaskMemFree` by default to free strings or `SysStringFree` for strings that are marked
as `UnmanagedType.BSTR`.

> **For most APIs with an output string buffer:**

> The passed in character count must include the null. If the returned value is less than the passed in character count the call has succeeded and the value is the number of characters *without* the trailing null. Otherwise the count is the required size of the buffer *including* the null character.

> - Pass in 5, get 4: The string is 4 characters long with a trailing null.
> - Pass in 5, get 6: The string is 5 characters long, need a 6 character buffer to hold the null.

> [Windows Data Types for Strings](http://msdn.microsoft.com/en-us/library/dd374131.aspx "MSDN")

Booleans
--------

Booleans are easy to mess up. The default marshalling for P/Invoke is as the Windows type `BOOL`, where it is a 4 byte value. `BOOLEAN`, however, is a *single* byte. This can lead to hard to track down bugs as half the return value will be discarded, which will only *potentially* change the result. You need to use `[MarshalAs(UnmanagedType.U1)]` or `[MarshalAs(UnmanagedType.I1)]` either should work as `TRUE` is defined as `1` and `FALSE` is defined as `0`. `U1` is technically more correct as it is defined as an `unsigned char`.

For COM (`VARIANT_BOOL`) the type is `2` bytes where true is `-1` and false is `0`. Marshalling uses this by default for bool in COM calls (`UnmanagedType.VariantBool`).

[Default Marshalling for Boolean Types](https://msdn.microsoft.com/en-us/library/t2t3725f.aspx "MSDN")  

Guids
-----

Guids are usable directly in signatures. When passed by ref, however, they should *not* be marked as `out` or `ref`. Instead the parameter should get the `[MarshalAs(UnmanagedType.LPStruct)]` attribute.

| Guid | By ref Guid |
|------|-------------|
| `KNOWNFOLDERID` | `REFKNOWNFOLDERID` |

`[MarshalAs(UnmanagedType.LPStruct)]` should _only_ be used for by ref Guids.

Common Windows Data Types
-------------------------


|Windows        | C                 | C#    | Alternative |
|---------------|-------------------|-------|-------------|
|`BOOL`           |`int`                |`int`    |`bool`
|`BOOLEAN`        |`unsigned char`      |`byte`   |`[MarshalAs(UnmanagedType.U1)] bool`
|`BYTE`           |`unsigned char`      |`byte` | |
|`CHAR`           |`char`               |`sbyte` | |
|`UCHAR`          |`unsigned char`      |`byte` | |
|`SHORT`          |`short`              |`short` | |
|`CSHORT`         |`short`              |`short` | |
|`USHORT`         |`unsigned short`     |`ushort` | |
|`WORD`           |`unsigned short`     |`ushort` | |
|`ATOM`           |`unsigned short`     |`ushort` | |
|`INT`            |`int`                |`int` | |
|`LONG`           |`long`               |`int` | |
|`ULONG`          |`unsigned long`      |`uint` | |
|`DWORD`          |`unsigned long`      |`uint` | |
|`LARGE_INTEGER`  |`__int64`            |`long` | |
|`LONGLONG`       |`__int64`            |`long` | |
|`ULONGLONG`      |`unsigned __int64`   |`ulong` | |
|`ULARGE_INTEGER` |`unsigned __int64`   |`ulong` | |
|`UCHAR`          |`unsigned char`      |`byte` | |
|`HRESULT`        |`long`               |`int` | |


| Signed Pointer Types (`IntPtr`) | Unsigned Pointer Types (`UIntPtr`) |
|----------------------------------|-------------------------------------|
| `HANDLE` | `WPARAM` |
| `HWND` | `UINT_PTR` |
| `HINSTANCE` | `ULONG_PTR` |
| `LPARAM` | `SIZE_T` |
| `LRESULT` | |
| `LONG_PTR` | |
| `INT_PTR` | |

[Windows Data Types](http://msdn.microsoft.com/en-us/library/aa383751.aspx "MSDN")  
[Data Type Ranges](http://msdn.microsoft.com/en-us/library/s3f49ktz.aspx "MSDN")

Blittable Types
---------------
Blittable types are types that have the same representation for native code. As such they do not need to be converted to another format to be marshalled to and from native code.

**Blittable types:**

- `byte`, `sbyte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `single`, `double`
- non-nested one dimensional arrays of blittable types (e.g. `int[]`)
- structs and classes with fixed layout that only have blittable types for instance fields
  - fixed layout requires `[StructLayout(LayoutKind.Sequential)]` or `[StructLayout(LayoutKind.Explicit)]`
  - structs are `LayoutKind.Sequential` by default, classes are `LayoutKind.Auto`

**NOT blittable:**

- `bool`

**SOMETIMES blittable:**

- `char`, `string`

When blittable types are passed by reference they are simply pinned by the marshaller instead of being copied to an intermediate buffer. (Classes are inherently passed by reference, structs are passed by reference when used with `ref` or `out`.)

`char` is blittable in a one dimensional array **or** if it is part of a type that contains it is explicitly marked with `[StructLayout]` with `CharSet = CharSet.Unicode`.

```C#
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
public struct UnicodeCharStruct
{
    public char c;
}
```

`string` is blittable if it isn't contained in another type and it's being passed as an argument that is marked with `[MarshalAs(UnmanagedType.LPWStr)]` or the `[DllImport]` has `CharSet = CharSet.Unicode` set.

You can see if a type is blittable by attempting to create a pinned `GCHandle`. If the type is not a string or considered blittable `GCHandle.Alloc` will throw an `ArgumentException`.


[Blittable and Non-Blittable Types](https://msdn.microsoft.com/en-us/library/75dwhxf7.aspx "MSDN")  
[Default Marshalling for Value Types](https://msdn.microsoft.com/en-us/library/0t2cwe11.aspx "MSDN")

Keeping Managed Objects Alive
-----------------------------
`GC.KeepAlive()` will ensure an object stays in scope until the KeepAlive method is hit.

[`HandleRef`][6] allows the marshaller to keep an object alive for the duration of a P/Invoke. It can be used instead of `IntPtr` in method signatures.

[6]: https://msdn.microsoft.com/en-us/library/system.runtime.interopservices.handleref.aspx "MSDN"

[`GCHandle`][7] allows pinning a managed object and getting the native pointer to it. Basic pattern is:  

``` C#
GCHandle handle = GCHandle.Alloc(obj, GCHandleType.Pinned);
IntPtr ptr = handle.AddrOfPinnedObject();
handle.Free();
```

[7]: https://msdn.microsoft.com/en-us/library/system.runtime.interopservices.gchandle.aspx "MSDN"

Pinning is not the default for `GCHandle`. The other major pattern is for passing a reference to a managed object through native code back to managed code (via a callback, typically). Here is the pattern:

``` C#
GCHandle handle = GCHandle.Alloc(obj);
SomeNativeEnumerator(callbackDelegate, GCHandle.ToIntPtr(handle));

// In the callback
GCHandle handle = GCHandle.FromIntPtr(param);
object managedObject = handle.Target;
```

Structs
-------

Managed structs are created on the stack and aren't removed until the method returns. By definition then, they are "pinned" (it won't get moved by the GC). You can also simply take the address in unsafe code blocks if native code won't use the pointer past the end of the current method. 

Class fields in structs are marshalled as pointers (`IntPtr`) _unless_ the class has explicit layout, in which case the class fields are embedded in the marshalling buffer. For example:

``` C#
public struct MyStruct
{
    public class MyClass;
}
```

`Marshal.SizeOf<MyStruct>()` will return `sizeof(IntPtr)` unless `MyClass` has explicit layout (via `[StructLayout]`). If `MyClass` has explicit layout `Marshal.SizeOf<MyStruct>()` will return `Marshal.SizeOf<MyClass>()` in this case (as `MyStruct` has no other fields).

Pointers to structs in definitions must either be passed by `ref` or use `unsafe` and `*`.


Other References
----------------

[MarshalAs Attribute](http://msdn.microsoft.com/en-us/library/system.runtime.interopservices.marshalasattribute.aspx "MSDN")  
[GetLastError and managed code](http://blogs.msdn.com/b/adam_nathan/archive/2003/04/25/56643.aspx "MSDN")  
[Copying and Pinning](https://msdn.microsoft.com/en-us/library/23acw07k.aspx "MSDN")  
[Marshalling between Managed and Unmanaged Code (MSDN Magazine January 2008)](http://download.microsoft.com/download/3/A/7/3A7FA450-1F33-41F7-9E6D-3AA95B5A6AEA/MSDNMagazineJanuary2008en-us.chm) *This is a .chm download*  

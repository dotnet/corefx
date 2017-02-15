// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Buffers;

namespace System
{
    internal struct PooledCharArray
    {
    	private char[] _buffer;

    	public PooledCharArray(int size)
    	{
    		_buffer = ArrayPool<char>.Shared.Rent(size);
    	}

    	public void Release()
    	{
    		if (_buffer != null)
    		{
    			ArrayPool<char>.Shared.Return(_buffer);
    			_buffer = null;
    		}
    	}

    	public int Length => _buffer.Length;

    	public string GetStringAndRelease(int stringLength)
    	{
    		string ret = new string(_buffer, 0, stringLength);
    		Release();
    		return ret;
    	}

    	public char this[int index]
    	{
    		get
    		{
    			return _buffer[index];
    		}
    		set
    		{
    			_buffer[index] = value;
    		}
    	}

    	public void GrowAndCopy(int extraSpace)
    	{
    		char[] oldBuffer = _buffer;
    		char[] newBuffer = ArrayPool<char>.Shared.Rent(_buffer.Length + extraSpace);
    		Buffer.BlockCopy(oldBuffer, 0, newBuffer, 0, _buffer.Length);
    		_buffer = newBuffer;
    		ArrayPool<char>.Shared.Return(oldBuffer);
    	}
    }
}
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace System.Data.Common
{
    public abstract class DbSchemaFactory
    {
        abstract public List<DbColumn> GetColumnSchema(DbDataReader dataReader);
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Data.Common;
using System.Security;

namespace System.Data.SqlClient
{
    public sealed class SqlCredential
    {
        string _userId;
        SecureString _password;

        public SqlCredential(string userId, SecureString password)
        {
            if (userId == null)
            {
                throw ADP.ArgumentNull("userId");
            }

            if (userId.Length > TdsEnums.MAXLEN_USERNAME)
            {
                throw ADP.InvalidArgumentLength("userId", TdsEnums.MAXLEN_USERNAME);
            }

            if (password == null)
            {
                throw ADP.ArgumentNull("password");
            }

            if (password.Length > TdsEnums.MAXLEN_PASSWORD)
            {
                throw ADP.InvalidArgumentLength("password", TdsEnums.MAXLEN_PASSWORD);
            }

            if (!password.IsReadOnly())
            {
                throw ADP.MustBeReadOnly("password");
            }

            _userId = userId;
            _password = password;
        }

        public string UserId
        {
            get
            {
                return _userId;
            }
        }

        public SecureString Password
        {
            get
            {
                return _password;
            }
        }
    }
}

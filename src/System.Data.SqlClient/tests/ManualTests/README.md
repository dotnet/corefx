# SqlClient Manual Tests

These tests require dedicated test servers, so they're designed to be run manually using a custom set of connection strings. These connection strings should be added as environment variables: TEST_NP_CONN_STR & TEST_TCP_CONN_STR. TEST_NP_CONN_STR is a named pipes connection string to the test server, and TEST_TCP_CONN_STR is a TCP connection string. Each protocol can be specified in the Server name parameter. These tests also assume the sample database "Northwind" exists in the target server. This sample database can be found [here](https://msdn.microsoft.com/en-us/library/mt710790.aspx).

Instructions for running tests: [Unix](https://github.com/dotnet/corefx/blob/master/Documentation/building/cross-platform-testing.md) and [Windows](https://github.com/dotnet/corefx/blob/master/Documentation/building/windows-instructions.md).
Documentation for connection string parameters: [SqlConnection.ConnectionString](https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlconnection.connectionstring.aspx).

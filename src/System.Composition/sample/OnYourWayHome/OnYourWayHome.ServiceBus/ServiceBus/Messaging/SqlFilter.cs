//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus.Messaging
{
    using System;
    using System.Runtime.Serialization;

    [DataContract(Name = "SqlFilter", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
    [KnownType(typeof(TrueFilter))]
    [KnownType(typeof(FalseFilter))]
    public class SqlFilter : Filter
    {
        public const int DefaultCompatibilityLevel = 20;

        public SqlFilter(string sqlExpression)
        {
            if (String.IsNullOrEmpty(sqlExpression))
            {
                throw new ArgumentNullException("sqlExpression");
            }

            this.SqlExpression = sqlExpression;
            this.CompatibilityLevel = DefaultCompatibilityLevel;
        }

        [DataMember(Order = 0x10001)]
        public string SqlExpression { get; private set; }
        
        [DataMember(Order = 0x10002)]
        public int CompatibilityLevel { get; private set; }
    }
}

//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus.Messaging
{
    using System;
    using System.Runtime.Serialization;

    [DataContract(Name = "SqlRuleAction", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
    public sealed class SqlRuleAction : RuleAction
    {
        public const int DefaultCompatibilityLevel = 20;

        public SqlRuleAction(string sqlExpression)
        {
            if (string.IsNullOrEmpty(sqlExpression))
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
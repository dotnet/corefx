//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus.Messaging
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Text;
    using OnYourWayHome.ServiceBus.Serialization;

    [DataContract]
    public sealed class BrokerProperties
    {
        private static readonly IDataContractSerializer serializer = ServiceBusAdapter.Current.CreateJsonSerializer(typeof(BrokerProperties));

        [DataMember(EmitDefaultValue = false)]
        public string CorrelationId { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string SessionId { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public int? DeliveryCount { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public Guid? LockToken { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string MessageId { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string Label { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string ReplyTo { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public long? SequenceNumber { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string To { get; set; }

        public DateTime? LockedUntilUtc { get; set; }

        public DateTime? ScheduledEnqueueTimeUtc { get; set; }

        public TimeSpan? TimeToLive { get; set; }

        [DataMember(EmitDefaultValue = false)]
        public string ReplyToSessionId { get; set; }

        [DataMember(Name = "LockedUntilUtc", EmitDefaultValue = false)]
        public string LockedUntilUtcString
        {
            get
            {
                if (this.LockedUntilUtc != null && this.LockedUntilUtc.HasValue)
                {
                    return this.LockedUntilUtc.Value.ToString("r");
                }

                return null;
            }

            set
            {
                try
                {
                    // When deserializing from JSON format, attempt to parse the value. 
                    // If the value cannot be parsed as a date time, ignore it.
                    this.LockedUntilUtc = DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                }
                catch
                {
                }
            }
        }

        [DataMember(Name = "ScheduledEnqueueTimeUtc", EmitDefaultValue = false)]
        public string ScheduledEnqueueTimeUtcString
        {
            get
            {
                if (this.ScheduledEnqueueTimeUtc != null && this.ScheduledEnqueueTimeUtc.HasValue)
                {
                    return this.ScheduledEnqueueTimeUtc.Value.ToString("r");
                }

                return null;
            }

            set
            {
                try
                {
                    // When deserializing from JSON format, attempt to parse the value. 
                    // If the value cannot be parsed as a date time, ignore it.
                    this.ScheduledEnqueueTimeUtc = DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                }
                catch
                {
                }
            }
        }

        [DataMember(Name = "TimeToLive", EmitDefaultValue = false)]
        public double TimeToLiveString
        {
            get
            {
                if (this.TimeToLive != null && this.TimeToLive.HasValue)
                {
                    return this.TimeToLive.Value.TotalSeconds;
                }

                return 0;
            }

            set
            {
                // This is needed as TimeSpan.FromSeconds(TimeSpan.MaxValue.TotalSeconds) throws Overflow exception.
                if (TimeSpan.MaxValue.TotalSeconds == value)
                {
                    this.TimeToLive = TimeSpan.MaxValue;
                }
                else
                {
                    this.TimeToLive = TimeSpan.FromSeconds(value);
                }
            }
        }

        public static BrokerProperties Deserialize(string jsonString)
        {
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                return (BrokerProperties)serializer.ReadObject(ms);
            }
        }

        public string Serialize()
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                serializer.WriteObject(memoryStream, this);
                
                memoryStream.Position = 0;
                using (StreamReader reader = new StreamReader(memoryStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}

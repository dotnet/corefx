//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus.Messaging
{
    using System;
    using System.Runtime.Serialization;

    [DataContract(Name = "QueueDescription", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
    public sealed class QueueDescription
    {
        [DataMember(Name = "LockDuration", IsRequired = false, Order = 1002, EmitDefaultValue = false)]
        public TimeSpan? LockDuration { get; set; }

        [DataMember(Name = "MaxSizeInMegabytes", IsRequired = false, Order = 1004, EmitDefaultValue = false)]
        public long? MaxSizeInMegabytes { get; set; }

        [DataMember(Name = "RequiresDuplicateDetection", IsRequired = false, Order = 1005, EmitDefaultValue = false)]
        public bool? RequiresDuplicateDetection { get; set; }

        [DataMember(Name = "RequiresSession", IsRequired = false, Order = 1006, EmitDefaultValue = false)]
        public bool? RequiresSession { get; set; }

        [DataMember(Name = "DefaultMessageTimeToLive", IsRequired = false, Order = 1007, EmitDefaultValue = false)]
        public TimeSpan? DefaultMessageTimeToLive { get; set; }

        [DataMember(Name = "DeadLetteringOnMessageExpiration", IsRequired = false, Order = 1008, EmitDefaultValue = false)]
        public bool? EnableDeadLetteringOnMessageExpiration { get; set; }

        [DataMember(Name = "DuplicateDetectionHistoryTimeWindow", IsRequired = false, Order = 1009, EmitDefaultValue = false)]
        public TimeSpan? DuplicateDetectionHistoryTimeWindow { get; set; }

        [DataMember(Name = "MaxDeliveryCount", IsRequired = false, Order = 1010, EmitDefaultValue = false)]
        public int? MaxDeliveryCount { get; set; }

        [DataMember(Name = "EnableBatchedOperations", IsRequired = false, Order = 1011, EmitDefaultValue = false)]
        public bool? EnableBatchedOperations { get; set; }

        [DataMember(Name = "SizeInBytes", IsRequired = false, Order = 1012, EmitDefaultValue = false)]
        public long? SizeInBytes { get; set; }

        [DataMember(Name = "MessageCount", IsRequired = false, Order = 1013, EmitDefaultValue = false)]
        public long? MessageCount { get; set; }

        public string Path { get; private set; }

        public Uri Uri { get; private set; }

        internal void UpdateFromEntity(Entity<QueueDescription> entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            this.Path = entity.Name;
            this.Uri = entity.Uri;
        }
    }
}

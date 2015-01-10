//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.ServiceBus.Messaging
{
    using System;
    using System.Runtime.Serialization;

    [DataContract(Name = "TopicDescription", Namespace = "http://schemas.microsoft.com/netservices/2010/10/servicebus/connect")]
    public sealed class TopicDescription
    {        
        [DataMember(Name = "DefaultMessageTimeToLive", IsRequired = false, Order = 1002, EmitDefaultValue = false)]
        public TimeSpan? DefaultMessageTimeToLive { get; set; }

        [DataMember(Name = "MaxSizeInMegabytes", IsRequired = false, Order = 1004, EmitDefaultValue = false)]
        public long? MaxSizeInMegabytes { get; set; }

        [DataMember(Name = "RequiresDuplicateDetection", IsRequired = false, Order = 1005, EmitDefaultValue = false)]
        public bool? RequiresDuplicateDetection { get; set; }

        [DataMember(Name = "DuplicateDetectionHistoryTimeWindow", IsRequired = false, Order = 1006, EmitDefaultValue = false)]
        public TimeSpan? DuplicateDetectionHistoryTimeWindow { get; set; }

        [DataMember(Name = "EnableBatchedOperations", IsRequired = false, Order = 1007, EmitDefaultValue = false)]
        public bool? EnableBatchedOperations { get; set; }

        [DataMember(Name = "SizeInBytes", IsRequired = false, Order = 1008, EmitDefaultValue = false)]
        public long? SizeInBytes { get; set; }

        public string Path { get; private set; }

        public Uri Uri { get; private set; }

        internal void UpdateFromEntity(Entity<TopicDescription> entity)
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

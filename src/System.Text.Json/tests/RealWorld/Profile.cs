// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Text.Json.Serialization;
using Xunit;

namespace System.Text.Json.Tests.RealWorld.Profile
{
    public class Test : RealWorldTest
    {
        public override void Initialize()
        {
            // This payload was obtained with permission from Layomi Akinrinade.
            PayloadAsString = File.ReadAllText("Resources/Payloads/Profile.json").StripWhitespace();
        }

        public override void VerifySerializer()
        {
            // TODO: Utilize string enum converter to prevent deserialization failure.

            //JsonSerializerOptions options = new JsonSerializerOptions
            //{
            //    IgnoreNullValues = true,
            //    WriteIndented = true,
            //};

            //options.Converters.Add(new JsonStringEnumConverter());

            //Profile profile = JsonSerializer.Deserialize<Profile>(PayloadAsString, options);

            //// TODO: drill into data to verify properties.
            //Assert.NotNull(profile);

            //// TDOD: compare to initial payload after Uri support is checked in.
            //Assert.NotNull(JsonSerializer.Serialize(profile, options));
        }

        public override void VerifyDocument() { }

        public override void VerifyReader() { }

        public override void VerifyWriter() { }
    }

    public class Profile
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("age_range")]
        public AgeRange AgeRange { get; set; }

        [JsonPropertyName("birthday")]
        public string Birthday { get; set; }

        [JsonPropertyName("can_review_measurement_request")]
        public bool CanReviewMeasurementRequest { get; set; }

        [JsonPropertyName("videos")]
        public Videos Videos { get; set; }

        [JsonPropertyName("accounts")]
        public Accounts Accounts { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("short_name")]
        public string ShortName { get; set; }

        [JsonPropertyName("posts")]
        public Posts Posts { get; set; }
    }

    public class Accounts
    {
        [JsonPropertyName("data")]
        public AccountsDatum[] Data { get; set; }

        [JsonPropertyName("paging")]
        public AccountsPaging Paging { get; set; }
    }

    public class AccountsDatum
    {
        [JsonPropertyName("fan_count")]
        public long FanCount { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }
    }

    public class AccountsPaging
    {
        [JsonPropertyName("cursors")]
        public Cursors Cursors { get; set; }
    }

    public class Cursors
    {
        [JsonPropertyName("before")]
        public string Before { get; set; }

        [JsonPropertyName("after")]
        public string After { get; set; }
    }

    public class AgeRange
    {
        [JsonPropertyName("min")]
        public long Min { get; set; }
    }

    public class Posts
    {
        [JsonPropertyName("data")]
        public PostsDatum[] Data { get; set; }

        [JsonPropertyName("paging")]
        public PostsPaging Paging { get; set; }
    }

    public class PostsDatum
    {
        [JsonPropertyName("updated_time")]
        public string UpdatedTime { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("instagram_eligibility")]
        public string InstagramEligibility { get; set; }

        [JsonPropertyName("promotion_status")]
        public PromotionStatus PromotionStatus { get; set; }

        [JsonPropertyName("status_type")]
        public StatusType StatusType { get; set; }

        [JsonPropertyName("timeline_visibility")]
        public TimelineVisibility TimelineVisibility { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("attachments")]
        public Attachments Attachments { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("comments")]
        public Comments Comments { get; set; }
    }

    public class Attachments
    {
        [JsonPropertyName("data")]
        public AttachmentsDatum[] Data { get; set; }
    }

    public class AttachmentsDatum
    {
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("media")]
        public PurpleMedia Media { get; set; }

        [JsonPropertyName("target")]
        public Target Target { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("type")]
        public TypeEnum Type { get; set; }

        [JsonPropertyName("url")]
        public Uri Url { get; set; }

        [JsonPropertyName("subattachments")]
        public Subattachments Subattachments { get; set; }
    }

    public class PurpleMedia
    {
        [JsonPropertyName("image")]
        public Image Image { get; set; }

        [JsonPropertyName("source")]
        public Uri Source { get; set; }
    }

    public class Image
    {
        [JsonPropertyName("height")]
        public long Height { get; set; }

        [JsonPropertyName("src")]
        public Uri Src { get; set; }

        [JsonPropertyName("width")]
        public long Width { get; set; }
    }

    public class Subattachments
    {
        [JsonPropertyName("data")]
        public SubattachmentsDatum[] Data { get; set; }
    }

    public class SubattachmentsDatum
    {
        [JsonPropertyName("media")]
        public FluffyMedia Media { get; set; }

        [JsonPropertyName("type")]
        public TypeEnum Type { get; set; }
    }

    public class FluffyMedia
    {
        [JsonPropertyName("image")]
        public Image Image { get; set; }
    }

    public class Target
    {
        [JsonPropertyName("url")]
        public Uri Url { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }
    }

    public class Comments
    {
        [JsonPropertyName("data")]
        public CommentsDatum[] Data { get; set; }

        [JsonPropertyName("paging")]
        public AccountsPaging Paging { get; set; }
    }

    public class CommentsDatum
    {
        [JsonPropertyName("can_remove")]
        public bool CanRemove { get; set; }

        [JsonPropertyName("created_time")]
        public string CreatedTime { get; set; }

        [JsonPropertyName("is_hidden")]
        public bool IsHidden { get; set; }

        [JsonPropertyName("user_likes")]
        public bool UserLikes { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }
    }

    public class PostsPaging
    {
        [JsonPropertyName("previous")]
        public Uri Previous { get; set; }

        [JsonPropertyName("next")]
        public Uri Next { get; set; }
    }

    public class Videos
    {
        [JsonPropertyName("data")]
        public VideosDatum[] Data { get; set; }

        [JsonPropertyName("paging")]
        public AccountsPaging Paging { get; set; }
    }

    public class VideosDatum
    {
        [JsonPropertyName("embeddable")]
        public bool Embeddable { get; set; }

        [JsonPropertyName("created_time")]
        public string CreatedTime { get; set; }

        [JsonPropertyName("format")]
        public Format[] Format { get; set; }

        [JsonPropertyName("is_crossposting_eligible")]
        public bool IsCrosspostingEligible { get; set; }

        [JsonPropertyName("is_crosspost_video")]
        public bool IsCrosspostVideo { get; set; }

        [JsonPropertyName("id")]
        public string Id { get; set; }
    }

    public class Format
    {
        [JsonPropertyName("embed_html")]
        public string EmbedHtml { get; set; }

        [JsonPropertyName("filter")]
        public string Filter { get; set; }

        [JsonPropertyName("height")]
        public long Height { get; set; }

        [JsonPropertyName("picture")]
        public Uri Picture { get; set; }

        [JsonPropertyName("width")]
        public long Width { get; set; }
    }

    public enum TypeEnum
    {
        Photo = 0,
        Share = 1,
        StreamPublish = 2,
        VideoAutoplay = 3
    };

    public enum PromotionStatus
    {
        Ineligible = 0,
    };

    public enum StatusType
    {
        AddedPhotos = 0,
        MobileStatusUpdate = 1,
        PublishedStory = 2,
        SharedStory = 3,
    };

    public enum TimelineVisibility
    {
        NoTimelineUnitForThisPost = 0,
        Normal = 1,
    };
}

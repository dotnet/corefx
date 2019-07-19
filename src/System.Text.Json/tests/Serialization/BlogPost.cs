// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace System.Text.Json.Serialization.Tests
{ 
    public class Post
    {
        [JsonPropertyName("value")]
        public Value Value { get; set; }

        [JsonPropertyName("mentionedUsers")]
        public List<MentionedUser> MentionedUsers { get; set; }

        [JsonPropertyName("collaborators")]
        public List<Collaborator> Collaborators { get; set; }

        [JsonPropertyName("hideMeter")]
        public bool HideMeter { get; set; }

        [JsonPropertyName("collectionUserRelations")]
        public List<CollectionUserRelation> CollectionUserRelations { get; set; }

        [JsonPropertyName("mode")]
        public object Mode { get; set; }

        [JsonPropertyName("references")]
        public References References { get; set; }
    }

    public class Collaborator
    {
        [JsonPropertyName("user")]
        public MentionedUser User { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }
    }

    public class MentionedUser
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("createdAt")]
        public long CreatedAt { get; set; }

        [JsonPropertyName("imageId")]
        public string ImageId { get; set; }

        [JsonPropertyName("backgroundImageId")]
        public string BackgroundImageId { get; set; }

        [JsonPropertyName("bio")]
        public string Bio { get; set; }

        [JsonPropertyName("twitterScreenName")]
        public string TwitterScreenName { get; set; }

        [JsonPropertyName("facebookAccountId")]
        public string FacebookAccountId { get; set; }

        [JsonPropertyName("allowNotes")]
        public long AllowNotes { get; set; }

        [JsonPropertyName("mediumMemberAt")]
        public long MediumMemberAt { get; set; }

        [JsonPropertyName("isNsfw")]
        public bool IsNsfw { get; set; }

        [JsonPropertyName("isWriterProgramEnrolled")]
        public bool IsWriterProgramEnrolled { get; set; }

        [JsonPropertyName("isQuarantined")]
        public bool IsQuarantined { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("socialStats")]
        public SocialStats SocialStats { get; set; }

        [JsonPropertyName("social")]
        public Social Social { get; set; }
    }

    public class Social
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [JsonPropertyName("targetUserId")]
        public string TargetUserId { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    public class SocialStats
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [JsonPropertyName("usersFollowedCount")]
        public long UsersFollowedCount { get; set; }

        [JsonPropertyName("usersFollowedByCount")]
        public long UsersFollowedByCount { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    public class CollectionUserRelation
    {
        [JsonPropertyName("collectionId")]
        public string CollectionId { get; set; }

        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [JsonPropertyName("role")]
        public string Role { get; set; }
    }

    public class References
    {
        [JsonPropertyName("User")]
        public User User { get; set; }

        [JsonPropertyName("Social")]
        public SocialClass Social { get; set; }

        [JsonPropertyName("SocialStats")]
        public SocialStatsClass SocialStats { get; set; }
    }

    public class SocialClass
    {
        [JsonPropertyName("7ef192b7f545")]
        public Social The7Ef192B7F545 { get; set; }
    }

    public class SocialStatsClass
    {
        [JsonPropertyName("7ef192b7f545")]
        public SocialStats The7Ef192B7F545 { get; set; }
    }

    public class User
    {
        [JsonPropertyName("7ef192b7f545")]
        public MentionedUser The7Ef192B7F545 { get; set; }
    }

    public class Value
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("versionId")]
        public string VersionId { get; set; }

        [JsonPropertyName("creatorId")]
        public string CreatorId { get; set; }

        [JsonPropertyName("homeCollectionId")]
        public string HomeCollectionId { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("detectedLanguage")]
        public string DetectedLanguage { get; set; }

        [JsonPropertyName("latestVersion")]
        public string LatestVersion { get; set; }

        [JsonPropertyName("latestPublishedVersion")]
        public string LatestPublishedVersion { get; set; }

        [JsonPropertyName("hasUnpublishedEdits")]
        public bool HasUnpublishedEdits { get; set; }

        [JsonPropertyName("latestRev")]
        public long LatestRev { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("acceptedAt")]
        public DateTime AcceptedAt { get; set; }

        [JsonPropertyName("firstPublishedAt")]
        public DateTime FirstPublishedAt { get; set; }

        [JsonPropertyName("latestPublishedAt")]
        public DateTime LatestPublishedAt { get; set; }

        [JsonPropertyName("vote")]
        public bool Vote { get; set; }

        [JsonPropertyName("experimentalCss")]
        public string ExperimentalCss { get; set; }

        [JsonPropertyName("displayAuthor")]
        public string DisplayAuthor { get; set; }

        [JsonPropertyName("content")]
        public Content Content { get; set; }

        [JsonPropertyName("virtuals")]
        public Virtuals Virtuals { get; set; }

        [JsonPropertyName("coverless")]
        public bool Coverless { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        [JsonPropertyName("translationSourcePostId")]
        public string TranslationSourcePostId { get; set; }

        [JsonPropertyName("translationSourceCreatorId")]
        public string TranslationSourceCreatorId { get; set; }

        [JsonPropertyName("isApprovedTranslation")]
        public bool IsApprovedTranslation { get; set; }

        [JsonPropertyName("inResponseToPostId")]
        public string InResponseToPostId { get; set; }

        [JsonPropertyName("inResponseToRemovedAt")]
        public long InResponseToRemovedAt { get; set; }

        [JsonPropertyName("isTitleSynthesized")]
        public bool IsTitleSynthesized { get; set; }

        [JsonPropertyName("allowResponses")]
        public bool AllowResponses { get; set; }

        [JsonPropertyName("importedUrl")]
        public string ImportedUrl { get; set; }

        [JsonPropertyName("importedPublishedAt")]
        public long ImportedPublishedAt { get; set; }

        [JsonPropertyName("visibility")]
        public long Visibility { get; set; }

        [JsonPropertyName("uniqueSlug")]
        public string UniqueSlug { get; set; }

        [JsonPropertyName("previewContent")]
        public PreviewContent PreviewContent { get; set; }

        [JsonPropertyName("license")]
        public long License { get; set; }

        [JsonPropertyName("inResponseToMediaResourceId")]
        public string InResponseToMediaResourceId { get; set; }

        [JsonPropertyName("canonicalUrl")]
        public Uri CanonicalUrl { get; set; }

        [JsonPropertyName("approvedHomeCollectionId")]
        public string ApprovedHomeCollectionId { get; set; }

        [JsonPropertyName("newsletterId")]
        public string NewsletterId { get; set; }

        [JsonPropertyName("webCanonicalUrl")]
        public Uri WebCanonicalUrl { get; set; }

        [JsonPropertyName("mediumUrl")]
        public Uri MediumUrl { get; set; }

        [JsonPropertyName("migrationId")]
        public string MigrationId { get; set; }

        [JsonPropertyName("notifyFollowers")]
        public bool NotifyFollowers { get; set; }

        [JsonPropertyName("notifyTwitter")]
        public bool NotifyTwitter { get; set; }

        [JsonPropertyName("notifyFacebook")]
        public bool NotifyFacebook { get; set; }

        [JsonPropertyName("responseHiddenOnParentPostAt")]
        public long ResponseHiddenOnParentPostAt { get; set; }

        [JsonPropertyName("isSeries")]
        public bool IsSeries { get; set; }

        [JsonPropertyName("isSubscriptionLocked")]
        public bool IsSubscriptionLocked { get; set; }

        [JsonPropertyName("seriesLastAppendedAt")]
        public long SeriesLastAppendedAt { get; set; }

        [JsonPropertyName("audioVersionDurationSec")]
        public long AudioVersionDurationSec { get; set; }

        [JsonPropertyName("sequenceId")]
        public string SequenceId { get; set; }

        [JsonPropertyName("isNsfw")]
        public bool IsNsfw { get; set; }

        [JsonPropertyName("isEligibleForRevenue")]
        public bool IsEligibleForRevenue { get; set; }

        [JsonPropertyName("isBlockedFromHightower")]
        public bool IsBlockedFromHightower { get; set; }

        [JsonPropertyName("deletedAt")]
        public DateTime DeletedAt { get; set; }

        [JsonPropertyName("lockedPostSource")]
        public long LockedPostSource { get; set; }

        [JsonPropertyName("hightowerMinimumGuaranteeStartsAt")]
        public long HightowerMinimumGuaranteeStartsAt { get; set; }

        [JsonPropertyName("hightowerMinimumGuaranteeEndsAt")]
        public long HightowerMinimumGuaranteeEndsAt { get; set; }

        [JsonPropertyName("featureLockRequestAcceptedAt")]
        public long FeatureLockRequestAcceptedAt { get; set; }

        [JsonPropertyName("mongerRequestType")]
        public long MongerRequestType { get; set; }

        [JsonPropertyName("layerCake")]
        public long LayerCake { get; set; }

        [JsonPropertyName("socialTitle")]
        public string SocialTitle { get; set; }

        [JsonPropertyName("socialDek")]
        public string SocialDek { get; set; }

        [JsonPropertyName("editorialPreviewTitle")]
        public string EditorialPreviewTitle { get; set; }

        [JsonPropertyName("editorialPreviewDek")]
        public string EditorialPreviewDek { get; set; }

        [JsonPropertyName("curationEligibleAt")]
        public long CurationEligibleAt { get; set; }

        [JsonPropertyName("primaryTopic")]
        public Topic PrimaryTopic { get; set; }

        [JsonPropertyName("primaryTopicId")]
        public string PrimaryTopicId { get; set; }

        [JsonPropertyName("isProxyPost")]
        public bool IsProxyPost { get; set; }

        [JsonPropertyName("proxyPostFaviconUrl")]
        public string ProxyPostFaviconUrl { get; set; }

        [JsonPropertyName("proxyPostProviderName")]
        public string ProxyPostProviderName { get; set; }

        [JsonPropertyName("proxyPostType")]
        public long ProxyPostType { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    public class Content
    {
        [JsonPropertyName("subtitle")]
        public string Subtitle { get; set; }

        [JsonPropertyName("bodyModel")]
        public ContentBodyModel BodyModel { get; set; }

        [JsonPropertyName("postDisplay")]
        public PostDisplay PostDisplay { get; set; }
    }

    public class ContentBodyModel
    {
        [JsonPropertyName("paragraphs")]
        public List<PurpleParagraph> Paragraphs { get; set; }

        [JsonPropertyName("sections")]
        public List<PurpleSection> Sections { get; set; }
    }

    public class PurpleParagraph
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public long Type { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("markups")]
        public List<Markup> Markups { get; set; }

        [JsonPropertyName("layout")]
        public long? Layout { get; set; }

        [JsonPropertyName("metadata")]
        public Image Metadata { get; set; }
    }

    public class Markup
    {
        [JsonPropertyName("type")]
        public long Type { get; set; }

        [JsonPropertyName("start")]
        public long Start { get; set; }

        [JsonPropertyName("end")]
        public long End { get; set; }

        [JsonPropertyName("href")]
        public Uri Href { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("rel")]
        public string Rel { get; set; }

        [JsonPropertyName("anchorType")]
        public long? AnchorType { get; set; }

        [JsonPropertyName("userId")]
        public string UserId { get; set; }
    }

    public class Image
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("originalWidth")]
        public long OriginalWidth { get; set; }

        [JsonPropertyName("originalHeight")]
        public long OriginalHeight { get; set; }

        [JsonPropertyName("isFeatured")]
        public bool? IsFeatured { get; set; }
    }

    public class PurpleSection
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("startIndex")]
        public long StartIndex { get; set; }
    }

    public class PostDisplay
    {
        [JsonPropertyName("coverless")]
        public bool Coverless { get; set; }
    }

    public class PreviewContent
    {
        [JsonPropertyName("bodyModel")]
        public PreviewContentBodyModel BodyModel { get; set; }

        [JsonPropertyName("isFullContent")]
        public bool IsFullContent { get; set; }

        [JsonPropertyName("subtitle")]
        public string Subtitle { get; set; }
    }

    public class PreviewContentBodyModel
    {
        [JsonPropertyName("paragraphs")]
        public List<FluffyParagraph> Paragraphs { get; set; }

        [JsonPropertyName("sections")]
        public List<FluffySection> Sections { get; set; }
    }

    public class FluffyParagraph
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("type")]
        public long Type { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("layout")]
        public long? Layout { get; set; }

        [JsonPropertyName("metadata")]
        public Image Metadata { get; set; }

        [JsonPropertyName("markups")]
        public List<Markup> Markups { get; set; }

        [JsonPropertyName("alignment")]
        public long? Alignment { get; set; }
    }

    public class FluffySection
    {
        [JsonPropertyName("startIndex")]
        public long StartIndex { get; set; }
    }

    public class Topic
    {
        [JsonPropertyName("topicId")]
        public string TopicId { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        [JsonPropertyName("createdAt")]
        public long CreatedAt { get; set; }

        [JsonPropertyName("deletedAt")]
        public long DeletedAt { get; set; }

        [JsonPropertyName("image")]
        public Image Image { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("relatedTopics")]
        public List<object> RelatedTopics { get; set; }

        [JsonPropertyName("visibility")]
        public long Visibility { get; set; }

        [JsonPropertyName("relatedTags")]
        public List<object> RelatedTags { get; set; }

        [JsonPropertyName("relatedTopicIds")]
        public List<object> RelatedTopicIds { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    public class Virtuals
    {
        [JsonPropertyName("allowNotes")]
        public bool AllowNotes { get; set; }

        [JsonPropertyName("previewImage")]
        public PreviewImage PreviewImage { get; set; }

        [JsonPropertyName("wordCount")]
        public long WordCount { get; set; }

        [JsonPropertyName("imageCount")]
        public long ImageCount { get; set; }

        [JsonPropertyName("readingTime")]
        public double ReadingTime { get; set; }

        [JsonPropertyName("subtitle")]
        public string Subtitle { get; set; }

        [JsonPropertyName("userPostRelation")]
        public UserPostRelation UserPostRelation { get; set; }

        [JsonPropertyName("usersBySocialRecommends")]
        public List<object> UsersBySocialRecommends { get; set; }

        [JsonPropertyName("noIndex")]
        public bool NoIndex { get; set; }

        [JsonPropertyName("recommends")]
        public long Recommends { get; set; }

        [JsonPropertyName("isBookmarked")]
        public bool IsBookmarked { get; set; }

        [JsonPropertyName("tags")]
        public List<Tag> Tags { get; set; }

        [JsonPropertyName("socialRecommendsCount")]
        public long SocialRecommendsCount { get; set; }

        [JsonPropertyName("responsesCreatedCount")]
        public long ResponsesCreatedCount { get; set; }

        [JsonPropertyName("links")]
        public Links Links { get; set; }

        [JsonPropertyName("isLockedPreviewOnly")]
        public bool IsLockedPreviewOnly { get; set; }

        [JsonPropertyName("metaDescription")]
        public string MetaDescription { get; set; }

        [JsonPropertyName("totalClapCount")]
        public long TotalClapCount { get; set; }

        [JsonPropertyName("sectionCount")]
        public long SectionCount { get; set; }

        [JsonPropertyName("readingList")]
        public long ReadingList { get; set; }

        [JsonPropertyName("topics")]
        public List<Topic> Topics { get; set; }
    }

    public class Links
    {
        [JsonPropertyName("entries")]
        public List<Entry> Entries { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("generatedAt")]
        public DateTime GeneratedAt { get; set; }
    }

    public class Entry
    {
        [JsonPropertyName("url")]
        public Uri Url { get; set; }

        [JsonPropertyName("alts")]
        public List<Alt> Alts { get; set; }

        [JsonPropertyName("httpStatus")]
        public long HttpStatus { get; set; }
    }

    public class Alt
    {
        [JsonPropertyName("type")]
        public long Type { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    public class PreviewImage
    {
        [JsonPropertyName("imageId")]
        public string ImageId { get; set; }

        [JsonPropertyName("filter")]
        public string Filter { get; set; }

        [JsonPropertyName("backgroundSize")]
        public string BackgroundSize { get; set; }

        [JsonPropertyName("originalWidth")]
        public long OriginalWidth { get; set; }

        [JsonPropertyName("originalHeight")]
        public long OriginalHeight { get; set; }

        [JsonPropertyName("strategy")]
        public string Strategy { get; set; }

        [JsonPropertyName("height")]
        public long Height { get; set; }

        [JsonPropertyName("width")]
        public long Width { get; set; }
    }

    public class Tag
    {
        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("postCount")]
        public long PostCount { get; set; }

        [JsonPropertyName("metadata")]
        public Metadata Metadata { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

    public class Metadata
    {
        [JsonPropertyName("postCount")]
        public long PostCount { get; set; }

        [JsonPropertyName("coverImage")]
        public CoverImage CoverImage { get; set; }
    }

    public class CoverImage
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("originalWidth")]
        public long OriginalWidth { get; set; }

        [JsonPropertyName("originalHeight")]
        public long OriginalHeight { get; set; }

        [JsonPropertyName("isFeatured")]
        public bool IsFeatured { get; set; }

        [JsonPropertyName("unsplashPhotoId")]
        public string UnsplashPhotoId { get; set; }

        [JsonPropertyName("focusPercentX")]
        public long? FocusPercentX { get; set; }

        [JsonPropertyName("focusPercentY")]
        public long? FocusPercentY { get; set; }
    }

    public class UserPostRelation
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [JsonPropertyName("postId")]
        public string PostId { get; set; }

        [JsonPropertyName("readAt")]
        public long ReadAt { get; set; }

        [JsonPropertyName("readLaterAddedAt")]
        public long ReadLaterAddedAt { get; set; }

        [JsonPropertyName("votedAt")]
        public long VotedAt { get; set; }

        [JsonPropertyName("collaboratorAddedAt")]
        public long CollaboratorAddedAt { get; set; }

        [JsonPropertyName("notesAddedAt")]
        public long NotesAddedAt { get; set; }

        [JsonPropertyName("subscribedAt")]
        public long SubscribedAt { get; set; }

        [JsonPropertyName("lastReadSectionName")]
        public string LastReadSectionName { get; set; }

        [JsonPropertyName("lastReadVersionId")]
        public string LastReadVersionId { get; set; }

        [JsonPropertyName("lastReadAt")]
        public long LastReadAt { get; set; }

        [JsonPropertyName("lastReadParagraphName")]
        public string LastReadParagraphName { get; set; }

        [JsonPropertyName("lastReadPercentage")]
        public long LastReadPercentage { get; set; }

        [JsonPropertyName("viewedAt")]
        public DateTime ViewedAt { get; set; }

        [JsonPropertyName("presentedCountInResponseManagement")]
        public long PresentedCountInResponseManagement { get; set; }

        [JsonPropertyName("clapCount")]
        public long ClapCount { get; set; }

        [JsonPropertyName("seriesUpdateNotifsOptedInAt")]
        public long SeriesUpdateNotifsOptedInAt { get; set; }

        [JsonPropertyName("queuedAt")]
        public long QueuedAt { get; set; }

        [JsonPropertyName("seriesFirstViewedAt")]
        public long SeriesFirstViewedAt { get; set; }

        [JsonPropertyName("presentedCountInStream")]
        public long PresentedCountInStream { get; set; }

        [JsonPropertyName("seriesLastViewedAt")]
        public long SeriesLastViewedAt { get; set; }

        [JsonPropertyName("audioProgressSec")]
        public long AudioProgressSec { get; set; }
    }
}

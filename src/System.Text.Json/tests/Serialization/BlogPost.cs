using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace System.Text.Json.Serialization.Tests.Schemas.BlogPost
{
    public class Post
    {
        public Value Value { get; set; }
        public List<MentionedUser> MentionedUsers { get; set; }
        public List<Collaborator> Collaborators { get; set; }
        public bool HideMeter { get; set; }
        public List<CollectionUserRelation> CollectionUserRelations { get; set; }
        public object Mode { get; set; }
        public References References { get; set; }
    }

    public class Value
    {
        public Guid Id { get; set; }
        public string VersionId { get; set; }
        public string CreatorId { get; set; }
        public string HomeCollectionId { get; set; }
        public string Title { get; set; }
        public string DetectedLanguage { get; set; }
        public string LatestVersion { get; set; }
        public string LatestPublishedVersion { get; set; }
        public bool HasUnpublishedEdits { get; set; }
        public int LatestRev { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime AcceptedAt { get; set; }
        public DateTime FirstPublishedAt { get; set; }
        public DateTime LatestPublishedAt { get; set; }
        public bool Vote { get; set; }
        public string ExperimentalCss { get; set; }
        public string DisplayAuthor { get; set; }
        public Content Content { get; set; }
        public Virtuals Virtuals { get; set; }
        public bool Coverless { get; set; }
        public string Slug { get; set; }
        public string TranslationSourcePostId { get; set; }
        public string TranslationSourceCreatorId { get; set; }
        public bool IsApprovedTranslation { get; set; }
        public string InResponseToPostId { get; set; }
        public int InResponseToRemovedAt { get; set; }
        public bool IsTitleSynthesized { get; set; }
        public bool AllowResponses { get; set; }
        public string ImportedUrl { get; set; }
        public int ImportedPublishedAt { get; set; }
        public int Visibility { get; set; }
        public string UniqueSlug { get; set; }
        public PreviewContent PreviewContent { get; set; }
        public int License { get; set; }
        public string InResponseToMediaResourceId { get; set; }
        public Uri CanonicalUrl { get; set; }
        public string ApprovedHomeCollectionId { get; set; }
        public string NewsletterId { get; set; }
        public Uri WebCanonicalUrl { get; set; }
        public Uri MediumUrl { get; set; }
        public string MigrationId { get; set; }
        public bool NotifyFollowers { get; set; }
        public bool NotifyTwitter { get; set; }
        public bool NotifyFacebook { get; set; }
        public int ResponseHiddenOnParentPostAt { get; set; }
        public bool IsSeries { get; set; }
        public bool IsSubscriptionLocked { get; set; }
        public int SeriesLastAppendedAt { get; set; }
        public int AudioVersionDurationSec { get; set; }
        public string SequenceId { get; set; }
        public bool IsNsfw { get; set; }
        public bool IsEligibleForRevenue { get; set; }
        public bool IsBlockedFromHightower { get; set; }
        public DateTime DeletedAt { get; set; }
        public int LockedPostSource { get; set; }
        public int HightowerMinimumGuaranteeStartsAt { get; set; }
        public int HightowerMinimumGuaranteeEndsAt { get; set; }
        public int FeatureLockRequestAcceptedAt { get; set; }
        public int MongerRequestType { get; set; }
        public int LayerCake { get; set; }
        public string SocialTitle { get; set; }
        public string SocialDek { get; set; }
        public string EditorialPreviewTitle { get; set; }
        public string EditorialPreviewDek { get; set; }
        public int CurationEligibleAt { get; set; }
        [JsonPropertyName("PrimaryTopic")]
        public Topic PrimaryTopic { get; set; }
        public string PrimaryTopicId { get; set; }
        public bool IsProxyPost { get; set; }
        public string ProxyPostFaviconUrl { get; set; }
        public string ProxyPostProviderName { get; set; }
        public int ProxyPostType { get; set; }
        public string Type { get; set; }
    }

    public class Content
    {
        public string Subtitle { get; set; }
        public BodyModel BodyModel { get; set; }
        public PostDisplay PostDisplay { get; set; }
    }

    public class BodyModel
    {
        public List<Paragraph> Paragraphs { get; set; }
        public List<Section> Sections { get; set; }
    }

    public class Metadata
    {
        public string Id { get; set; }
        public int OriginalWidth { get; set; }
        public int OriginalHeight { get; set; }
        public bool IsFeatured { get; set; }
    }

    public class Markup
    {
        public int Type { get; set; }
        public int Start { get; set; }
        public int End { get; set; }
        public Uri Href { get; set; }
        public string Title { get; set; }
        public string Rel { get; set; }
        public int AnchorType { get; set; }
        public string UserId { get; set; }
    }

    public class Section
    {
        public string Name { get; set; }
        public int StartIndex { get; set; }
    }

    public class PostDisplay
    {
        public bool Coverless { get; set; }
    }

    public class Virtuals
    {
        public bool AllowNotes { get; set; }
        public PreviewImage PreviewImage { get; set; }
        public int WordCount { get; set; }
        public int ImageCount { get; set; }
        public double ReadingTime { get; set; }
        public string Subtitle { get; set; }
        public UserPostRelation UserPostRelation { get; set; }
        public List<object> UsersBySocialRecommends { get; set; }
        public bool NoIndex { get; set; }
        public int Recommends { get; set; }
        public bool IsBookmarked { get; set; }
        public List<Tag> Tags { get; set; }
        public int SocialRecommendsCount { get; set; }
        public int ResponsesCreatedCount { get; set; }
        public Links Links { get; set; }
        public bool IsLockedPreviewOnly { get; set; }
        public string MetaDescription { get; set; }
        public int TotalClapCount { get; set; }
        public int SectionCount { get; set; }
        public int ReadingList { get; set; }
        public List<Topic> Topics { get; set; }
    }

    public class PreviewImage
    {
        public string ImageId { get; set; }
        public string Filter { get; set; }
        public string BackgroundSize { get; set; }
        public int OriginalWidth { get; set; }
        public int OriginalHeight { get; set; }
        public string Strategy { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
    }

    public class UserPostRelation
    {
        public string UserId { get; set; }
        public string PostId { get; set; }
        public int ReadAt { get; set; }
        public int ReadLaterAddedAt { get; set; }
        public int VotedAt { get; set; }
        public int CollaboratorAddedAt { get; set; }
        public int NotesAddedAt { get; set; }
        public int SubscribedAt { get; set; }
        public string LastReadSectionName { get; set; }
        public string LastReadVersionId { get; set; }
        public int LastReadAt { get; set; }
        public string LastReadParagraphName { get; set; }
        public int LastReadPercentage { get; set; }
        public DateTime ViewedAt { get; set; }
        public int PresentedCountInResponseManagement { get; set; }
        public int ClapCount { get; set; }
        public int SeriesUpdateNotifsOptedInAt { get; set; }
        public int QueuedAt { get; set; }
        public int SeriesFirstViewedAt { get; set; }
        public int PresentedCountInStream { get; set; }
        public int SeriesLastViewedAt { get; set; }
        public int AudioProgressSec { get; set; }
    }

    public class Links
    {
        public List<Entry> Entries { get; set; }
        public string Version { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class Entry
    {
        public Uri Url { get; set; }
        public List<Alt> Alts { get; set; }
        public HttpStatusCode HttpStatus { get; set; }
    }

    public class Alt
    {
        public int Type { get; set; }
        public Uri Url { get; set; }
    }

    public class Tag
    {
        public string Slug { get; set; }
        public string Name { get; set; }
        public int PostCount { get; set; }
        public Metadata1 Metadata { get; set; }
        public string Type { get; set; }
    }

    public class Metadata1
    {
        public int PostCount { get; set; }
        public CoverImage CoverImage { get; set; }
    }

    public class CoverImage
    {
        public string Id { get; set; }
        public int OriginalWidth { get; set; }
        public int OriginalHeight { get; set; }
        public bool IsFeatured { get; set; }
        public string UnsplashPhotoId { get; set; }
        public int FocusPercentX { get; set; }
        public int FocusPercentY { get; set; }
    }
    public class Image
    {
        public string Id { get; set; }
        public int OriginalWidth { get; set; }
        public int OriginalHeight { get; set; }
    }

    public class PreviewContent
    {
        public BodyModel BodyModel { get; set; }
        public bool IsFullContent { get; set; }
        public string Subtitle { get; set; }
    }

    public class Paragraph
    {
        public string Name { get; set; }
        public int Type { get; set; }
        public string Text { get; set; }
        public int Layout { get; set; }
        public Metadata Metadata { get; set; }
        public List<Markup> Markups { get; set; }
        public int Alignment { get; set; }
    }

    public class Topic
    {
        public string TopicId { get; set; }
        public string Slug { get; set; }
        public long CreatedAt { get; set; }
        public int DeletedAt { get; set; }
        public Image Image { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Post> RelatedTopics { get; set; }
        public int Visibility { get; set; }
        public List<object> RelatedTags { get; set; }
        public List<object> RelatedTopicIds { get; set; }
        public string Type { get; set; }
    }

    public class References
    {
        public User User { get; set; }
        public Social Social { get; set; }
        public SocialStats SocialStats { get; set; }
    }

    public class User
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public long CreatedAt { get; set; }
        public string ImageId { get; set; }
        public string BackgroundImageId { get; set; }
        public string Bio { get; set; }
        public string TwitterScreenName { get; set; }
        public SocialStats SocialStats { get; set; }
        public Social Social { get; set; }
        public string FacebookAccountId { get; set; }
        public int AllowNotes { get; set; }
        public int MediumMemberAt { get; set; }
        public bool IsNsfw { get; set; }
        public bool IsWriterProgramEnrolled { get; set; }
        public bool IsQuarantined { get; set; }
        public string Type { get; set; }
    }

    public class SocialStats
    {
        public string UserId { get; set; }
        public int UsersFollowedCount { get; set; }
        public int UsersFollowedByCount { get; set; }
        public string Type { get; set; }
    }

    public class Social
    {
        public string UserId { get; set; }
        public string TargetUserId { get; set; }
        public string Type { get; set; }
    }

    public class MentionedUser
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Username { get; set; }
        public long CreatedAt { get; set; }
        public string ImageId { get; set; }
        public string BackgroundImageId { get; set; }
        public string Bio { get; set; }
        public string TwitterScreenName { get; set; }
        public string FacebookAccountId { get; set; }
        public int AllowNotes { get; set; }
        public int MediumMemberAt { get; set; }
        public bool IsNsfw { get; set; }
        public bool IsWriterProgramEnrolled { get; set; }
        public bool IsQuarantined { get; set; }
        public string Type { get; set; }
    }

    public class Collaborator
    {
        public User User { get; set; }
        public string State { get; set; }
    }

    public class CollectionUserRelation
    {
        public string CollectionId { get; set; }
        public string UserId { get; set; }
        public string Role { get; set; }
    }
}

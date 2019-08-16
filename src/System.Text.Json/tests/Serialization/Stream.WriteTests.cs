// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json.Serialization.Tests.Schemas.BlogPost;
using System.Threading.Tasks;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static partial class StreamTests
    {
        [Fact]
        public static async Task VerifyValueFail()
        {
            MemoryStream stream = new MemoryStream();
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await JsonSerializer.SerializeAsync(stream, "", (Type)null));
        }

        [Fact]
        public static async Task VerifyTypeFail()
        {
            MemoryStream stream = new MemoryStream();
            await Assert.ThrowsAsync<ArgumentException>(async () => await JsonSerializer.SerializeAsync(stream, 1, typeof(string)));
        }

        [Fact]
        public static async Task NullObjectValue()
        {
            MemoryStream stream = new MemoryStream();
            await JsonSerializer.SerializeAsync(stream, (object)null);

            stream.Seek(0, SeekOrigin.Begin);

            byte[] readBuffer = new byte[4];
            int bytesRead = stream.Read(readBuffer, 0, 4);

            Assert.Equal(4, bytesRead);
            string value = Encoding.UTF8.GetString(readBuffer);
            Assert.Equal("null", value);
        }

        [Fact]
        public static async Task RoundTripAsync()
        {
            byte[] buffer;

            using (TestStream stream = new TestStream(1))
            {
                await WriteAsync(stream);

                // Make a copy
                buffer = stream.ToArray();
            }

            using (TestStream stream = new TestStream(buffer))
            {
                await ReadAsync(stream);
            }
        }

        [Fact]
        public static async Task RoundTripLargeJsonViaJsonElementAsync()
        {
            // Generating tailored json
            int i = 0;
            StringBuilder json = new StringBuilder();
            json.Append("{");
            while (true)
            {
                if (json.Length >= 14757)
                {
                    break;
                }
                json.AppendFormat(@"""Key_{0}"":""{0}"",", i);
                i++;
            }
            json.Remove(json.Length - 1, 1).Append("}");

            JsonElement root = JsonSerializer.Deserialize<JsonElement>(json.ToString());
            var ms = new MemoryStream();
            await JsonSerializer.SerializeAsync(ms, root, root.GetType());
        }

        [Fact]
        public static async Task RoundTripLargeJsonViaPocoAsync()
        {
            byte[] array = JsonSerializer.Deserialize<byte[]>(JsonSerializer.Serialize(new byte[11056]));
            var ms = new MemoryStream();
            await JsonSerializer.SerializeAsync(ms, array, array.GetType());
        }

        private static async Task WriteAsync(TestStream stream)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                // Will likely default to 4K due to buffer pooling.
                DefaultBufferSize = 1
            };

            {
                LargeDataTestClass obj = new LargeDataTestClass();
                obj.Initialize();
                obj.Verify();

                await JsonSerializer.SerializeAsync(stream, obj, options: options);
            }

            // Must be changed if the test classes change:
            Assert.Equal(551_368, stream.TestWriteBytesCount);

            // We should have more than one write called due to the large byte count.
            Assert.InRange(stream.TestWriteCount, 1, int.MaxValue);

            // We don't auto-flush.
            Assert.Equal(0, stream.TestFlushCount);
        }

        private static async Task ReadAsync(TestStream stream)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                // Will likely default to 4K due to buffer pooling.
                DefaultBufferSize = 1
            };

            LargeDataTestClass obj = await JsonSerializer.DeserializeAsync<LargeDataTestClass>(stream, options);
            // Must be changed if the test classes change; may be > since last read may not have filled buffer.
            Assert.InRange(stream.TestRequestedReadBytesCount, 551368, int.MaxValue);

            // We should have more than one read called due to the large byte count.
            Assert.InRange(stream.TestReadCount, 1, int.MaxValue);

            // We don't auto-flush.
            Assert.Equal(0, stream.TestFlushCount);

            obj.Verify();
        }

        [Fact]
        public static async Task WritePrimitivesAsync()
        {
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(@"1"));
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                DefaultBufferSize = 1
            };

            int i = await JsonSerializer.DeserializeAsync<int>(stream, options);
            Assert.Equal(1, i);
        }

        private class Session
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public virtual string Abstract { get; set; }
            public virtual DateTimeOffset? StartTime { get; set; }
            public virtual DateTimeOffset? EndTime { get; set; }
            public TimeSpan Duration => EndTime?.Subtract(StartTime ?? EndTime ?? DateTimeOffset.MinValue) ?? TimeSpan.Zero;
            public int? TrackId { get; set; }
        }

        private class SessionResponse : Session
        {
            public Track Track { get; set; }
            public List<Speaker> Speakers { get; set; } = new List<Speaker>();
        }

        private class Track
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class Speaker
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Bio { get; set; }
            public virtual string WebSite { get; set; }
        }

        [Theory]
        [InlineData(0)]
        [InlineData(10)]
        [InlineData(1000)]
        [InlineData(4000)]
        [InlineData(8000)]
        [InlineData(16000)]
        [InlineData(32000)]
        [InlineData(64000)]
        public static async Task LargeJsonFile(int bufferSize)
        {
            const int SessionResponseCount = 100;

            // Build up a large list to serialize.
            var list = new List<SessionResponse>();
            for (int i = 0; i < SessionResponseCount; i++)
            {
                SessionResponse response = new SessionResponse
                {
                    Id = i,
                    Abstract = new string('A', i * 2),
                    Title = new string('T', i),
                    StartTime = new DateTime(i, DateTimeKind.Utc),
                    EndTime = new DateTime(i * 10000, DateTimeKind.Utc),
                    TrackId = i,
                    Track = new Track()
                    {
                        Id = i,
                        Name = new string('N', i),
                    },
                };

                for (int j = 0; j < 5; j++)
                {
                    response.Speakers.Add(new Speaker()
                    {
                        Bio = new string('B', 50),
                        Id = j,
                        Name = new string('N', i),
                        WebSite = new string('W', 20),
                    });
                }

                list.Add(response);
            }

            // Adjust buffer length to encourage buffer flusing at several levels.
            JsonSerializerOptions options = new JsonSerializerOptions();
            if (bufferSize != 0)
            {
                options.DefaultBufferSize = bufferSize;
            }

            string json = JsonSerializer.Serialize(list, options);
            Assert.True(json.Length > 100_000); // Verify data is large and will cause buffer flushing.
            Assert.True(json.Length < 200_000); // But not too large for memory considerations.

            // Sync case.
            {
                List<SessionResponse> deserializedList = JsonSerializer.Deserialize<List<SessionResponse>>(json, options);
                Assert.Equal(SessionResponseCount, deserializedList.Count);

                string jsonSerialized = JsonSerializer.Serialize(deserializedList, options);
                Assert.Equal(json, jsonSerialized);
            }

            // Async case.
            using (var memoryStream = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync(memoryStream, list, options);
                string jsonSerialized = Encoding.UTF8.GetString(memoryStream.ToArray());
                Assert.Equal(json, jsonSerialized);

                memoryStream.Position = 0;
                List<SessionResponse> deserializedList = await JsonSerializer.DeserializeAsync<List<SessionResponse>>(memoryStream, options);
                Assert.Equal(SessionResponseCount, deserializedList.Count);
            }
        }

        [Theory]
        [InlineData(1, true, true)]
        [InlineData(1, true, false)]
        [InlineData(1, false, true)]
        [InlineData(1, false, false)]
        [InlineData(10, true, false)]
        [InlineData(10, false, false)]
        [InlineData(100, true, false)]
        [InlineData(1000, true, false)]
        public static async Task VeryLargeJsonFileTest(int payloadSize, bool ignoreNull, bool writeIndented)
        {
            List<Post> list = PopulateLargeObject(payloadSize);

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                IgnoreNullValues = ignoreNull,
                WriteIndented = writeIndented
            };

            string json = JsonSerializer.Serialize(list, options);

            // Sync case.
            {
                List<Post> deserializedList = JsonSerializer.Deserialize<List<Post>>(json, options);
                Assert.Equal(payloadSize, deserializedList.Count);

                string jsonSerialized = JsonSerializer.Serialize(deserializedList, options);
                Assert.Equal(json, jsonSerialized);
            }

            // Async case.
            using (var memoryStream = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync(memoryStream, list, options);
                string jsonSerialized = Encoding.UTF8.GetString(memoryStream.ToArray());
                Assert.Equal(json, jsonSerialized);

                memoryStream.Position = 0;
                List<Post> deserializedList = await JsonSerializer.DeserializeAsync<List<Post>>(memoryStream, options);
                Assert.Equal(payloadSize, deserializedList.Count);
            }
        }

        [Theory]
        [InlineData(1, true, true)]
        [InlineData(1, true, false)]
        [InlineData(1, false, true)]
        [InlineData(1, false, false)]
        [InlineData(2, true, false)]
        [InlineData(2, false, false)]
        [InlineData(4, true, false)]
        [InlineData(8, true, false)]
        [InlineData(16, true, false)]
        public static async Task DeepNestedJsonFileTest(int depthFactor, bool ignoreNull, bool writeIndented)
        {
            int length = 10 * depthFactor;
            List<Post>[] posts = new List<Post>[length];
            posts[0] = PopulateLargeObject(1);
            for (int i = 1; i < length; i++ )
            {
                posts[i] = PopulateLargeObject(1);
                posts[i - 1][0].Value.PrimaryTopic.RelatedTopics = posts[i];
            }

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                MaxDepth = depthFactor * 64,
                IgnoreNullValues = ignoreNull,
                WriteIndented = writeIndented
            };
            string json = JsonSerializer.Serialize(posts[0], options);

            // Sync case.
            {
                List<Post> deserializedList = JsonSerializer.Deserialize<List<Post>>(json, options);

                string jsonSerialized = JsonSerializer.Serialize(deserializedList, options);
                Assert.Equal(json, jsonSerialized);
            }

            // Async case.
            using (var memoryStream = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync(memoryStream, posts[0], options);
                string jsonSerialized = Encoding.UTF8.GetString(memoryStream.ToArray());
                Assert.Equal(json, jsonSerialized);

                memoryStream.Position = 0;
                List<Post> deserializedList = await JsonSerializer.DeserializeAsync<List<Post>>(memoryStream, options);
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(16)]
        public static async Task DeepNestedJsonFileCircularDependencyTest(int depthFactor)
        {
            int length = 10 * depthFactor;
            List<Post>[] posts = new List<Post>[length];
            posts[0] = PopulateLargeObject(1000);
            for (int i = 1; i < length; i++)
            {
                posts[i] = PopulateLargeObject(1);
                posts[i - 1][0].Value.PrimaryTopic.RelatedTopics = posts[i];
            }
            posts[length - 1][0].Value.PrimaryTopic.RelatedTopics = posts[0];

            JsonSerializerOptions options = new JsonSerializerOptions()
            {
                MaxDepth = depthFactor * 64,
                IgnoreNullValues = true
            };

            Assert.Throws<JsonException> (() => JsonSerializer.Serialize(posts[0], options));

            using (var memoryStream = new MemoryStream())
            {
                await Assert.ThrowsAsync<JsonException>(async () => await JsonSerializer.SerializeAsync(memoryStream, posts[0], options));
            }
        }


        [Theory]
        [InlineData(128)]
        [InlineData(1024)]
        [InlineData(4096)]
        [InlineData(8192)]
        [InlineData(16384)]
        [InlineData(65536)]
        public static async void FlushThresholdTest(int bufferSize)
        {
            // bufferSize * 0.9 is the threshold size from codebase, substruct 2 for [" characters, then create a 
            // string containing (threshold - 2) amount of char 'a' which when written into output buffer produces buffer 
            // which size equal to or very close to threshold size, then adding the string to the list, then adding a big 
            // object to the list which changes depth of written json and should cause buffer flush
            int thresholdSize = (int)(bufferSize * 0.9 - 2);
            FlushThresholdTestClass serializeObject = new FlushThresholdTestClass(GenerateListOfSize(bufferSize));
            List<object> list = new List<object>();
            string stringOfThresholdSize = new string('a', thresholdSize);
            list.Add(stringOfThresholdSize);
            serializeObject.StringProperty = stringOfThresholdSize;
            list.Add(serializeObject);
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.DefaultBufferSize = bufferSize;

            string json = JsonSerializer.Serialize(list);

            using (var memoryStream = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync(memoryStream, list, options);
                string jsonSerialized = Encoding.UTF8.GetString(memoryStream.ToArray());
                Assert.Equal(json, jsonSerialized);

                List<object> deserializedList = JsonSerializer.Deserialize<List<object>>(json, options);
                Assert.Equal(stringOfThresholdSize, ((JsonElement)deserializedList[0]).GetString());
                JsonElement.ObjectEnumerator obj = ((JsonElement)deserializedList[1]).EnumerateObject();
                obj.MoveNext();               
                Assert.Equal(stringOfThresholdSize, obj.Current.Value.GetString());
            }
        }

        private class FlushThresholdTestClass {
            public string StringProperty { get; set; }
            public List<int> ListOfInts { get; set; }
            public FlushThresholdTestClass(List<int> list)
            {
                ListOfInts = list;
            }
        }

        private static List<int> GenerateListOfSize(int size)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < size; i++)
            {
                list.Add(1);
            }
            return list;
        }

        private static List<Post> PopulateLargeObject(int size)
        {
            List<Post> posts = new List<Post>(size);
            for (int i = 0; i < size; i++)
            {
                Post post = new Post
                {
                    Value = new Value
                    {
                        Id = Guid.NewGuid(),
                        VersionId = $"version{i}",
                        CreatorId = $"{i}creatorId",
                        HomeCollectionId = $"HomeCollectionId{i}",
                        Title = $"The Adventure of the Engineer’s Thumb {i}",
                        LatestVersion = $"LatestVersion{i}.{i}",
                        LatestPublishedVersion = $"LatestPublishedVersion{i}",
                        DetectedLanguage = "en",
                        HasUnpublishedEdits = i % 2 == 0,
                        Vote = i % 2 == 1,
                        LatestRev = size + i,
                        CreatedAt = DateTime.Today,
                        DeletedAt = DateTime.Now.AddDays(i),
                        FirstPublishedAt = DateTime.Today,
                        UpdatedAt = DateTime.Now,
                        ExperimentalCss = string.Empty,
                        DisplayAuthor = $"Author: {i}",
                        Content = new Content()
                        {
                            Subtitle = $"The Five Orange Pips {i}",
                            BodyModel = new BodyModel()
                            {
                                Paragraphs = new List<Paragraph>(),  
                                Sections = new List<Section>()
                            },
                            PostDisplay = new PostDisplay { Coverless = i % 3 == 0 }
                        },
                        Virtuals = new Virtuals
                        {
                            AllowNotes = i % 2 == 0,
                            PreviewImage = new PreviewImage
                            {
                                ImageId = $"1-Image-{i}.jpeg",  
                                OriginalWidth = 1200 + i,
                                OriginalHeight = 400 + i,
                                Strategy = "resample",
                                Height = i,
                                Width = i + i
                            },
                            WordCount = 1000 + i,
                            ImageCount = i,
                            ReadingTime = 3.14 + i,
                            Subtitle = "Lorem ipsam subtitle of virtuals",
                            UserPostRelation = new UserPostRelation
                            {
                                UserId = $"UserPostRelation_UsrId{i}",
                                PostId = $"UserPostRelation_PostId{i}",
                                ViewedAt = DateTime.Now,
                                PresentedCountInStream = i,
                                LastReadSectionName = $"Last Read Section {i}",
                                ClapCount = 0
                            },
                            UsersBySocialRecommends = new List<object>(),
                            NoIndex = false,
                            IsBookmarked = true,
                            Recommends = 10 + i,
                            Tags = new List<Tag>
                            {
                                new Tag
                                {
                                    Slug = "csharp",
                                    Name = "C#",
                                    PostCount = 98130,
                                    Metadata = new Metadata1
                                    {
                                        PostCount = 98130,
                                        CoverImage = new CoverImage
                                        {
                                            Id = "CoverImageId.png",
                                            OriginalWidth = 1112,
                                            OriginalHeight = 556,
                                            IsFeatured = true,
                                            FocusPercentX = 30,
                                            FocusPercentY = 44
                                        }
                                    },
                                    Type = "Tag"
                                },
                                new Tag
                                {
                                    Slug =  "software-engineering",
                                    Name = "Software Engineering",
                                    PostCount = 62810,
                                    Metadata = new Metadata1
                                    {
                                        PostCount = 62810,
                                        CoverImage = new CoverImage
                                        {
                                            Id = "cover-image-id-b",
                                            OriginalWidth = 812,
                                            OriginalHeight = 461,
                                            IsFeatured = true,
                                            UnsplashPhotoId = "PhotoId1"
                                        }
                                    },
                                    Type = "Tag"
                                }
                            },
                            SocialRecommendsCount = i / 2,
                            ResponsesCreatedCount = 2 + i,
                            Links = new Links {
                                Entries = new List<Entry>
                                {
                                    new Entry
                                    {
                                        Url = new Uri("http://dotnet.test/link/entries/entry/1"),
                                        Alts = new List<Alt>(),
                                        HttpStatus = HttpStatusCode.OK
                                    },
                                    new Entry
                                    {
                                        Url = new Uri("http://dotnet.test/link/entries/entry/2"),
                                        Alts = new List<Alt>
                                        {
                                            new Alt
                                            {
                                                Type = 3,
                                                Url = new Uri("http://dotnet.test/link/entries/alts/1")
                                            },
                                            new Alt
                                            {
                                                Type = 2,
                                                Url = new Uri("http://dotnet.test/link/entries/alts/2")
                                            }
                                        },
                                        HttpStatus = HttpStatusCode.Accepted
                                    }
                                },
                                Version = $"0.{i}",
                                GeneratedAt = new DateTime(2019, 12, 28)
                            },
                            IsLockedPreviewOnly = false,
                            TotalClapCount = 4000 + i,
                            SectionCount = i,
                            Topics = new List<Topic>
                            {
                                new Topic
                                {
                                    TopicId = "TopicID52b64",
                                    Slug = "development",
                                    CreatedAt  = 1493934116328,
                                    DeletedAt = 0,
                                    Image = new Image
                                    {
                                        Id = "1*TopitImageid@2x.jpeg",
                                        OriginalWidth = 6016,
                                        OriginalHeight = 4016
                                    },
                                    Name = "Development",
                                    Description = "Once upon a time.",
                                    RelatedTopics = new List<Post>(),
                                    Visibility = 1,
                                    Type = "Topic"
                                 }
                            }
                        },
                        Coverless = i % 3 == 0,
                        Slug = $"Lorem ipsam mampsa sorem{i}",
                        TranslationSourcePostId = string.Empty,
                        TranslationSourceCreatorId = string.Empty,
                        IsApprovedTranslation = false,
                        InResponseToPostId = string.Empty,
                        InResponseToRemovedAt = i,
                        IsTitleSynthesized = false,
                        AllowResponses = true,
                        ImportedUrl = string.Empty,
                        ImportedPublishedAt = 0,
                        Visibility = 1,
                        UniqueSlug = $"There were doors all round the hall, but they were all locked {i}",
                        PreviewContent = new PreviewContent {
                            BodyModel = new BodyModel
                            {
                                Paragraphs = new List<Paragraph>
                                {
                                    new Paragraph
                                    {
                                        Name = "previewImage",
                                        Type = 4,
                                        Text = string.Empty,
                                        Layout = 10,
                                        Metadata = new Metadata
                                        {
                                            Id = "1-pararagraph-id.jpeg",
                                            OriginalWidth = 1200,
                                            OriginalHeight = 412,
                                            IsFeatured = true
                                        }
                                    },
                                    new Paragraph
                                    {
                                        Name = "Paragraph 2",
                                        Type = 3,
                                        Text = "There were doors all round the hall, but they were all locked",
                                        Markups = new List<Markup>(),
                                        Alignment = 1
                                    },
                                    new Paragraph
                                    {
                                        Name = "Paragraph 3",
                                        Type = 13,
                                        Text = "There were doors all round the hall, but they were all locked",
                                        Markups = new List<Markup>
                                        {
                                            new Markup
                                            {
                                                Type = 3,
                                                Start = 35,
                                                End = 42,
                                                Href = new Uri("http://dotnet.test/markup"),
                                                Title = string.Empty,
                                                Rel = string.Empty,
                                                AnchorType = 0
                                            } 
                                        },
                                        Alignment = 1
                                    }
                                },
                                Sections = new List<Section>
                                {
                                    new Section
                                    {
                                        StartIndex = 0  
                                    }
                                }
                            },
                            IsFullContent = false,
                            Subtitle = "There were doors all round the hall, but they were all locked"
                        },
                        License = i,
                        InResponseToMediaResourceId = string.Empty,
                        CanonicalUrl = new Uri("http://dotnet.test/CanonicalUrl"),
                        ApprovedHomeCollectionId = string.Empty,
                        NewsletterId = string.Empty + i,
                        WebCanonicalUrl = new Uri("http://dotnet.test/WebCanonicalUrl"),
                        MediumUrl = new Uri("http://dotnet.test/MediumUrl"),
                        MigrationId = string.Empty,
                        NotifyFollowers = true,
                        NotifyFacebook = false,
                        NotifyTwitter = false,
                        ResponseHiddenOnParentPostAt = 0,
                        IsSeries = false,
                        IsSubscriptionLocked = false,
                        SeriesLastAppendedAt = 0,
                        AudioVersionDurationSec = 0,
                        SequenceId = string.Empty,
                        IsNsfw = false,
                        IsEligibleForRevenue = false,
                        IsBlockedFromHightower = false,
                        LockedPostSource = 0,
                        HightowerMinimumGuaranteeStartsAt = 0,
                        HightowerMinimumGuaranteeEndsAt = 0,
                        FeatureLockRequestAcceptedAt = i,
                        MongerRequestType = 1,
                        LayerCake = i + 2,
                        SocialTitle = string.Empty,
                        SocialDek = $"Social dek{i}",
                        EditorialPreviewTitle = string.Empty,
                        EditorialPreviewDek = string.Empty,
                        CurationEligibleAt = i,
                        PrimaryTopic = new Topic
                        {
                            TopicId = $"{i}decTopicId",
                            Slug = "programming",
                            CreatedAt = 1493934116328,
                            DeletedAt = 0,
                            Image = new Image
                            {
                                
                            },
                            Name = "Programming",
                            Description = "Lorem ipsam programming.",
                            RelatedTopics = new List<Post>(),
                            Visibility = i,
                            RelatedTags = new List<object>(),
                            RelatedTopicIds = new List<object>(),
                            Type = "Topic"
                        },
                        PrimaryTopicId = $"{i}PrimaryTopicId",
                        IsProxyPost = false,
                        ProxyPostFaviconUrl = string.Empty,
                        ProxyPostProviderName = "ProxyPostProviderName",
                        ProxyPostType = 0,
                        Type = "Post"
                    },
                    MentionedUsers = new List<MentionedUser>
                    {
                        new MentionedUser()
                        {
                            UserId = "cccccaaaabbbbbb",
                            Name = "Lorem Ipsam ",
                            Username = "loremipsam",
                            CreatedAt = 1438064675373,
                            ImageId = string.Empty,
                            BackgroundImageId = string.Empty,
                            Bio = string.Empty,
                            TwitterScreenName = string.Empty,
                            FacebookAccountId = "9988998877662222111",
                            AllowNotes = 1,
                            MediumMemberAt = 0,
                            IsNsfw = false,
                            IsWriterProgramEnrolled = true,
                            IsQuarantined = false,
                            Type = "User"
                        }
                    }, 
                    Collaborators = new List<Collaborator>
                    {
                        new Collaborator
                        {
                            User  = new User
                            {
                                UserId = "222ffbbb888kkk",
                                Name = "Green Field",
                                Username = "greenfield",
                                CreatedAt = 1438064675373,
                                ImageId = string.Empty,
                                BackgroundImageId = string.Empty,
                                Bio = string.Empty,
                                TwitterScreenName = string.Empty,
                                FacebookAccountId = "123456789123465",
                                AllowNotes = 1,
                                MediumMemberAt = 0,
                                IsNsfw = false,
                                IsWriterProgramEnrolled = true,
                                IsQuarantined = false,
                                Type = "User"
                            },
                            State = "visible"
                        }
                    },
                    HideMeter = false,
                    CollectionUserRelations = new List<CollectionUserRelation>
                    {
                        new CollectionUserRelation
                        {
                            CollectionId = "6666ddd777ddd",
                            Role = "ADMIN",
                            UserId = "9999ddd9999"
                        }
                    },
                    References = new References
                    {
                        User = new User
                        {
                            UserId = $"777hhh99888",
                            Username = "johndoe",
                            Name = "John Doe",
                            CreatedAt = 1456628553936,
                            ImageId = $"{i}image-id-A.jpeg",
                            BackgroundImageId = string.Empty,
                            Bio = "Do cats eat bats? Do cats eat bats. Do cats eat bats? Do cats eat bats. Do cats eat bats? Do cats eat bats. Do cats eat bats? Do cats eat bats",
                            TwitterScreenName = "JohnDoe",
                            SocialStats = new SocialStats
                            {
                                UserId = "777hhh99888",
                                UsersFollowedByCount = 40 + i,
                                UsersFollowedCount = i,
                                Type = "SocialStats"
                            },
                            Social = new Social
                            {
                                UserId = "2299kkhhh77gglll",
                                TargetUserId = "22kkoobbb777",
                                Type = "Social"
                            },
                            FacebookAccountId = string.Empty,
                            AllowNotes = 1,
                            MediumMemberAt = 0,
                            IsNsfw = false,
                            IsWriterProgramEnrolled = true,
                            IsQuarantined = false,
                            Type = "User"
                        },
                        Social = new Social
                        {
                            UserId = "888hh99kkk222",
                            TargetUserId = "2299kkhhh77gglll",
                            Type = "Social"
                        },
                        SocialStats = new SocialStats
                        {
                            UserId = "2299kkhhh77gglll",
                            UsersFollowedByCount = 40 + i,
                            UsersFollowedCount = i,
                            Type = "SocialStats"
                        }
                    } 
                };
            
                for (int j = 0; j < i % 4; j++)
                {
                    Paragraph purpleParagraph = new Paragraph
                    {
                        Name = $"a{i}b{j}",
                        Type = i,
                        Text = "Down, down, down. Would the fall never come to an end! ‘I wonder how many miles I’ve fallen by this time. I think—’ (for, you see, Alice had learnt several things of this sort in her lessons in the schoolroom, and though this was not a very good opportunity for showing off her knowledge, as there was no one to listen to her, still it was good practice to say it over) ‘—yes, that’s about the right distance—but then I wonder what Latitude or Longitude I’ve got to",
                        Markups = new List<Markup>(), 
                    };
                    if (j % 2 == 0)
                    {
                        purpleParagraph.Layout = j;
                        for (int a = j % 5; a >= 0; a--)
                        {
                            purpleParagraph.Markups.Add(new Markup
                            {
                                Type = j,
                                AnchorType = a,
                                Href = new Uri($"http://github.test/JohnDoe{i}{j}"),
                                Rel = "nofollow noopener",
                                Start = i,
                                End = i + j
                            });
                        }                      
                    }
                    if (j % 3 == 0)
                    {
                        purpleParagraph.Metadata = new Metadata
                        {
                            IsFeatured = i % 6 == 0,
                            Id = $"{i}*{j}paragraph-id.png",
                            OriginalHeight = 500 + i + j,
                            OriginalWidth = 500 + i + j
                        };
                    }
                    post.Value.Content.BodyModel.Paragraphs.Add(purpleParagraph);
                }

                for (int j = 0; j < i % 3; j++)
                {
                    Section section = new Section
                    {
                        Name = $"ca{i}{j}",
                        StartIndex = i + j
                    };
                    post.Value.Content.BodyModel.Sections.Add(section);
                }
                posts.Add(post);
            }
            return posts;
        }
    }

    public sealed class TestStream : Stream
    {
        private readonly MemoryStream _stream;

        public TestStream(int capacity) { _stream = new MemoryStream(capacity); }

        public TestStream(byte[] buffer) { _stream = new MemoryStream(buffer); }

        public int TestFlushCount { get; private set; }

        public int TestWriteCount { get; private set; }
        public int TestWriteBytesCount { get; private set; }
        public int TestReadCount { get; private set; }
        public int TestRequestedReadBytesCount { get; private set; }

        public byte[] ToArray() => _stream.ToArray();

        public override void Flush()
        {
            TestFlushCount++;
            _stream.Flush();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            TestWriteCount++;
            TestWriteBytesCount += (count - offset);
            _stream.Write(buffer, offset, count);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            TestReadCount++;
            TestRequestedReadBytesCount += count;
            return _stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin) => _stream.Seek(offset, origin);
        public override void SetLength(long value) => _stream.SetLength(value);
        public override bool CanRead => _stream.CanRead;
        public override bool CanSeek => _stream.CanSeek;
        public override bool CanWrite => _stream.CanWrite;
        public override long Length => _stream.Length;
        public override long Position { get => _stream.Position; set => _stream.Position = value; }
    }
}

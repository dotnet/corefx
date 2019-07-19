// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace System.Text.Json.Serialization.Tests
{
    public static partial class StreamTests
    {
        [Fact]
        public async static Task VerifyValueFail()
        {
            MemoryStream stream = new MemoryStream();
            await Assert.ThrowsAsync<ArgumentNullException>(async () => await JsonSerializer.SerializeAsync(stream, "", (Type)null));
        }

        [Fact]
        public async static Task VerifyTypeFail()
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
        [InlineData(10, true, true)]
        [InlineData(10, true, false)]
        [InlineData(10, false, true)]
        [InlineData(10, false, false)]
        [InlineData(100, true, true)]
        [InlineData(100, true, false)]
        [InlineData(100, false, true)]
        [InlineData(100, false, false)]
        [InlineData(1000, true, true)]
        [InlineData(1000, true, false)]
        [InlineData(1000, false, true)]
        [InlineData(1000, false, false)]
        public static async Task VeryLargeJsonFileTest(int payloadSize, bool ignoreNull, bool writeIndented)
        {
            List<Post> list = populateLargeObject(payloadSize);

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
        [InlineData(2, true, true)]
        [InlineData(2, true, false)]
        [InlineData(2, false, true)]
        [InlineData(2, false, false)]
        [InlineData(4, true, true)]
        [InlineData(4, true, false)]
        [InlineData(4, false, true)]
        [InlineData(4, false, false)]
        [InlineData(8, true, true)]
        [InlineData(8, true, false)]
        [InlineData(8, false, true)]
        [InlineData(8, false, false)]
        [InlineData(16, true, true)]
        [InlineData(16, true, false)]
        [InlineData(16, false, true)]
        [InlineData(16, false, false)]
        public static async Task DeepJsonFileTest(int depthFactor, bool ignoreNull, bool writeIndented) {
            int length = 10 * depthFactor;
            List<Post>[] posts = new List<Post>[length];
            posts[0] = populateLargeObject(1);
            for (int i = 1; i < length; i++ )
            {
                posts[i] = populateLargeObject(1);
                posts[i - 1][0].Value.PrimaryTopic.RelatedTopics = new List<object> { posts[i] };
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

        private static List<Post> populateLargeObject(int size)
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
                        Title = $"A Smart Programmer Understands The Problems Worth Fixing {i}",
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
                            Subtitle = $"The difference between solving any problem and the right one {i}",
                            BodyModel = new ContentBodyModel()
                            {
                                Paragraphs = new List<PurpleParagraph>(),  // depth 7
                                Sections = new List<PurpleSection>()
                            },
                            PostDisplay = new PostDisplay { Coverless = i % 3 == 0 }
                        },
                        Virtuals = new Virtuals
                        {
                            AllowNotes = i % 2 == 0,
                            PreviewImage = new PreviewImage
                            {
                                ImageId = $"1*SNhelFbiMUTGX73GF_c_{i}.jpeg",  //depth 6
                                OriginalWidth = 1200 + i,
                                OriginalHeight = 400 + i,
                                Strategy = "resample",
                                Height = i,
                                Width = i + i
                            },
                            WordCount = 1000 + i,
                            ImageCount = i,
                            ReadingTime = 5.6632075471698 + i,
                            Subtitle = "The difference between solving any problem and the right one",
                            UserPostRelation = new UserPostRelation
                            {
                                UserId = $"78aeffcb483e{i}",
                                PostId = $"dcf15871f943{i}",
                                ViewedAt = DateTime.Now,
                                PresentedCountInStream = i,
                                LastReadSectionName = $"Section {i}",
                                ClapCount = 0
                            },
                            UsersBySocialRecommends = new List<object>(),
                            NoIndex = false,
                            IsBookmarked = true,
                            Recommends = 100 + i,
                            Tags = new List<Tag>(),  // TODO
                            SocialRecommendsCount = i / 2,
                            ResponsesCreatedCount = 2 + i,
                            Links = new Links {
                                Entries = new List<Entry>(), // TODO
                                Version = $"0.{i}",
                                GeneratedAt = new DateTime(2019, 12, 28)
                            },
                            IsLockedPreviewOnly = false,
                            TotalClapCount = 4000 + i,
                            SectionCount = i,
                            Topics = new List<Topic>() //TODO
                        },
                        Coverless = i % 3 == 0,
                        Slug = $"a-smart-programmer-understands-the-problems-worth-fixing-{i}",
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
                        UniqueSlug = $"a-smart-programmer-understands-the-problems-worth-fixing-dcf15871f943{i}",
                        PreviewContent = new PreviewContent {
                            BodyModel = new PreviewContentBodyModel
                            {
                                Paragraphs = new List<FluffyParagraph>
                                {
                                    new FluffyParagraph
                                    {
                                        Name = "previewImage",
                                        Type = 4,
                                        Text = string.Empty,
                                        Layout = 10,
                                        Metadata = new Image
                                        {
                                            Id = "1*SNhelFbiMUTGX73GF_c_ew.jpeg", //depth 8
                                            OriginalWidth = 1200,
                                            OriginalHeight = 412,
                                            IsFeatured = true
                                        }
                                    },
                                    new FluffyParagraph
                                    {
                                        Name = "97c6",
                                        Type = 3,
                                        Text = "A Smart Programmer Understands The Problems Worth Fixing",
                                        Markups = new List<Markup>(),
                                        Alignment = 1
                                    },
                                    new FluffyParagraph
                                    {
                                        Name = "d1b2",
                                        Type = 13,
                                        Text = "The difference between solving any problem\u2026",
                                        Markups = new List<Markup>
                                        {
                                            new Markup
                                            {
                                                Type = 3,
                                                Start = 35,
                                                End = 42,
                                                Href = new Uri("https://levelup.gitconnected.com/the-problem-you-solve-is-more-important-than-the-code-you-write-d0e5493132c6"),
                                                Title = string.Empty,
                                                Rel = string.Empty,
                                                AnchorType = 0
                                            } 
                                        },
                                        Alignment = 1
                                    }
                                },
                                Sections = new List<FluffySection>
                                {
                                    new FluffySection
                                    {
                                        StartIndex = 0  
                                    }
                                }
                            },
                            IsFullContent = false,
                            Subtitle = "The difference between solving any problem and the right one"
                        },
                        License = i,
                        InResponseToMediaResourceId = string.Empty,
                        CanonicalUrl = new Uri("https://medium.com/@fagnerbracka-smart-programmer-understands-the-problems-worth-fixing-dcf15871f943"),
                        ApprovedHomeCollectionId = string.Empty,
                        NewsletterId = string.Empty + i,
                        WebCanonicalUrl = new Uri("https://medium.com/@fagnerbrack/a-smart-programmer-understands-the-problems-worth-fixing-dcf15871f943"),
                        MediumUrl = new Uri("https://medium.com/@fagnerbrack/a-smart-programmer-understands-the-problems-worth-fixing-dcf15871f943"),
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
                            TopicId = $"{i}decb52b64abf",
                            Slug = "programming",
                            CreatedAt = 1493934116328, //TODO it should be DateTime
                            DeletedAt = 0,             // This one too
                            Image = new Image
                            {

                            },
                            Name = "Programming",
                            Description = "The good, the bad, the buggy.",
                            RelatedTopics = new List<object>(),
                            Visibility = i,
                            RelatedTags = new List<object>(),
                            RelatedTopicIds = new List<object>(),
                            Type = "Topic"
                        },
                        PrimaryTopicId = $"{i}decb52b64abf",
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
                            UserId = "c4ccb3ab8d17",
                            Name = "Ian Tinsley",
                            Username = "itinsley",
                            CreatedAt = 1438064675373,
                            ImageId = string.Empty,
                            BackgroundImageId = string.Empty,
                            Bio = string.Empty,
                            TwitterScreenName = string.Empty,
                            FacebookAccountId = "10155043593360477",
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
                            User  = new MentionedUser
                            {
                                UserId = "c4ccb3ab8d17",
                                Name = "Ian Tinsley",
                                Username = "itinsley",
                                CreatedAt = 1438064675373,
                                ImageId = string.Empty,
                                BackgroundImageId = string.Empty,
                                Bio = string.Empty,
                                TwitterScreenName = string.Empty,
                                FacebookAccountId = "10155043593360477",
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
                            CollectionId = "6b0b79fe3153",
                            Role = "ADMIN",
                            UserId = "78aeffcb483e"
                        }
                    },
                    References = new References
                    {
                        User = new User
                        {
                            The7Ef192B7F545 = new MentionedUser
                            {
                                UserId = $"7ef192b7f545",
                                Username = "fagnerbrack",
                                Name = "Fagner Brack",
                                CreatedAt = 1456628553936, // TODO should be datetime
                                ImageId = $"{i}*h8Ph7pgNeQHZTiLlah3h-A.jpeg",
                                BackgroundImageId = string.Empty,
                                Bio = "I believe ideas should be open and free. This is a non-profit initiative to write about fundamentals you won't find anywhere else. ~7 min post every few weeks.",
                                TwitterScreenName = "FagnerBrack",
                                SocialStats = new SocialStats
                                {
                                    UserId = "7ef192b7f545",   // depth 8
                                    UsersFollowedByCount = 4000 + i,
                                    UsersFollowedCount = i,
                                    Type = "SocialStats"
                                },
                                Social = new Social
                                {
                                    UserId = "78aeffcb483e",
                                    TargetUserId = "7ef192b7f545",
                                    Type = "Social"
                                },
                                FacebookAccountId = string.Empty,
                                AllowNotes = 1,
                                MediumMemberAt = 0,
                                IsNsfw = false,
                                IsWriterProgramEnrolled = true,
                                IsQuarantined = false,
                                Type = "User"
                            }
                        },
                        Social = new SocialClass
                        {
                            The7Ef192B7F545 = new Social
                            {
                                UserId = "78aeffcb483e",
                                TargetUserId = "7ef192b7f545",
                                Type = "Social"
                            }
                        },
                        SocialStats = new SocialStatsClass
                        {
                            The7Ef192B7F545 = new SocialStats
                            {
                                UserId = "7ef192b7f545",
                                UsersFollowedByCount = 4000+i,
                                UsersFollowedCount = i,
                                Type = "SocialStats"
                            }
                        }
                    } 
                };
            
                for (int j = 0; j < i % 10; j++)
                {
                    PurpleParagraph purpleParagraph = new PurpleParagraph
                    {
                        Name = $"a{i}b{j}",
                        Type = i,
                        Text = "The picture of two firefighters clearing up the snow surrounding a hydrant. In case there's an emergency, the cost in lives due to having snow blocking the hydrant is more significant than the cost of clearing them all beforehand. Clearing a hydrant after a snowfall is a problem worth solving.",
                        Markups = new List<Markup>(), 
                    };
                    if (j % 2 == 0)
                    {
                        purpleParagraph.Layout = j;
                        for (int a = j % 5; a >= 0; a--)
                        {
                            purpleParagraph.Markups.Add(new Markup // depth 9
                            {
                                Type = j,
                                AnchorType = a,
                                Href = new Uri($"http://github.com/JohnDoe{i}{j}"),
                                Rel = "nofollow noopener",
                                Start = i,
                                End = i + j
                            });
                        }                      
                    }
                    if (j % 3 == 0)
                    {
                        purpleParagraph.Metadata = new Image
                        {
                            IsFeatured = i % 6 == 0,
                            Id = $"{i}*{j}5XZrAmo8ToLaZsP5vxSY9A.png",
                            OriginalHeight = 500 + i + j,
                            OriginalWidth = 500 + i + j
                        };
                    }
                    post.Value.Content.BodyModel.Paragraphs.Add(purpleParagraph);
                }

                for (int j = 0; j < i % 5; j++)
                {
                    PurpleSection section = new PurpleSection
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

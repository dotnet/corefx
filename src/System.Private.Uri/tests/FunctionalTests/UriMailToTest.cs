﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.PrivateUri.Tests
{
    /// <summary>
    /// Summary description for UriMailToParsing
    /// </summary>
    public class UriMailToTest
    {
        public UriMailToTest()
        {
        }

        [Fact]
        public void UriMailTo_SchemeOnly_Success()
        {
            Uri uri = new Uri("mailto:");
            Assert.Equal("mailto", uri.Scheme);
            Assert.Equal("mailto:", uri.AbsoluteUri);
            Assert.Equal("mailto:", uri.ToString());
            Assert.Equal("", uri.Host);
        }

        [Fact]
        public void UriMailTo_SchemeAndBackslash_Throws()
        {
            Assert.ThrowsAny<FormatException>(() =>
          {
              Uri uri = new Uri(@"mailto:\");
              ValidateNotCorrupt(uri);
          });
        }

        [Fact]
        public void UriMailTo_SchemeAndForwardSlash_Success()
        {
            Uri uri = new Uri("mailto:/");
            Assert.Equal("mailto", uri.Scheme);
            Assert.Equal("mailto:/", uri.AbsoluteUri);
            Assert.Equal("mailto:/", uri.ToString());
            Assert.Equal("", uri.Host);
            Assert.Equal("/", uri.AbsolutePath);
        }

        [Fact]
        public void UriMailTo_SchemeAndDoubleForwardSlash_Success()
        {
            Uri uri = new Uri("mailto://");
            Assert.Equal("mailto", uri.Scheme);
            Assert.Equal("mailto://", uri.AbsoluteUri);
            Assert.Equal("mailto://", uri.ToString());
            Assert.Equal("", uri.Host);
            Assert.Equal("//", uri.AbsolutePath);
        }

        [Fact]
        public void UriMailTo_SchemeAndQuery_Success()
        {
            Uri uri = new Uri("mailto:?to=User2@Host2.com;cc=User3@Host3com");
            Assert.Equal("mailto", uri.Scheme);
            Assert.Equal("mailto:?to=User2@Host2.com;cc=User3@Host3com", uri.AbsoluteUri);
            Assert.Equal("mailto:?to=User2@Host2.com;cc=User3@Host3com", uri.ToString());
            Assert.Equal("", uri.Host);
            Assert.Equal("", uri.UserInfo);
            Assert.Equal("", uri.AbsolutePath);
            Assert.Equal("?to=User2@Host2.com;cc=User3@Host3com", uri.Query);
        }

        [Fact]
        public void UriMailTo_SchemeUserAt_Throws()
        {
            Assert.ThrowsAny<FormatException>(() =>
          {
              Uri uri = new Uri("mailto:User@");
              ValidateNotCorrupt(uri);
          });
        }

        [Fact]
        public void UriMailTo_SchemeUserColonPasswordAt_Throws()
        {
            Assert.ThrowsAny<FormatException>(() =>
          {
              Uri uri = new Uri("mailto:User:Password@");
          });
        }

        [Fact]
        public void UriMailTo_SchemeUserAtQuery_Throws()
        {
            Assert.ThrowsAny<FormatException>(() =>
          {
              Uri uri = new Uri("mailto:User@?to=User2@Host2.com;cc=User3@Host3com");
              ValidateNotCorrupt(uri);
          });
        }

        [Fact]
        public void UriMailTo_SchemeUserAtHost_Success()
        {
            Uri uri = new Uri("mailto:User@Host");
            Assert.Equal("mailto", uri.Scheme);
            Assert.Equal("mailto:User@host", uri.AbsoluteUri);
            Assert.Equal("mailto:User@host", uri.ToString());
            Assert.Equal("host", uri.Host);
            Assert.Equal("User", uri.UserInfo);
            Assert.Equal("", uri.AbsolutePath);
        }

        [Fact]
        public void UriMailTo_SchemeUserColonPasswordAtHost_Success()
        {
            Uri uri = new Uri("mailto:User:Password@Host");
            Assert.Equal("mailto", uri.Scheme);
            Assert.Equal("mailto:User:Password@host", uri.AbsoluteUri);
            Assert.Equal("mailto:User:Password@host", uri.ToString());
            Assert.Equal("host", uri.Host);
            Assert.Equal("User:Password", uri.UserInfo);
            Assert.Equal("", uri.AbsolutePath);
        }

        [Fact]
        public void UriMailTo_SchemeUserAtHostPort_Success()
        {
            Uri uri = new Uri("mailto:User@Host:3555");
            Assert.Equal("mailto", uri.Scheme);
            Assert.Equal("mailto:User@host:3555", uri.AbsoluteUri);
            Assert.Equal("mailto:User@host:3555", uri.ToString());
            Assert.Equal("host", uri.Host);
            Assert.Equal(3555, uri.Port);
            Assert.Equal("User", uri.UserInfo);
            Assert.Equal("", uri.AbsolutePath);
        }

        [Fact]
        public void UriMailTo_TwoSemiColonSepratedAddresses_Success()
        {
            Assert.ThrowsAny<FormatException>(() =>
          {
              Uri uri = new Uri("mailto:User@Host;User@Host");
              ValidateNotCorrupt(uri);
          });
        }

        [Fact]
        public void UriMailTo_TwoCommaSepratedAddresses_Success()
        {
            Assert.ThrowsAny<FormatException>(() =>
          {
              Uri uri = new Uri("mailto:User@Host,User@Host");
              ValidateNotCorrupt(uri);
          });
        }

        [Fact]
        public void UriMailTo_SchemeUserAtHostPath_Success()
        {
            Uri uri = new Uri("mailto:User@Host/Path1/./Path2/../...");
            Assert.Equal("mailto", uri.Scheme);
            Assert.Equal("mailto:User@host/Path1/./Path2/../...", uri.AbsoluteUri);
            Assert.Equal("mailto:User@host/Path1/./Path2/../...", uri.ToString());
            Assert.Equal("host", uri.Host);
            Assert.Equal("User", uri.UserInfo);
            Assert.Equal("/Path1/./Path2/../...", uri.AbsolutePath);
        }

        [Fact]
        public void UriMailTo_SchemeUserAtHostQuery_Success()
        {
            Uri uri = new Uri("mailto:User@Host?to=User2@Host2.com;cc=User3@Host3com");
            Assert.Equal("mailto", uri.Scheme);
            Assert.Equal("mailto:User@host?to=User2@Host2.com;cc=User3@Host3com", uri.AbsoluteUri);
            Assert.Equal("mailto:User@host?to=User2@Host2.com;cc=User3@Host3com", uri.ToString());
            Assert.Equal("host", uri.Host);
            Assert.Equal("User", uri.UserInfo);
            Assert.Equal("", uri.AbsolutePath);
            Assert.Equal("?to=User2@Host2.com;cc=User3@Host3com", uri.Query);
        }

        [Fact]
        public void UriMailTo_SchemeUserAtHostPathQuery_Success()
        {
            Uri uri = new Uri("mailto:User@Host/Path1/./Path2/../...?to=User2@Host2.com;cc=User3@Host3com");
            Assert.Equal("mailto", uri.Scheme);
            Assert.Equal("mailto:User@host/Path1/./Path2/../...?to=User2@Host2.com;cc=User3@Host3com", uri.AbsoluteUri);
            Assert.Equal("mailto:User@host/Path1/./Path2/../...?to=User2@Host2.com;cc=User3@Host3com", uri.ToString());
            Assert.Equal("host", uri.Host);
            Assert.Equal("User", uri.UserInfo);
            Assert.Equal("/Path1/./Path2/../...", uri.AbsolutePath);
            Assert.Equal("?to=User2@Host2.com;cc=User3@Host3com", uri.Query);
        }

        [Fact]
        public void UriMailTo_EAI_SomeEscaping()
        {
            Uri uri = new Uri("mailto:\u30AF@\u30AF");
            Assert.Equal("mailto", uri.Scheme);
            Assert.Equal("mailto:%E3%82%AF@\u30AF", uri.AbsoluteUri);
            Assert.Equal("mailto:\u30AF@\u30AF", uri.ToString());
            Assert.Equal("%E3%82%AF", uri.UserInfo);
            Assert.Equal("\u30AF", uri.Host);
            Assert.Equal("", uri.AbsolutePath);
            Assert.Equal("\u30AF@\u30AF", uri.GetComponents(UriComponents.UserInfo | UriComponents.Host, UriFormat.SafeUnescaped));
        }


        #region Helper methods

        // private void ValidateResult(Uri result, string absoluteUri, string scheme, string host, 

        // Some MailTo Uri's succesfully parse in the contructor or TryParse, but then they throw when accessing properties.
        private void ValidateNotCorrupt(Uri uri)
        {
            try
            {
                string result = uri.AbsoluteUri;
            }
            catch (Exception ex)
            {
                Assert.False(true, "Exception thrown too late: " + ex);
            }
        }

        #endregion Helper methods
    }
}

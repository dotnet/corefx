using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace System.Net.Mail.Functional.Tests
{
    public class CredentialTest
    {
        private ICredentialsByHost GetTransportCredentials(SmtpClient client)
        {
            Type smtpTransportType = (typeof(SmtpClient)).Assembly.GetType("System.Net.Mail.SmtpTransport");

            var transport = typeof(SmtpClient)
                .GetField("_transport", BindingFlags.NonPublic | BindingFlags.GetField | BindingFlags.Instance)
                .GetValue(client);

            var transportCredentials = smtpTransportType
                .GetProperty("Credentials", BindingFlags.NonPublic | BindingFlags.GetProperty | BindingFlags.Instance)
                .GetValue(transport);

            return (ICredentialsByHost) transportCredentials;
        }

        [Fact]
        public void Credentials_Unset_Null()
        {
            SmtpClient client = new SmtpClient();

            ICredentialsByHost transportCredentials = GetTransportCredentials(client);

            Assert.Null(client.Credentials);

            // Verify that transport credentials are correct
            Assert.Null(transportCredentials);
        }

        [Fact]
        public void DefaultCredentials_True_DefaultCredentials()
        {
            NetworkCredential expectedCredentials = CredentialCache.DefaultNetworkCredentials;

            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = true;

            ICredentialsByHost transportCredentials = GetTransportCredentials(client);

            Assert.Equal(expectedCredentials, client.Credentials);

            // Verify that transport credentials are correct
            Assert.Equal(expectedCredentials, transportCredentials);
        }

        [Fact]
        public void UseDefaultCredentials_False_Null()
        {
            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;

            ICredentialsByHost transportCredentials = GetTransportCredentials(client);

            Assert.Null(client.Credentials);

            // Verify that transport credentials are correct
            Assert.Null(transportCredentials);
        }

        [Fact]
        public void Credentials_UseDefaultCredentialsSetFalseBeforeCredentials_Credentials()
        {
            string userName = "user";
            string password = "password";
            NetworkCredential expectedCredentials = new NetworkCredential(userName, password);

            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = false;
            client.Credentials = expectedCredentials;

            ICredentialsByHost transportCredentials = GetTransportCredentials(client);

            Assert.NotNull(client.Credentials);
            Assert.Equal(expectedCredentials, client.Credentials);

            // Verify that transport credentials are correct
            Assert.Equal(expectedCredentials, transportCredentials);
        }

        [Fact]
        public void Credentials_UseDefaultCredentialsSetFalseAfterCredentials_Credentials()
        {
            string userName = "user";
            string password = "password";

            NetworkCredential expectedCredentials = new NetworkCredential(userName, password);

            SmtpClient client = new SmtpClient();
            client.Credentials = expectedCredentials;
            client.UseDefaultCredentials = false;

            ICredentialsByHost transportCredentials = GetTransportCredentials(client);

            Assert.NotNull(client.Credentials);
            Assert.Equal(expectedCredentials, client.Credentials);

            // Verify that transport credentials are correct
            Assert.Equal(expectedCredentials, transportCredentials);
        }

        [Fact]
        public void Credentials_UseDefaultCredentialsSetTrueBeforeCredentials_DefaultNetworkCredentials()
        {
            string userName = "user";
            string password = "password";

            NetworkCredential expectedCredentials = CredentialCache.DefaultNetworkCredentials;

            SmtpClient client = new SmtpClient();
            client.UseDefaultCredentials = true;
            client.Credentials = new NetworkCredential(userName, password);

            ICredentialsByHost transportCredentials = GetTransportCredentials(client);

            Assert.Equal(expectedCredentials, client.Credentials);

            // Verify that transport credentials are correct
            Assert.Equal(expectedCredentials, transportCredentials);
        }

        [Fact]
        public void Credentials_UseDefaultCredentialsSetTrueAfterCredentials_DefaultNetworkCredentials()
        {
            string userName = "user";
            string password = "password";

            NetworkCredential expectedCredentials = CredentialCache.DefaultNetworkCredentials;

            SmtpClient client = new SmtpClient();
            client.Credentials = new NetworkCredential(userName, password);
            client.UseDefaultCredentials = true;

            ICredentialsByHost transportCredentials = GetTransportCredentials(client);

            Assert.Equal(expectedCredentials, client.Credentials);

            // Verify that transport credentials are correct
            Assert.Equal(expectedCredentials, transportCredentials);
        }
    }
}

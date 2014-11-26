//---------------------------------------------------------------------------------
// Copyright (c), Microsoft Corporation
//---------------------------------------------------------------------------------

namespace OnYourWayHome.AccessControl
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using OnYourWayHome.ServiceBus;
    using OnYourWayHome.ServiceBus.Serialization;

    [DataContract]
    internal class TokenResponse
    {
        private static readonly IDataContractSerializer jsonTokenSerializer = ServiceBusAdapter.Current.CreateJsonSerializer(typeof(TokenResponse));

        [DataMember(Name = "access_token")]
        public string AccessToken { get; set; }

        public static TokenResponse Create(TokenRequestFormat format, Stream responseStream)
        {
            switch (format)
            {
                case TokenRequestFormat.Wrap:
                    return CreateFromWrapResponse(responseStream);

                case TokenRequestFormat.OAuth2:
                    return CreateFromOAuthResponse(responseStream);

                default:
                    throw new NotSupportedException("The token format '" + format.ToString() + "' is unknown.");
            }
        }

        private static TokenResponse CreateFromWrapResponse(Stream responseStream)
        {
            using (var reader = new StreamReader(ToMemoryStream(responseStream)))
            {
                var response = reader.ReadToEnd();
                var responseProperties = response.DecodeHttpForm();
                var tokenString = responseProperties["wrap_access_token"];

                return new TokenResponse
                {
                    AccessToken = tokenString
                };
            }
        }

        private static Stream ToMemoryStream(Stream input)
        {
            byte[] buffer = new byte[4096];

            MemoryStream stream = new MemoryStream();

            int bytesRead;
            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) != 0)
            {
                stream.Write(buffer, 0, bytesRead);
            }

            stream.Seek(0, SeekOrigin.Begin);

            return stream;
        }

        private static TokenResponse CreateFromOAuthResponse(Stream responseStream)
        {
            return (TokenResponse)jsonTokenSerializer.ReadObject(responseStream);
        }
    }
}

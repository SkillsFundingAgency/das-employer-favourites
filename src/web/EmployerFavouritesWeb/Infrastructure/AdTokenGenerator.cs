﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace DfE.EmployerFavourites.Infrastructure
{
    public class AdTokenGenerator
    {
        private readonly Dictionary<string, Token> _tokenLookup;
        private readonly IHostingEnvironment _environment;
        private const string KeyPattern = "{0}-{1}-{2}-{3}";

        public AdTokenGenerator(IHostingEnvironment environment)
        {
            _environment = environment;
        }

        private class Token
        {
            public Token(string value, DateTime expiry)
            {
                TokenValue = value;
                Expires = expiry;
            }

            public string TokenValue { get; set; }
            public DateTime Expires { get; set; }
        }

        public AdTokenGenerator()
        {
            _tokenLookup = new Dictionary<string, Token>();
        }

        public async Task<string> Generate(string tenant, string clientId, string secret, string identifierUri)
        {
            if (_environment.IsDevelopment())
            {
                return string.Empty;
            }

            var key = string.Format(KeyPattern, tenant, clientId, secret, identifierUri);
            var token = _tokenLookup.GetValueOrDefault(key);

            if (token != null && token.Expires < DateTime.UtcNow)
                return token.TokenValue;

            var authority = $"https://login.microsoftonline.com/{tenant}";
            var clientCredential = new ClientCredential(clientId, secret);
            var context = new AuthenticationContext(authority, true);
            var result = await context.AcquireTokenAsync(identifierUri, clientCredential);

            _tokenLookup.Add(key, new Token(result.AccessToken, result.ExpiresOn.DateTime));

            return result.AccessToken;
        }
    }

}
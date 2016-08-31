﻿/*
 * Copyright 2014, 2015 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using IdentityServer3.Core.Logging;
using IdentityServer3.Core.Models;
using Newtonsoft.Json;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using IdentityServer3.Core.Extensions;
using IdentityServer3.Core.Services;

namespace IdentityServer3.Core.Results
{
    internal class TokenResult : IHttpActionResult
    {
        private static readonly ILog Logger = LogProvider.GetCurrentClassLogger();

        private readonly TokenResponse _response;

        public TokenResult(TokenResponse response)
        {
            _response = response;
        }

        public Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute());
        }

        private HttpResponseMessage Execute()
        {
            var dto = new TokenResponseDto
            {
                id_token = _response.IdentityToken,
                access_token = _response.AccessToken,
                refresh_token = _response.RefreshToken,
                expires_in = _response.AccessTokenLifetime,
                token_type = _response.TokenType,
                alg = _response.Algorithm
            };

            var jObject = ObjectSerializer.ToJObject(dto);

            // custom entries
            if (_response.Custom != null && _response.Custom.Any())
            {
                jObject.AddDictionary(_response.Custom);
            }

            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                //Content = new ObjectContent<JObject>(jobject, new JsonMediaTypeFormatter())
                Content = new StringContent(jObject.ToString(Formatting.None), Encoding.UTF8, "application/json")
            };

            Logger.Info("Returning token response.");
            return response;
        }

        internal class TokenResponseDto
        {
            public string id_token { get; set; }
            public string access_token { get; set; }
            public int expires_in { get; set; }
            public string token_type { get; set; }
            public string refresh_token { get; set; }
            public string alg { get; set; }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace Cms.Tests.Base
{
    public abstract class ApiTestsBase : IDisposable
    {
        private const string Username = "RadOnlyService";
        private const string Password = "Episerver123%";

        protected readonly HttpClient Client;
        private const string IntegrationUrl = "https://readonly-serviceapi.localtest.me";

        protected ApiTestsBase()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            Client = new HttpClient
            {
                BaseAddress = new Uri(IntegrationUrl)
            };
            Authenticate(Client);
        }

        public void Dispose()
        {
            Client.Dispose();
        }

        private void Authenticate(HttpClient client)
        {
            var fields = new Dictionary<string, string>
                {
                    { "grant_type", "password" },
                    { "username", Username },
                    { "password", Password }
                };
            var response = client.PostAsync("/episerverapi/token", new FormUrlEncodedContent(fields)).Result;
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Authentication failed! Status: {response.StatusCode}");
            }

            var content = response.Content.ReadAsStringAsync().Result;
            var token = JObject.Parse(content).GetValue("access_token").ToString();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
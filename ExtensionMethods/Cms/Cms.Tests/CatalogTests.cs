using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using Cms.Helpers;
using Cms.Tests.Base;
using Newtonsoft.Json;
using Xunit;

namespace Cms.Tests
{
    public class CatalogTests : ApiTestsBase
    {
        [Fact]
        public void it_replaces_IContentLoader()
        {
            ExtensionMethods.Loader.Accessor = new EPiServer.ServiceLocation.ServiceAccessor`1();
        }

        public class StubContentLoader : IContentLoader
        {
            // Implementation
        }


        [Fact]
        public void it_can_retrieve_catalogs()
        {
            var response =
                Client.GetAsync("/episerverapi/commerce/catalogs")
                    .Result;

            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(response.StatusCode, HttpStatusCode.OK);
        }

        [Fact]
        public void it_fails_to_post_catalog()
        {
            var model = new Catalog
            {
                DefaultCurrency = "usd",
                DefaultLanguage = "en",
                EndDate = DateTime.UtcNow.AddYears(1),
                IsActive = true,
                IsPrimary = true,
                Languages = new List<CatalogLanguage>
                {
                    new CatalogLanguage
                    {
                        Catalog = "Test Post",
                        LanguageCode = "en",
                        UriSegment = "Test Post"
                    }
                },
                Name = "Test Post",
                StartDate = DateTime.UtcNow,
                WeightBase = "lbs"
            };
            var json = JsonConvert.SerializeObject(model);
            var response = Client.PostAsync(
                "/episerverapi/commerce/catalogs",
                new StringContent(json, Encoding.UTF8, "application/json")).Result;

            Assert.False(response.IsSuccessStatusCode);
            Assert.Equal(response.StatusCode, HttpStatusCode.Unauthorized);
        }
    }

    [Serializable]
    public class NameValue
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    [Serializable]
    public class ResourceLink
    {
        public string Title { get; set; }
        public string Type { get; set; }
        public string Href { get; set; }
        public List<NameValue> Properties { get; set; }
    }

    [Serializable]
    public class CatalogLanguage
    {
        public string LanguageCode { get; set; }
        public string Catalog { get; set; }
        public string UriSegment { get; set; }
    }

    [Serializable]
    public class Catalog
    {
        public Guid ApplicationId { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsPrimary { get; set; }
        public string Owner { get; set; }
        public int SortOrder { get; set; }
        public string Name { get; set; }
        public string DefaultCurrency { get; set; }
        public string DefaultLanguage { get; set; }
        public string WeightBase { get; set; }
        public List<CatalogLanguage> Languages { get; set; }
        public List<ResourceLink> Nodes { get; set; }
        public List<ResourceLink> Entries { get; set; }
    }
}
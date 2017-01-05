using System;
using System.Globalization;
using System.Linq;
using EPiServer;
using EPiServer.Commerce.Catalog.ContentTypes;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using Mediachase.Commerce.Catalog;

namespace Cms.Helpers
{
    public static class ExtensionMethods
    {
        //public static string FirstChildName(
        //    this ContentReference link, IContentLoader loader)
        //{
        //    var item = loader.GetChildren<IContent>(link).FirstOrDefault();
        //    return item == null ? string.Empty : item.Name;
        //}

        public static string FirstChildNameSl(
            this ContentReference link)
        {
            var loader = ServiceLocator.Current.GetInstance<IContentLoader>();
            var item = loader.GetChildren<IContent>(link).FirstOrDefault();
            return item == null ? string.Empty : item.Name;
        }

        //public static Injected<IContentLoader> Loader { get; set; }

        public static string FirstChildNameP(
            this ContentReference link)
        {
            var item = Loader.Service.GetChildren<IContent>(link).FirstOrDefault();
            return item == null ? string.Empty : item.Name;
        }

        

        public static string FirstChildName(
            this ContentReference link, IContentLoader loader)
        {
            var item = loader.GetChildren<IContent>(link).FirstOrDefault();
            return item == null ? string.Empty : item.Name;
        }

        private static Injected<IContentLoader> Loader { get; set; }

        public static string FirstChildName(
            this ContentReference link)
        {
            return FirstChildName(link, Loader.Service);
        }


        public static string FirstChildName(
            this IContentLoader loader, ContentReference link)
        {
            var item = loader.GetChildren<IContent>(link).FirstOrDefault();
            return item == null ? string.Empty : item.Name;
        }

        public static T GetByCode<T>(this string code, ReferenceConverter converter, IContentLoader loader)
            where T : CatalogContentBase
        {
            var link = converter.GetContentLink(code);
            return loader.Get<T>(link);
        }

        

    }

    public class ProductLoader : IProductLoader
    {
        private readonly ReferenceConverter _converter;
        private readonly IContentLoader _loader;

        public ProductLoader(ReferenceConverter converter, IContentLoader loader)
        {
            _converter = converter;
            _loader = loader;
        }

        public T GetByCode<T>(string code)
            where T : CatalogContentBase
        {
            var link = _converter.GetContentLink(code);
            return _loader.Get<T>(link);
        }
    }

    public class MethodUsage
    {
        private readonly IContentLoader _contentLoader;
        private readonly ReferenceConverter _referenceConverter;

        public MethodUsage(IContentLoader contentLoader, ReferenceConverter referenceConverter)
        {
            _contentLoader = contentLoader;
            _referenceConverter = referenceConverter;
        }

        public void Use(ContentReference link)
        {
            var name = link.FirstChildName(_contentLoader);

            var n = _contentLoader.FirstChildName(ContentReference.StartPage);
            var children = _contentLoader.GetChildren<IContent>(ContentReference.StartPage);
            // Use the name here

            var product = "ABC-123".GetByCode<ProductContent>(_referenceConverter, _contentLoader);
        }

        public void Test()
        {
            ExtensionMethods.Loader = new Injected<IContentLoader>(new StubContentLoader());

            // Rest of the test

            ContentReference.StartPage.FirstChildName(_contentLoader);

        }
    }

    public class StubContentLoader : IContentLoader
    {
        // Implementation

        public T Get<T>(Guid contentGuid) where T : IContentData
        {
            throw new System.NotImplementedException();
        }

        public T Get<T>(Guid contentGuid, LoaderOptions settings) where T : IContentData
        {
            throw new System.NotImplementedException();
        }

        public T Get<T>(Guid contentGuid, CultureInfo language) where T : IContentData
        {
            throw new System.NotImplementedException();
        }

        public T Get<T>(ContentReference contentLink) where T : IContentData
        {
            throw new System.NotImplementedException();
        }

        public T Get<T>(ContentReference contentLink, LoaderOptions settings) where T : IContentData
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable GetChildren<T>(ContentReference contentLink) where T : IContentData
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable GetChildren<T>(ContentReference contentLink, LoaderOptions settings) where T : IContentData
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable GetChildren<T>(ContentReference contentLink, LoaderOptions settings, int startIndex, int maxRows) where T : IContentData
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable GetDescendents(ContentReference contentLink)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable GetAncestors(ContentReference contentLink)
        {
            throw new System.NotImplementedException();
        }

        public IContent GetBySegment(ContentReference parentLink, string urlSegment, LoaderOptions settings)
        {
            throw new System.NotImplementedException();
        }

        public bool TryGet<T>(ContentReference contentLink, out T content) where T : IContentData
        {
            throw new System.NotImplementedException();
        }

        public bool TryGet<T>(ContentReference contentLink, LoaderOptions settings, out T content) where T : IContentData
        {
            throw new System.NotImplementedException();
        }

        public bool TryGet<T>(Guid contentGuid, LoaderOptions loaderOptions, out T content) where T : IContentData
        {
            throw new System.NotImplementedException();
        }

        public bool TryGet<T>(Guid contentGuid, CultureInfo language, out T content) where T : IContentData
        {
            throw new System.NotImplementedException();
        }

        public bool TryGet<T>(Guid contentGuid, out T content) where T : IContentData
        {
            throw new System.NotImplementedException();
        }

        public bool TryGet<T>(ContentReference contentLink, CultureInfo language, out T content) where T : IContentData
        {
            throw new System.NotImplementedException();
        }

        public IContent GetBySegment(ContentReference parentLink, string urlSegment, CultureInfo language)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable GetItems(IEnumerable contentLinks, LoaderOptions settings)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable GetItems(IEnumerable contentLinks, CultureInfo language)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable GetChildren<T>(ContentReference contentLink, CultureInfo language, int startIndex, int maxRows) where T : IContentData
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable GetChildren<T>(ContentReference contentLink, CultureInfo language) where T : IContentData
        {
            throw new System.NotImplementedException();
        }

        public T Get<T>(ContentReference contentLink, CultureInfo language) where T : IContentData
        {
            throw new System.NotImplementedException();
        }
    }
}
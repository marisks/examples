using EPiServer.Commerce.Catalog.ContentTypes;

namespace Cms.Helpers
{
    public interface IProductLoader
    {
        T GetByCode<T>(string code) where T : CatalogContentBase;
    }
}
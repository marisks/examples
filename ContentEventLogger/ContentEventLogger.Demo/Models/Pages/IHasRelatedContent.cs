using EPiServer.Core;

namespace ContentEventLogger.Demo.Models.Pages
{
    public interface IHasRelatedContent
    {
        ContentArea RelatedContentArea { get; }
    }
}

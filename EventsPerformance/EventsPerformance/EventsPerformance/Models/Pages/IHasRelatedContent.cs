using EPiServer.Core;

namespace EventsPerformance.Models.Pages
{
    public interface IHasRelatedContent
    {
        ContentArea RelatedContentArea { get; }
    }
}

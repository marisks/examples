using System;
using EPiServer;
using EPiServer.Core;

namespace ContentEventLogger
{
    public class Initializer
    {
        private readonly IContentEvents _contentEvents;
        private readonly ContentEventLogger _logger;

        public Initializer(
            IContentEvents contentEvents,
            ContentEventLogger logger)
        {
            _contentEvents = contentEvents ?? throw new ArgumentNullException(nameof(contentEvents));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initialize(InitializerSettings settings)
        {
            foreach (var contentEvent in settings.Events)
            {
                Subscribe(contentEvent);
            }
        }

        private void Subscribe(ContentEvent contentEvent)
        {
            switch (contentEvent)
            {
                case ContentEvent.Created:
                    _contentEvents.CreatedContent += _contentEvents_CreatedContent;
                    break;
                case ContentEvent.Published:
                    _contentEvents.PublishedContent += _contentEvents_PublishedContent;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(contentEvent), contentEvent, null);
            }
        }

        private void _contentEvents_PublishedContent(object sender, ContentEventArgs e)
        {
            _logger.Log("Published", e);
        }

        private void _contentEvents_CreatedContent(object sender, ContentEventArgs e)
        {
            _logger.Log("Created", e);
        }
    }
}
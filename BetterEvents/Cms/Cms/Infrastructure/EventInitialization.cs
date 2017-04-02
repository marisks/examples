using EPiServer;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using MediatR;

namespace Cms.Infrastructure
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class EventInitialization : IInitializableModule
    {
        private static bool _initialized;

        private Injected<IMediator> InjectedMediator { get; set; }
        private IMediator Mediator => InjectedMediator.Service;

        public void Initialize(InitializationEngine context)
        {
            if (_initialized)
            {
                return;
            }

            var contentEvents = context.Locate.ContentEvents();
            contentEvents.SavedContent += OnSavedContent;

            _initialized = true;
        }

        private void OnSavedContent(object sender, ContentEventArgs contentEventArgs)
        {
            var ev = new SavedContentEvent(contentEventArgs.ContentLink, contentEventArgs.Content);
            Mediator.Publish(ev);
        }

        public void Uninitialize(InitializationEngine context)
        {
            var contentEvents = context.Locate.ContentEvents();
            contentEvents.SavedContent -= OnSavedContent;
        }
    }

    public class SavedContentEvent : INotification
    {
        public SavedContentEvent(ContentReference contentLink, IContent content)
        {
            ContentLink = contentLink;
            Content = content;
        }

        public ContentReference ContentLink { get; set; }

        public IContent Content { get; set; }
    }

    public class SendAdminEmailOnSavedContent : INotificationHandler<SavedContentEvent>
    {
        private readonly IEmailService _emailService;

        public SendAdminEmailOnSavedContent(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public void Handle(SavedContentEvent notification)
        {
            // Handle event.
        }
    }

    public class LogOnSavedContent: INotificationHandler<SavedContentEvent>
    {
        public void Handle(SavedContentEvent notification)
        {
            // Handle event.
        }
    }
}
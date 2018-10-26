using System.Collections.Generic;
using System.Linq;

namespace ContentEventLogger
{
    public class InitializerSettings
    {
        public IEnumerable<ContentEvent> Events { get; private set; }

        public InitializerSettings()
        {
            Events = Enumerable.Empty<ContentEvent>();
        }

        public InitializerSettings SubscribeTo(ContentEvent contentEvent)
        {
            return new InitializerSettings
            {
                Events = Events.Union(new [] {contentEvent})
            };
        }
    }
}
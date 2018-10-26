using System;
using EPiServer;
using EPiServer.Logging;

namespace ContentEventLogger
{
    public class ContentEventLogger
    {
        private readonly LoggerSettings _settings;
        private readonly ILogger _logger = LogManager.GetLogger(typeof(ContentEventLogger));

        public ContentEventLogger(LoggerSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        public virtual void Log(string name, ContentEventArgs args)
        {
            _logger.Log(_settings.Level, $"Event: {name}; Content: {args.Content?.Name}");
        }
    }
}
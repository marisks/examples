using EPiServer.Logging;

namespace ContentEventLogger
{
    public class LoggerSettings
    {
        public LoggerSettings()
        {
            Level = Level.Information;
        }

        public Level Level { get; private set; }

        public LoggerSettings LogLevel(Level level)
        {
            return new LoggerSettings { Level = level };
        }
    }
}
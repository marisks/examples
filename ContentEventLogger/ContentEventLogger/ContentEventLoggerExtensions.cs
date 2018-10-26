using System;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace ContentEventLogger
{
    public static class ContentEventLoggerExtensions
    {
        public static void AddContentEventLogger(
            this ServiceConfigurationContext context)
        {
            context.AddContentEventLogger(_ => _);
        }

        public static void AddContentEventLogger(
            this ServiceConfigurationContext context, Func<LoggerSettings, LoggerSettings> configure)
        {
            var settings = configure(new LoggerSettings());

            context.Services.AddSingleton(settings);
            context.Services.AddSingleton<ContentEventLogger>();
            context.Services.AddSingleton<Initializer>();
        }

        public static void UseContentEventLogger(
            this InitializationEngine context, Func<InitializerSettings, InitializerSettings> configure)
        {
            var settings = configure(new InitializerSettings());

            var initializer = context.Locate.Advanced.GetInstance<Initializer>();
            initializer.Initialize(settings);
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;

namespace EventsPerformance
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class EventHandling : IInitializableModule
    {
        private const bool UseContentEvents = true;
        private const bool UseChildrenEvents = true;

        public void Initialize(InitializationEngine context)
        {
            var contentEvents = context.Locate.ContentEvents();

            if (UseContentEvents)
            {
                contentEvents.LoadingContent += ContentEvents_LoadingContent;
                contentEvents.LoadedContent += ContentEvents_LoadedContent;
                contentEvents.FailedLoadingContent += ContentEvents_FailedLoadingContent;
            }

            if (UseChildrenEvents)
            {
                contentEvents.LoadingChildren += ContentEvents_LoadingChildren;
                contentEvents.LoadedChildren += ContentEvents_LoadedChildren;
                contentEvents.FailedLoadingChildren += ContentEvents_FailedLoadingChildren;
            }
        }

        private void ContentEvents_FailedLoadingChildren(object sender, EPiServer.ChildrenEventArgs e)
        {
            RunMe();
        }

        private void ContentEvents_LoadedChildren(object sender, EPiServer.ChildrenEventArgs e)
        {
            RunMe();
        }

        private void ContentEvents_LoadingChildren(object sender, EPiServer.ChildrenEventArgs e)
        {
            RunMe();
        }

        private void ContentEvents_FailedLoadingContent(object sender, EPiServer.ContentEventArgs e)
        {
            RunMe();
        }

        private void ContentEvents_LoadedContent(object sender, EPiServer.ContentEventArgs e)
        {
            RunMe();
        }

        private async void ContentEvents_LoadingContent(object sender, EPiServer.ContentEventArgs e)
        {
            await RunMeAsync();
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void RunMe()
        {
            RunMeAsync();
            // RunMeAsync().Wait(); // Hungs
            //AsyncHelper.RunSync(RunMeAsync);
        }

        private static bool _thrown;
        public async Task RunMeAsync()
        {
            await Task.Run(
                () =>
                {
                    if (_thrown) return;
                    _thrown = true;
                    throw new Exception("Lol");
                });
        }
    }

    /// <summary>
    /// http://stackoverflow.com/a/25097498/660154
    /// </summary>
    public static class AsyncHelper
    {
        private static readonly TaskFactory MyTaskFactory = new
          TaskFactory(CancellationToken.None,
                      TaskCreationOptions.None,
                      TaskContinuationOptions.None,
                      TaskScheduler.Default);

        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return MyTaskFactory
              .StartNew(func)
              .Unwrap()
              .GetAwaiter()
              .GetResult();
        }

        public static void RunSync(Func<Task> func)
        {
            MyTaskFactory
              .StartNew(func)
              .Unwrap()
              .GetAwaiter()
              .GetResult();
        }
    }
}
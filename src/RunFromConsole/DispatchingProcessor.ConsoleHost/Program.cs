using Microsoft.Practices.DataPipeline.Processor.Instrumentation;

namespace DispatchingProcessor.ConsoleHost
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Practices.DataPipeline;
    using Microsoft.Practices.DataPipeline.Cars.Handlers;
    using Microsoft.Practices.DataPipeline.Processor;
    using Microsoft.Practices.DataPipeline.Tests;
    using Microsoft.ServiceBus;
    using Microsoft.WindowsAzure.Storage;

    internal class Program
    {
        private static void Main(string[] args)
        {
            ConsoleHost.WithOptions(
                new Dictionary<string, Func<CancellationToken, Task>>
                    {
                        {
                            "Provision Resources",
                            ProvisionResourcesAsync
                        },
                        {
                            "Run Dispatching Processor", 
                            RunAsync
                        }
                    });
        }

        private static async Task ProvisionResourcesAsync(CancellationToken token)
        {
            var configuration = DispatcherConfiguration.GetCurrentConfiguration();

            var nsm = NamespaceManager.CreateFromConnectionString(configuration.EventHubConnectionString);

            Console.WriteLine("EventHub name/path: {0}", configuration.EventHubName);

            Console.WriteLine("Confirming consumer group: {0}", configuration.ConsumerGroupName);
            await nsm.CreateConsumerGroupIfNotExistsAsync(configuration.EventHubName, configuration.ConsumerGroupName);

            Console.WriteLine("Consumer group confirmed");

            Console.WriteLine("Confirming blob container for poison messages: {0}", configuration.PoisonMessageContainer);

            var poisonStorage =
                CloudStorageAccount.Parse(configuration.PoisonMessageStorageAccount).CreateCloudBlobClient();

            var poisonContainer = poisonStorage.GetContainerReference(configuration.PoisonMessageContainer);
            await poisonContainer.CreateIfNotExistsAsync(token);
            Console.WriteLine("container confirmed");
        }

        private static async Task RunAsync(CancellationToken token)
        {
            var configuration = DispatcherConfiguration.GetCurrentConfiguration();

            // Configure dependency resolver (including poison handler)
            var resolver = await ConsoleHostDependencyResolver.CreateAsync();
            DependencyResolverFactory.Register(
                resolver
            );

            Console.WriteLine("Initializing coordinator");

            var messageProcessor = await ProcessingCoordinator.CreateAsync(
                "Console",
                configuration.EventHubName,
                configuration.ConsumerGroupName,
                configuration.EventHubConnectionString,
                configuration.CheckpointStorageAccount,
                configuration.MaxBatchSize,
                configuration.PrefetchCount,
                configuration.ReceiveTimeout,
                configuration.MaxConcurrencyPerProcessor,
                typeof(UpdateLocationHandler).Assembly.DefinedTypes,
                (name, partitionId) =>
                    new CircuitBreaker(
                        name,
                        partitionId,
                        configuration.CircuitBreakerWarningLevel,
                        configuration.CircuitBreakerTripLevel,
                        configuration.CircuitBreakerStallInterval,
                        configuration.CircuitBreakerLogCooldownInterval),
                new DispatcherInstrumentationManager(true, true).CreatePublisher("console"));

            Console.WriteLine("Running processor");

            try
            {
                await Task.Delay(Timeout.InfiniteTimeSpan, token);
            }
            catch (TaskCanceledException) { /* expected cancellation */ }

            messageProcessor.Dispose();
        }
    }
}
using System.Threading.Tasks;
using NLog;

namespace Sample
{
	public class SampleQueueProcessingService
	{
		private readonly ILogger logger = LogManager.GetCurrentClassLogger();

		private readonly IMessageHandler<SampleMessage> handler;
		private readonly IQueueController queueController;

		public SampleQueueProcessingService()
		{
			this.queueController = new AzureServiceBusQueueController();
			this.handler = new SampleMessageHandler();
		}

		public async Task StartAsync()
		{
			await this.queueController.SubscribeAsync(handler).ConfigureAwait(false);
			this.logger.Info($"{handler.GetType().Name} subscribed;");

			await queueController.EnqueueAsync(new SampleMessage()).ConfigureAwait(false);
			logger.Debug("Sample message queued - type \"exit\" to stop");
		}

		public async Task StopAsync()
		{
			await this.queueController.UnsubscribeAsync<SampleMessage>().ConfigureAwait(false);
			this.logger.Info($"{nameof(SampleQueueProcessingService)} stopped");
		}
	}
}
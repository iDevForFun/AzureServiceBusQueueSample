using System;
using NLog;

namespace Sample
{
	public class Program
	{
		private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
		
		public static void Main(string[] args)
		{
			try
			{
				logger.Debug("Starting");

				var queueController = new AzureServiceBusQueueController();
				var sampleHandler = new SampleMessageHandler();

				queueController.SubscribeAsync(sampleHandler).GetAwaiter().GetResult();
				logger.Debug("Message handler subscribed");

				queueController.EnqueueAsync(new SampleMessage()).GetAwaiter().GetResult();
				logger.Debug("Sample message queued - press any key to exit");

				Console.ReadKey();
				logger.Debug("Exit");
			}
			catch(Exception ex)
			{
				logger.Error(ex);
			}
		}
	}
}

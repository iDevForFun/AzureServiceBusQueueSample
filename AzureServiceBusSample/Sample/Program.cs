using System;
using System.Threading.Tasks;
using NLog;
using Topshelf;

namespace Sample
{
	public class Program
	{
		private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
		
		public static void Main(string[] args)
		{
			HostFactory.Run(cfg =>
			{
				cfg.Service<SampleQueueProcessingService>(s =>
				{
					s.ConstructUsing(name =>
					{
						try
						{
							return new SampleQueueProcessingService();
						}
						catch (Exception ex)
						{
							logger.Fatal(() => $"{nameof(SampleQueueProcessingService)} failed to initilaise with error -->{ex.Message}");
							throw;
						}
					});

					s.WhenStarted(qps =>
					{			
						Task.Run(qps.StartAsync).GetAwaiter().GetResult();
						logger.Debug(() => $"{nameof(SampleQueueProcessingService)} started");
					});

					s.WhenStopped(qps =>
					{
						try
						{
							Task.Run(qps.StopAsync).GetAwaiter().GetResult();
							logger.Debug(() => $"{nameof(SampleQueueProcessingService)} stopped");
						}
						catch (Exception ex)
						{
							logger.Error(() => $"{nameof(SampleQueueProcessingService)} failed to stop with error --> {ex.Message}");
							throw;
						}
					});
				});
			});
		}
	}
}

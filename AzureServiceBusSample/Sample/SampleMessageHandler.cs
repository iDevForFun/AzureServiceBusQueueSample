using System;
using System.Threading.Tasks;
using NLog;

namespace Sample
{
	public class SampleMessageHandler : IMessageHandler<SampleMessage>
	{
		private readonly ILogger logger = LogManager.GetCurrentClassLogger();

		public async Task HandleMessageAsync(SampleMessage message, Func<Task> heartbeatCallBack = null)
		{
			for (var i = 0; i < 6000; i++)
			{
				await Task.Delay(5000).ConfigureAwait(false);
				logger.Debug($"Processed i = {i}");

				if (heartbeatCallBack != null)
				{
					await heartbeatCallBack().ConfigureAwait(false);
					logger.Debug($"RenewLock called for i = {i}");
				}
			}
		}
	}
}

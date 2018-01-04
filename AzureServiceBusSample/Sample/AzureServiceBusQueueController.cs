using System;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using NLog;

namespace Sample
{
	public class AzureServiceBusQueueController : IQueueController
	{
		private readonly string connectionString = ConfigurationManager.ConnectionStrings["azureServiceBus"].ConnectionString;
		
		public async Task EnqueueAsync<TMessage>(TMessage message) where TMessage : class, IMessage
		{
			await InitQueueAsync<TMessage>().ConfigureAwait(false);

			using (var brokeredMessage = new BrokeredMessage(message) {ContentType = "application/json"})
			{
				await GetQueue<TMessage>().SendAsync(brokeredMessage).ConfigureAwait(false);
			}
		}

		public async Task SubscribeAsync<TMessage>(IMessageHandler<TMessage> handler) where TMessage : class, IMessage
		{
			await InitQueueAsync<TMessage>().ConfigureAwait(false);

			GetQueue<TMessage>().OnMessageAsync(
				async msg =>
				{
					try
					{
						var body = msg.GetBody<TMessage>();
						
						await handler.HandleMessageAsync(body, msg.RenewLockAsync).ConfigureAwait(false);
						await msg.CompleteAsync().ConfigureAwait(false);
					}
					catch (Exception)
					{
						await msg.AbandonAsync().ConfigureAwait(false);
					}
				},
				new OnMessageOptions
				{
					AutoComplete = false 
				}
			);
		}

		public async Task UnsubscribeAsync<TMessage>() where TMessage : class, IMessage
		{
			if (!await QueueExistsAsync<TMessage>().ConfigureAwait(false))
			{
				return;
			}

			await GetQueue<TMessage>().CloseAsync().ConfigureAwait(false);
		}

		private Task<bool> QueueExistsAsync<TMessage>() => NamespaceManager.CreateFromConnectionString(connectionString).QueueExistsAsync(typeof(TMessage).Name);
		
		private async Task InitQueueAsync<TMessage>()
		{
			if (await QueueExistsAsync<TMessage>().ConfigureAwait(false))
			{
				return;
			}

			await NamespaceManager.CreateFromConnectionString(connectionString).CreateQueueAsync(typeof(TMessage).Name).ConfigureAwait(false);
		}

		private QueueClient GetQueue<TMessage>() => QueueClient.CreateFromConnectionString(connectionString, typeof(TMessage).Name, ReceiveMode.PeekLock);
	}
}

using System.Threading.Tasks;

namespace Sample
{
	public interface IQueueController
	{
		Task EnqueueAsync<TMessage>(TMessage message) where TMessage : class, IMessage;
		Task SubscribeAsync<TMessage>(IMessageHandler<TMessage> handler) where TMessage : class, IMessage;
		Task UnsubscribeAsync<TMessage>() where TMessage : class, IMessage;
	}
}
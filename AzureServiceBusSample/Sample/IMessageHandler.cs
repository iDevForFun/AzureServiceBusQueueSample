using System;
using System.Threading.Tasks;

namespace Sample
{ 
	public interface IMessageHandler<in T> : IMessage
	{
		Task HandleMessageAsync(T message, Func<Task> heartbeatCallBack = null);
	}
}
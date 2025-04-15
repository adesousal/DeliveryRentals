using Confluent.Kafka;
using System.Text.Json;

namespace DeliveryRentals.Infrastructure.Messaging
{
	public class KafkaEventPublisher : IEventPublisher
	{
		private readonly IProducer<Null, string> _producer;
		private readonly string _topic;

		public KafkaEventPublisher(string bootstrapServers, string topic = "motos")
		{
			_topic = topic;

			var config = new ProducerConfig
			{
				BootstrapServers = bootstrapServers,
				MessageTimeoutMs = 5000
			};

			_producer = new ProducerBuilder<Null, string>(config).Build();
		}

		public async Task PublishAsync<T>(T @event) where T : class
		{
			var json = JsonSerializer.Serialize(@event);

			// Without OutboxEvent implemented, I just throw a message to the console
			try
			{
				await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = json });
			}
			catch (ProduceException<Null, string> ex)
			{
				Console.WriteLine("Failed to publish event to Kafka", ex);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Unexpected error while publishing to Kafka", ex);
			}
		}
	}
}

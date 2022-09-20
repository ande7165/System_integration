using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;



var factory = new ConnectionFactory() { HostName = "localhost" };

var messages = new List<string>();

using (var connection = factory.CreateConnection()) { 
	using (var channel = connection.CreateModel())
	{
		channel.ExchangeDeclare(exchange: "topic_logs", type: "topic");
		var queueName = channel.QueueDeclare().QueueName;

			channel.QueueBind(queue: queueName,
							  exchange: "topic_logs",
							  routingKey: "Tour.*");

		Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");

		var consumer = new EventingBasicConsumer(channel);
		consumer.Received += (model, ea) =>
		{
			var body = ea.Body.ToArray();
			var message = Encoding.UTF8.GetString(body);

			messages.Add(message);

			var routingKey = ea.RoutingKey;
			Console.WriteLine(" BackOffice [x] Received '{0}':'{1}'",
							  routingKey,
							  message);
		};
		channel.BasicConsume(queue: queueName,
							 autoAck: true,
							 consumer: consumer);

		Console.WriteLine(" Press [enter] to exit.");
		Console.ReadLine();

	}
}

using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory() { HostName = "localhost" };

var messages = new List<string>();

using (var connection = factory.CreateConnection())
{
	using (var channel = connection.CreateModel())
	{
		channel.ExchangeDeclare(exchange: "dlx", type: ExchangeType.Fanout);

		var queueName = channel.QueueDeclare().QueueName;

		channel.QueueDeclare(queue: "dlxexchangequeue");

		channel.QueueBind("dlxexchangequeue", "dlx", "");


		var consumer = new EventingBasicConsumer(channel);
		consumer.Received += (model, ea) =>
		{
			var body = ea.Body.ToArray();
			var message = Encoding.UTF8.GetString(body);

			messages.Add(message);

			var routingKey = ea.RoutingKey;
			Console.WriteLine(" Admin, DeadMessage [x] Received : '{0}'",
							  message);
		};
		channel.BasicConsume(queue: "dlxexchangequeue",
							 autoAck: true,
							 consumer: consumer);

		Console.WriteLine(" Press [enter] to exit.");
		Console.ReadLine();
	}
}

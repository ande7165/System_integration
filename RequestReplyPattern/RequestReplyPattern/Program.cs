using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory() { HostName = "localhost" };

using (var connection = factory.CreateConnection())
{
	using(var channel = connection.CreateModel())
	{
		channel.QueueDeclare(queue: "rr_queue", durable:false, exclusive:false, autoDelete:false, arguments: null);

		channel.BasicQos(0, 1, false);

		var consumer = new EventingBasicConsumer(channel);

		channel.BasicConsume(queue: "rr_queue", autoAck: false, consumer: consumer);

		Console.WriteLine("Awaiting Request");

		consumer.Received += (model, ea) =>
		{
			string response = null;

			var body = ea.Body.ToArray();

			var props = ea.BasicProperties;

			var replyProps = channel.CreateBasicProperties();

			replyProps.CorrelationId = props.CorrelationId;

			try
			{
				var message = Encoding.UTF8.GetString(body);
				Console.WriteLine("Message:" + message);
				response = "Message has been received in the server and this is a reply to the client";
			}
			catch (Exception e)
			{
				Console.WriteLine(" [.] " + e.Message);
				response = "";
			}
			finally
			{
				var responseBytes = Encoding.UTF8.GetBytes(response);
				channel.BasicPublish(exchange: "", routingKey: props.ReplyTo,
				  basicProperties: replyProps, body: responseBytes);
				channel.BasicAck(deliveryTag: ea.DeliveryTag,
				  multiple: false);
			}
		};

		Console.WriteLine(" Press [enter] to exit.");
		Console.ReadLine();
	}
}

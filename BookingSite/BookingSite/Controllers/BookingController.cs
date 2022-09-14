using BookingSite.Model;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Linq;
using RabbitMQ.Client;
using System.Text.Json;
using System.Diagnostics;

namespace BookingSite.Controllers
{
	public class BookingController : Controller { 

		public IActionResult Index()
		{
			return View();
		}

		[HttpPost]
		public IActionResult SendMessage(Booking booking)
		{
			if(booking != null)
			{
				var factory = new ConnectionFactory() { HostName = "localhost" };
				using (var connection = factory.CreateConnection())
				using (var channel = connection.CreateModel())
				{
					channel.ExchangeDeclare(exchange: "topic_logs",
											type: "topic");

					var routingKey = "Booking.Message";
					var message = JsonSerializer.Serialize(booking);
					var body = Encoding.UTF8.GetBytes(message);
					channel.BasicPublish(exchange: "topic_logs",
										 routingKey: routingKey,
										 basicProperties: null,
										 body: body);

					Debug.WriteLine(" [x] Sent '{0}':'{1}'", routingKey, message);
				}

			}
			return RedirectToAction("Index");
		}
	}
}

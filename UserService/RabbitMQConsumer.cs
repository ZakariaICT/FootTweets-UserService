using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using UserService.Model;

namespace UserService
{
    public class RabbitMQConsumer
    {
        private readonly IModel _channel;

        public RabbitMQConsumer(IModel channel)
        {
            _channel = channel;
        }

        public void StartListening(string queueName)
        {
            _channel.QueueDeclare(queue: queueName,
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                // Extract UID from the request (customize this based on your actual implementation)
                var uid = ExtractUidFromRequest(message);

                // Send UID to another RabbitMQ queue
                SendUidToQueue(uid);
            };

            _channel.BasicConsume(queue: queueName,
                                  autoAck: true,
                                  consumer: consumer);

            Console.WriteLine($"Listening to RabbitMQ queue: {queueName}");
        }

        private string ExtractUidFromRequest(string message)
        {
            try
            {
                // Assuming the message is a JSON with a "uid" property
                var tweetDetails = Newtonsoft.Json.JsonConvert.DeserializeObject<UserRequest>(message);

                // Return the UID from the tweetDetails
                return tweetDetails.Uid;
            }
            catch (Exception ex)
            {
                // Handle exceptions or log them
                Console.WriteLine($"Error extracting UID from request: {ex.Message}");
                return null; // Or throw an exception or handle accordingly
            }
        }


        private void SendUidToQueue(string uid)
        {
            // Replace "uid_queue" with the actual queue name where UID messages should be sent
            _channel.BasicPublish(exchange: "",
                                  routingKey: "uid_queue",
                                  basicProperties: null,
                                  body: Encoding.UTF8.GetBytes(uid));
        }
    }
}

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

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

                // Process the message (assuming it contains tweet creation request details)
                // Send a response with the UID to another queue

                // Example: Assuming message is a JSON with tweet details, extract and process
                var tweetDetails = Newtonsoft.Json.JsonConvert.DeserializeObject<TweetDetails>(message);

                // Perform actions to get the UID (replace this with your actual logic)
                string uid = GetUidFromUserService(tweetDetails);

                // Send UID to another RabbitMQ queue
                SendUidToQueue(uid);
            };

            _channel.BasicConsume(queue: queueName,
                                  autoAck: true,
                                  consumer: consumer);

            Console.WriteLine($"Listening to RabbitMQ queue: {queueName}");
        }

        private void SendUidToQueue(string uid)
        {
            // Replace "uid_queue" with the actual queue name where UID messages should be sent
            _channel.BasicPublish(exchange: "",
                                  routingKey: "uid_queue",
                                  basicProperties: null,
                                  body: Encoding.UTF8.GetBytes(uid));
        }

        private string GetUidFromUserService(TweetDetails tweetDetails)
        {
            // Implement logic to get UID from UserService based on tweet details
            // Replace this with your actual logic

            // For simplicity, just return a hardcoded UID for demonstration purposes
            return "hardcoded_uid_for_demo";
        }
    }

    public class TweetDetails
    {
        // Define your tweet details properties based on your application needs
        [Required]
        public string Text { get; set; }
        [Required]

        public string PictureURL { get; set; }
        [Required]

        public string Uid { get; set; }
    }
}

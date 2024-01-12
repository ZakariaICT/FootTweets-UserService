using System.Text;
using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class RabbitMQService
{
    private readonly IModel _channel;

    public RabbitMQService(IModel channel)
    {
       _channel = channel;
    }

    public void SendMessage(string message, string uid)
    {
        try
        {
            // Send a message to user_registration_queue
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "", routingKey: "user_registration_queue", basicProperties: null, body: body);

            Console.WriteLine($" [x] Sent to 'user_registration_queue': '{message}'");

            // Send UID to uid_queue
            var uidBody = Encoding.UTF8.GetBytes(uid);
            _channel.BasicPublish(exchange: "", routingKey: "uid_queue", basicProperties: null, body: uidBody);

            Console.WriteLine($" [x] Sent to 'uid_queue': '{uid}'");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message: {ex.Message}");
        }
    }



    public void ListenForMessages()
    {
       // Declare the queue if it doesn't exist
       _channel.QueueDeclare(queue: "user_registration_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

       // Set up the consumer
       var consumer = new EventingBasicConsumer(_channel);
       consumer.Received += (model, ea) =>
       {
           var body = ea.Body.ToArray();
           var message = Encoding.UTF8.GetString(body);

           // Process the message (e.g., create a user in the database)
           Console.WriteLine($" [x] Received '{message}'");

           // Acknowledge the message
           _channel.BasicAck(ea.DeliveryTag, false);
       };

       // Start consuming messages
       _channel.BasicConsume(queue: "user_registration_queue", autoAck: false, consumer: consumer);
    }
}

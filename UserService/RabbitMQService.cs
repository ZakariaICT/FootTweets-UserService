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

    public void SendMessage(string message)
    {
       // Send a message
       var body = Encoding.UTF8.GetBytes(message);

       _channel.BasicPublish(exchange: "", routingKey: "user_registration_queue", basicProperties: null, body: body);

       Console.WriteLine($" [x] Sent '{message}'");
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

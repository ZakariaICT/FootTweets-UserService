using System.Text;
using System;
using RabbitMQ.Client;

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
}

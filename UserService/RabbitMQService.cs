using System.Text;
using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using UserService.Model;

public class RabbitMQService
{
    private readonly IModel _channel;

    public RabbitMQService(IModel channel)
    {
       _channel = channel;
    }

    public RabbitMQService()
    {

    }

    public void SendMessage(string message, string uid)
    {
        try
        {
            // Send a message to user_registration_queue
            //var body = Encoding.UTF8.GetBytes(message);
            //_channel.BasicPublish(exchange: "", routingKey: "user_registration_queue", basicProperties: null, body: body);

            //Console.WriteLine($" [x] Sent to 'user_registration_queue': '{message}'");

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

    public void DeleteUser(string uid)
    {
        // Publish a message to RabbitMQ for other services to consume
        var message = new
        {
            UserId = uid
        };

        var messageBytes = Encoding.UTF8.GetBytes(Newtonsoft.Json.JsonConvert.SerializeObject(message));

        _channel.BasicPublish(exchange: "",
                              routingKey: "user.deleted", // RabbitMQ topic
                              basicProperties: null,
        body: messageBytes);

        Console.WriteLine($"User with ID {uid} deleted and message published.");
    }
}

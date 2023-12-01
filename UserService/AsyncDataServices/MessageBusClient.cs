using UserService.DTO;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System.Text;

namespace UserService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(_configuration["RabbitMQPort"])
            };
            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                Console.WriteLine("---> Connected to Message Bus")
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the messagebus: {ex.Message}");
            }
            
        }
        void PublishNewUser(UserPublishedDto userPublishedDto)
        {
            var message = JsonSerializer.Serialize(userPublishedDto);

            if (_connection.IsOpen)
            {
                Console.WriteLine(" ---> RabbitMQ connection is open, Sending message.......");
                SendMessage(message);
            }
            else
            {
                Console.WriteLine("--> RabbitMQ connection is closed, not sending")
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: "trigger", routingKey: "", basicProperties: null, body: body);

            Console.WriteLine($"---> We have sent {message}")
        }

        public void Dispose()
        {
            Console.WriteLine("Message bus disposed");
            if(_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine(" --->  Connection has been shutdown!")
        }
    }
}
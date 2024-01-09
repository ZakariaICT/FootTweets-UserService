using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using UserService.DTO;
using UserService.Model;
using UserService.Repositories;

namespace UserService.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private IUserRepo _userRepo;
        private IMapper _mapper;

        public UserController(IConfiguration configuration, IUserRepo userRepo, IMapper mapper)
        {
            _configuration = configuration;
            _userRepo = userRepo;
            _mapper = mapper;

            // Start listening for RabbitMQ messages
            ListenForRabbitMQMessages();
        }

        [HttpGet("all")]
        public ActionResult<IEnumerable<UsersReadDTO>> GetUsers()
        {
            Console.WriteLine("--> Getting Users.....");

            var users = _userRepo.GetAllUsers();

            return Ok(_mapper.Map<IEnumerable<UsersReadDTO>>(users));
        }

        [HttpGet("{id}", Name = "GetUserByID")]
        public ActionResult<UsersReadDTO> GetUserByID(Guid id)
        {
            var users = _userRepo.GetUserByID(id);
            if (users != null)
            {
                return Ok(_mapper.Map<UsersReadDTO>(users));
            }

            return NotFound();
        }

        // Switch Usermodelgebruik naar UserDTO.
        [HttpPost("post")]
        public ActionResult<UsersReadDTO> CreateUser(UserDTO user)
        {
            var userModel = _mapper.Map<Users>(user);
            userModel.Id = Guid.NewGuid();
            _userRepo.CreateUser(userModel);
            _userRepo.saveChanges();

            var userDTO = _mapper.Map<UsersReadDTO>(userModel);

            // Send RabbitMQ message
            try
            {
                var factory = new ConnectionFactory
                {
                    Uri = new Uri(_configuration["RabbitMQConnection"])
                };

                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    var rabbitMQService = new RabbitMQService(channel);
                    rabbitMQService.SendMessage($"New user created: {userDTO.Username}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not send RabbitMQ message: {ex.Message}");
            }

            return CreatedAtRoute(nameof(GetUserByID), new { Id = userDTO.Id }, userDTO);
        }

        private void ListenForRabbitMQMessages()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    Uri = new Uri(_configuration["RabbitMQConnection"])
                };

                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    // Declare the queue if it doesn't exist
                    channel.QueueDeclare(queue: "user_creation_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

                    // Set up the consumer
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = System.Text.Encoding.UTF8.GetString(body);

                        // Process the message and create a user in the database
                        CreateUserInDatabase(message);

                        // Acknowledge the message
                        channel.BasicAck(ea.DeliveryTag, false);
                    };

                    // Start consuming messages
                    channel.BasicConsume(queue: "user_creation_queue", autoAck: false, consumer: consumer);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not set up RabbitMQ consumer: {ex.Message}");
            }
        }

        private void CreateUserInDatabase(string message)
        {
            // Process the RabbitMQ message and create a user in the database
            // Example: Deserialize JSON message or parse the message content
            var user = new Users(); // Replace with your actual Users model
            // Populate user properties from the message

            _userRepo.CreateUser(user);
            _userRepo.saveChanges();
        }
    }
}

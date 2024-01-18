using AutoMapper;
using Castle.Core.Configuration;
using Microsoft.AspNetCore.Mvc;
using UserService.DTO;
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

        [HttpGet("by-uidAuth/{uidAuth}", Name = "GetUserByUidAuth")]
        public ActionResult<UsersReadDTO> GetUserByUidAuth(string uidAuth)
        {
            var user = _userRepo.GetUserByUidAuth(uidAuth);
            if (user != null)
            {
                return Ok(_mapper.Map<UsersReadDTO>(user));
            }

            return NotFound();
        }

        // Switch Usermodelgebruik naar UserDTO.
        [HttpPost("post")]
        public ActionResult<UsersReadDTO> CreateUser(UserDTO user)
        {
            try
            {
                var userModel = _mapper.Map<Users>(user);
                // userModel.Uid = Guid.NewGuid();
                // _userRepo.CreateUser(userModel);
                // _userRepo.saveChanges();

                var userDTO = _mapper.Map<UsersReadDTO>(userModel);

                // Send RabbitMQ message with UID
                var factory = new ConnectionFactory
                {
                    Uri = new Uri(_configuration["RabbitMQConnection"])
                };

                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    var rabbitMQService = new RabbitMQService(channel);
                    rabbitMQService.SendMessage($"New user created: {userDTO.UidAuth}", userDTO.UidAuth.ToString());

                    // Process the message immediately in the database
                    ProcessMessageLocally(userDTO);
                }

                return CreatedAtRoute(nameof(GetUserByID), new { Id = userDTO.UidAuth }, userDTO);
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine($"Error creating user: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }


        private void ProcessMessageLocally(UsersReadDTO userDTO)
        {
            // Process the message (e.g., create a user in the database)
            Console.WriteLine($" [x] Received 'New user created: {userDTO.Username}'");

            // Save the user to the database
            var userModel = _mapper.Map<Users>(userDTO);
            //userModel.Uid = Guid.NewGuid();
            _userRepo.CreateUser(userModel);
            _userRepo.saveChanges();
        }



        [HttpPut("{id}")]
        public ActionResult UpdateUser(Guid id, UserDTO userDto)
        {
            var userFromRepo = _userRepo.GetUserByID(id);
            if (userFromRepo == null)
            {
                return NotFound();
            }

            // Additional logic to ensure the user can only update their own account

            _mapper.Map(userDto, userFromRepo);
            _userRepo.UpdateUser(userFromRepo);
            _userRepo.saveChanges();

            return NoContent();
        }

        [HttpDelete("{uidAuth}")]
        public ActionResult DeleteUser(string uidAuth)
        {
            var userFromRepo = _userRepo.GetUserByUidAuth(uidAuth);
            if (userFromRepo == null)
            {
                return NotFound();
            }

            // Additional logic to ensure the user can only delete their own account

            _userRepo.DeleteUser(userFromRepo);
            _userRepo.saveChanges();

            return NoContent();
        }





    }
}

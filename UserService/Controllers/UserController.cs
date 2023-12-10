using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserService.DTO;
using UserService.Model;
using UserService.Repositories;
using UserService.AsyncDataServices;

namespace UserService.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserRepo _userRepo;
        private IMapper _mapper;
        private readonly IMessageBusClient _messageBusClient;


        public UserController(IUserRepo userRepo, IMapper mapper, IMessageBusClient messageBusClient)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _messageBusClient = messageBusClient;
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

            // Send Async Message
            try
            {
                var userPublishedDto = _mapper.Map<UserPublishedDto>(userDTO);
                userPublishedDto.Event = "User_Published";
                _messageBusClient.PublishNewUser(userPublishedDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not send asynchronously: {ex.Message}");
            }

            return CreatedAtRoute(nameof(GetUserByID), new { Id = userDTO.Id }, userDTO);
        }
    }
}

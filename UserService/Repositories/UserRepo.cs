using System.Reflection.Metadata.Ecma335;
using UserService.Data;
using UserService.Model;

namespace UserService.Repositories
{
    public class UserRepo : IUserRepo
    {
        private readonly AppDbContext _context;
        private readonly RabbitMQService _rabbitMQService;

        public UserRepo(AppDbContext context, RabbitMQService rabbitMQService)
        {
            _context = context;
            _rabbitMQService = rabbitMQService;
        }

        public void CreateUser(Users user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            _context.users.Add(user);
        }

        public IEnumerable<Users> GetAllUsers()
        {
            return _context.users.ToList();
        }

        public Users GetUserByID(Guid Guid)
        {
            return _context.users.FirstOrDefault(u => u.Uid == Guid);
        }

        public Users GetUserByUidAuth(string ID)
        {
            return _context.users.FirstOrDefault(u => u.UidAuth == ID);
        }

        public bool saveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }


        public void UpdateUser(Users user)
        {
            // No implementation required with EF Core,
            // but you might want to check if the user exists
        }

        public void DeleteUser(Users user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            _rabbitMQService.DeleteUser(user.UidAuth);
            _context.users.Remove(user);
        }

    }
}

using System.Reflection.Metadata.Ecma335;
using UserService.Data;
using UserService.Model;

namespace UserService.Repositories
{
    public class UserRepo : IUserRepo
    {
        private readonly AppDbContext _context;

        public UserRepo(AppDbContext context)
        {
            _context = context;
        }

        public void CreateUser(Users user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            _context.User.Add(user);
        }

        public IEnumerable<Users> GetAllUsers()
        {
            return _context.User.ToList();
        }

        public Users GetUserByID(Guid Guid)
        {
            return _context.User.FirstOrDefault(u => u.Id == Guid);
        }

        public bool saveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}

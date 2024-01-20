using UserService.Model;

namespace UserService.Repositories
{
    public interface IUserRepo
    {
        bool saveChanges();

        IEnumerable<Users> GetAllUsers();

        Users GetUserByID(Guid Guid);

        void CreateUser(Users user);

        Users GetUserByUidAuth(string ID);

        void UpdateUser(Users user);

        void DeleteUser(Users user);

        void RemoveDuplicateUsers();
    }
}

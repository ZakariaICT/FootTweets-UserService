using UserService.Model;
using UserserviceTests;

namespace UserService.Repositories
{
    public interface IUserRepo
    {
        bool saveChanges();

        IEnumerable<Model.Users> GetAllUsers();

        Model.Users GetUserByID(Guid Guid);

        void CreateUser(Model.Users user);

        Model.Users GetUserByUidAuth(string ID);

        void UpdateUser(Model.Users user);

        void DeleteUser(Model.Users user);
    }
}

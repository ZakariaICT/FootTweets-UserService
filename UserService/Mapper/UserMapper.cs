using AutoMapper;
using UserService.DTO;
using UserService.Model;

namespace UserService.Mapper
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<Users, UsersReadDTO>();
            CreateMap<UserDTO, Users>();
            CreateMap<UsersReadDTO, UserPublishedDto>();
        }
    }
}

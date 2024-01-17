﻿using System.Reflection.Metadata.Ecma335;
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

            _context.users.Remove(user);
        }

    }
}

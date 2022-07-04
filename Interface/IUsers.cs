using System;
using jwt_auth.Models;

namespace jwt_auth.Interface
{
    public interface IUsers
    {
        public void AddUser (User user);

        public void UpdateUser (User user);
        public User DeleteUser (int id);
    }
}

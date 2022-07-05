using System;
using jwt_auth.Data;
using jwt_auth.Interface;
using jwt_auth.Models;
using Microsoft.EntityFrameworkCore;

namespace jwt_auth.Repository
{
    public class UserRepository : IUsers
    {
        readonly AppDbContext _context = new();

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public void AddUser(User user)
        {
            try
            {
                _context.Add(user);
                _context.SaveChanges();
            }
            catch
            {
                throw;
            }
        }

        public User DeleteUser(int id)
        {
            try
            {
                User? user = _context.Users!.Find(id);
                if (user != null)
                {
                    _context.Users.Remove(user);
                    _context.SaveChanges();
                    return user;
                }
                else
                {
                    throw new ArgumentNullException();
                }
            }
            catch
            {
                throw;
            }
        }

        public void UpdateUser(User user)
        {
            try
            {
                _context.Entry(user).State = EntityState.Modified;
                _context.SaveChanges();
            }
            catch
            {
                throw;
            }
        }
    }
}

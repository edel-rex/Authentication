using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using jwt_auth.Data;
using jwt_auth.Interface;
using jwt_auth.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace jwt_auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        private static User user = new();

        protected readonly IConfiguration _configuration;
        protected readonly IUsers _IUser;


        public AuthController(IConfiguration configuration, IUsers IUser, AppDbContext context)
        {
            _configuration = configuration;
            _IUser = IUser;
            _context = context;
        }


        [HttpPost("register")]
        public async Task<ActionResult<string>> Register(UserDTO request)
        {
            
            if (_context.Users!.Where(u => u.Username == request.Username).IsNullOrEmpty())
            {
                
                CreatePasswordHash(request.Password!, out byte[] passwordHash, out byte[] passwordSalt);
                user.Username = request.Username;
                user.PasswordSalt = passwordSalt;
                user.PasswordHash = passwordHash;

                Console.WriteLine(user.PasswordHash);
                _IUser.AddUser(user);
                return Ok(_context);
            }
            else
            {
                var result = await Login(request: request);
                return result;


            }


        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDTO request)
        {
            if (_context.Users!.Where(u => u.Username == request.Username).IsNullOrEmpty()) return BadRequest("User not found");

            byte[]? passwordHash = _context.Users!.Single(u => u.Username == request.Username).PasswordHash;
            byte[]? passwordSalt = _context.Users!.Single(u => u.Username == request.Username).PasswordSalt;

            if (!VerifyPasswordHash(request.Password!, passwordHash!, passwordSalt!)) return BadRequest("Wrong Credentials");

            string token = CreateToken(request);
            return Ok(token);

        }

        [NonAction]
        private string CreateToken(UserDTO request)
        {
            List<Claim> claims = new List<Claim>{
                new Claim(ClaimTypes.Name, request.Username!)
             };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:SecretKey").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        [NonAction]
        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            }
        }

        [NonAction]
        private bool VerifyPasswordHash(string password, byte[] PasswordHash, byte[] PasswordSalt)
        {
            using (var hmac = new HMACSHA512(PasswordSalt))
            {
                var ComputedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                Console.WriteLine(ComputedHash.ToString());
                return ComputedHash.SequenceEqual(PasswordHash);
            }
        }
    }
}
